/*
 * session-monitor.js
 *
 * Pops a Bootstrap 5 modal at (timeoutMinutes - warningMinutes) into the
 * forms-auth window warning the user their session is about to expire.
 * "Stay signed in" pings the server (refreshing the sliding-expiration cookie)
 * and restarts the timer. If the countdown runs out, redirects to the DNN
 * login page with returnurl preserved.
 *
 * Usage (emitted from CourtCounselModuleBase / JudicialReferralModuleBase):
 *   SessionMonitor.init({
 *     timeoutMinutes: 90,
 *     warningMinutes: 5,
 *     loginUrl: '/Login?ReturnUrl=...',
 *     keepAliveUrl: '/'
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
            this.expiryTimer = setTimeout(function () { self.signOut(); }, totalMs);
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
            // Hit a same-origin URL with credentials — the browser sends the
            // auth cookie automatically, the server's sliding-expiration logic
            // issues a fresh Set-Cookie on the response, and the user stays on
            // this page with any unsaved form state intact. No DOM mutation,
            // no navigation, no reload.
            var self = this;
            var url = (this.cfg && this.cfg.keepAliveUrl) || '/';
            // Bust caches so the request always reaches the server (and thus
            // the auth ticket gets refreshed).
            var cacheBuster = (url.indexOf('?') >= 0 ? '&' : '?') + '_k=' + Date.now();
            // Note: use default redirect handling (follow). DNN portals often
            // redirect '/' to a friendlier URL; with redirect:'manual' that
            // shows up as an opaque response and we'd have no way to tell
            // whether auth survived.
            fetch(url + cacheBuster, {
                credentials: 'same-origin',
                cache: 'no-store',
                method: 'GET'
            })
            .then(function (resp) {
                // If the request ended up on a login page or returned 401,
                // the session is actually gone — sign the user out via DNN's
                // logoff handler instead of pretending we extended it.
                var finalUrl = (resp.url || '').toLowerCase();
                if (resp.status === 401 || finalUrl.indexOf('login') >= 0) {
                    self.signOut();
                    return;
                }
                if (self.modal) { self.modal.hide(); }
                if (self.countdownTimer) { clearInterval(self.countdownTimer); self.countdownTimer = null; }
                self.start();
            })
            .catch(function (err) {
                // Network error — surface it so a silent dead button doesn't
                // mystify the user, and keep the modal up.
                try { console.warn('SessionMonitor keepAlive failed:', err); } catch (e) { }
            });
        },

        signOut: function () {
            // Sign-out via DNN's /ctl/Logoff handler — clears the auth cookie
            // server-side and DNN sends the user to the portal's configured
            // logoff page (the site home page in a logged-out state by default).
            // Used for both the Sign Out button and the countdown-expired
            // auto-action, so users always land at the same place.
            this.clearTimers();
            window.location.href = this.cfg.logoffUrl;
        }
    };

    window.SessionMonitor = SessionMonitor;
})();
