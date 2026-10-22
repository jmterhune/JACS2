/*
 * session-monitor.js
 *
 * Pops a Bootstrap 5 modal warningMinutes before the forms-auth ticket
 * actually expires, warning the user their session is about to end.
 * "Stay signed in" reloads the current page (the request itself refreshes
 * the sliding-expiration cookie) and restarts the timer. Abandoning the
 * session — either by clicking "Sign out" or by letting the countdown run
 * out — navigates to DNN's logoff handler, which clears the auth cookie
 * and redirects to the portal home page on its own.
 *
 * Usage (emitted from CourtCounselModuleBase / JudicialReferralModuleBase):
 *   SessionMonitor.init({
 *     timeoutMinutes: 60,      // forms-auth timeout, fallback only
 *     secondsRemaining: 3600,  // auth ticket's real remaining lifetime
 *     warningMinutes: 20,
 *     logoffUrl: '/ctl/Logoff'
 *   });
 *
 * secondsRemaining is authoritative; timeoutMinutes is only used if the
 * server couldn't read the ticket. See CourtCounselModuleBase for why the
 * distinction matters (sliding expiration only renews past the halfway
 * point of the window).
 *
 * Re-init is safe — calling init() again re-anchors the countdown without
 * duplicating the modal or its handlers.
 */
(function () {
    var SessionMonitor = {
        cfg: null,
        warningTimer: null,
        expiryTimer: null,
        countdownTimer: null,
        modal: null,
        expiryTime: 0,
        wired: false,

        init: function (opts) {
            this.cfg = opts;
            try { console.log('SessionMonitor cfg:', opts); } catch (e) { }

            // Anchor to the auth ticket's real remaining lifetime, which the
            // server recomputes on every render. Assuming a full timeout
            // window instead would over-estimate: ASP.NET reissues a
            // sliding-expiration cookie only once a request arrives past the
            // halfway point, so an early page load doesn't extend anything.
            var remainingMs = opts.secondsRemaining > 0
                ? opts.secondsRemaining * 1000
                : opts.timeoutMinutes * 60 * 1000;
            this.expiryTime = Date.now() + remainingMs;

            this.start();

            if (!this.wired && typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                var mgr = Sys.WebForms.PageRequestManager.getInstance();
                var self = this;
                mgr.add_endRequest(function () { self.start(); });
                this.wired = true;
            }
        },

        // Re-arms the timers against the expiry set by init(). Note this
        // does NOT push the expiry out — only a fresh init() from the
        // server can do that, and it only happens when the server actually
        // renewed the ticket. An UpdatePanel postback that didn't renew
        // therefore keeps counting down to the true expiry.
        start: function () {
            this.clearTimers();

            var remainingMs = this.expiryTime - Date.now();
            var warningMs = this.cfg.warningMinutes * 60 * 1000;
            var self = this;

            // Already inside the warning window (less time left than the
            // lead time) — warn right away rather than scheduling it into
            // the past.
            if (warningMs >= remainingMs) warningMs = remainingMs;

            // A fresh init() may have renewed the ticket and pushed the
            // expiry back outside the warning window while the modal was
            // still up — drop the now-stale warning instead of leaving it
            // on screen.
            if (this.modal && remainingMs > warningMs) this.modal.hide();

            this.warningTimer = setTimeout(function () { self.showWarning(); }, remainingMs - warningMs);
            this.expiryTimer = setTimeout(function () { self.logOff(); }, remainingMs);
        },

        clearTimers: function () {
            if (this.warningTimer) { clearTimeout(this.warningTimer); this.warningTimer = null; }
            if (this.expiryTimer) { clearTimeout(this.expiryTimer); this.expiryTimer = null; }
            if (this.countdownTimer) { clearInterval(this.countdownTimer); this.countdownTimer = null; }
        },

        ensureModal: function () {
            if (this.modal) return;
            var html =
                '<div class="modal" tabindex="-1" id="sessionExpiryModal" aria-hidden="true">' +
                '  <div class="modal-dialog modal-dialog-centered">' +
                '    <div class="modal-content">' +
                '      <div class="modal-header">' +
                '        <h5 class="modal-title"><i class="fas fa-clock"></i>&nbsp;Session about to expire</h5>' +
                '      </div>' +
                '      <div class="modal-body">' +
                '        <p>You will be signed out in <strong><span id="sessionCountdown">20:00</span></strong> due to inactivity.</p>' +
                '        <p>Do you want to stay signed in?</p>' +
                '      </div>' +
                '      <div class="modal-footer">' +
                '        <button type="button" class="btn btn-secondary" id="sessionSignOutBtn">Sign out</button>' +
                '        <button type="button" class="btn btn-primary" id="sessionStayBtn">Stay signed in</button>' +
                '      </div>' +
                '    </div>' +
                '  </div>' +
                '</div>';
            document.body.insertAdjacentHTML('beforeend', html);
            var modalEl = document.getElementById('sessionExpiryModal');
            this.modal = bootstrap.Modal.getOrCreateInstance(modalEl, { backdrop: 'static', keyboard: false });

            var self = this;
            document.getElementById('sessionStayBtn').addEventListener('click', function () { self.keepAlive(); });
            document.getElementById('sessionSignOutBtn').addEventListener('click', function () { self.logOff(); });
        },

        showWarning: function () {
            this.ensureModal();
            this.modal.show();
            this.updateCountdown();
            var self = this;
            this.countdownTimer = setInterval(function () { self.updateCountdown(); }, 1000);
        },

        updateCountdown: function () {
            var remaining = Math.max(0, this.expiryTime - Date.now());
            var mins = Math.floor(remaining / 60000);
            var secs = Math.floor((remaining % 60000) / 1000);
            var el = document.getElementById('sessionCountdown');
            if (el) { el.textContent = mins + ':' + (secs < 10 ? '0' : '') + secs; }
            if (remaining <= 0) {
                if (this.countdownTimer) { clearInterval(this.countdownTimer); this.countdownTimer = null; }
            }
        },

        keepAlive: function () {
            // Reload the current page. The request carries the auth cookie,
            // the server's sliding-expiration logic issues a fresh Set-Cookie
            // on the response, and the page's own re-render restarts the
            // timers. If the session had actually already expired, the
            // reload lands on DNN's login page like any other request would.
            this.clearTimers();
            window.location.reload();
        },

        logOff: function () {
            // The user abandoned the session — either clicked "Sign out" or
            // let the countdown run out. Navigate to DNN's logoff handler
            // the same way the site's own logout link does: a background
            // fetch() here did NOT end the session, and the redirect that
            // followed it just left the user on the home page still signed
            // in. DNN clears the auth cookie and handles the redirect home
            // itself, so there's nothing to do afterwards.
            this.clearTimers();
            window.location.href = this.cfg.logoffUrl;
        }
    };

    window.SessionMonitor = SessionMonitor;
})();
