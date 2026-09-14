/*
 * session-monitor.js
 *
 * Pops a Bootstrap 5 modal at (timeoutMinutes - warningMinutes) into the
 * forms-auth window warning the user their session is about to expire.
 * "Stay signed in" reloads the current page (the request itself refreshes
 * the sliding-expiration cookie) and restarts the timer. If the countdown
 * runs out, the user is logged off (auth cookie cleared) and sent to the
 * portal home page.
 *
 * Usage (emitted from CourtCounselModuleBase / JudicialReferralModuleBase):
 *   SessionMonitor.init({
 *     timeoutMinutes: 90,
 *     warningMinutes: 5,
 *     homeUrl: '/',
 *     logoffUrl: '/ctl/Logoff'
 *   });
 *
 * Re-init is safe — calling init() again restarts the timers without
 * duplicating the modal or its handlers. The base class also re-runs on
 * UpdatePanel endRequest so partial postbacks reset the clock.
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
            this.start();

            if (!this.wired && typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                var mgr = Sys.WebForms.PageRequestManager.getInstance();
                var self = this;
                mgr.add_endRequest(function () { self.start(); });
                this.wired = true;
            }
        },

        start: function () {
            this.clearTimers();

            var totalMs = this.cfg.timeoutMinutes * 60 * 1000;
            var warningMs = this.cfg.warningMinutes * 60 * 1000;
            var self = this;

            this.expiryTime = Date.now() + totalMs;

            this.warningTimer = setTimeout(function () { self.showWarning(); }, totalMs - warningMs);
            this.expiryTimer = setTimeout(function () { self.expire(); }, totalMs);
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
                '        <p>You will be signed out in <strong><span id="sessionCountdown">5:00</span></strong> due to inactivity.</p>' +
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
            document.getElementById('sessionSignOutBtn').addEventListener('click', function () { self.signOut(); });
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

        expire: function () {
            // Countdown ran out with no response from the user. Ping DNN's
            // logoff handler same-origin so the auth cookie actually gets
            // cleared server-side, then land on the home page regardless of
            // whatever page the portal's logoff control would otherwise
            // redirect to.
            this.clearTimers();
            var self = this;
            fetch(this.cfg.logoffUrl, { credentials: 'same-origin', cache: 'no-store' })
                .catch(function (err) { try { console.warn('SessionMonitor logoff ping failed:', err); } catch (e) { } })
                .then(function () { window.location.href = self.cfg.homeUrl; });
        },

        signOut: function () {
            // Sign-out via DNN's /ctl/Logoff handler — clears the auth cookie
            // server-side and DNN sends the user to the portal's configured
            // logoff page. Used only for the explicit "Sign out" button.
            this.clearTimers();
            window.location.href = this.cfg.logoffUrl;
        }
    };

    window.SessionMonitor = SessionMonitor;
})();
