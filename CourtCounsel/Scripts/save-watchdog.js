/*
 * save-watchdog.js
 *
 * Users reported Save sticking on "Saving…" forever with no error in the
 * console. The Data Entry save is a full postback from a button rendered
 * with UseSubmitBehavior="false", so the browser runs one inline handler:
 *
 *     <OnClientClick>; __doPostBack('cmdSave','')
 *
 * OnClientClick validates, disables the button and relabels it "Saving…".
 * __doPostBack then submits the form ONLY if theForm.onsubmit() doesn't
 * return false — and ASP.NET's onsubmit re-runs validation. So a validator
 * that passed at click time but fails at submit time cancels the post
 * silently: no exception, no request, and a button stuck as disabled.
 * Nothing on the page ever tells the user.
 *
 * This watches the save button and, if the page is still sitting here
 * after timeoutSeconds, reports which stage didn't complete and re-enables
 * the button so the save can be retried.
 *
 * Usage:
 *   SaveWatchdog.watch({ buttonId: '<%= cmdSave.ClientID %>', timeoutSeconds: 10 });
 */
(function () {
    var SaveWatchdog = {
        state: null,
        hooked: false,

        watch: function (opts) {
            var button = document.getElementById(opts.buttonId);
            if (!button) return;

            var seconds = opts.timeoutSeconds || 10;
            var self = this;

            this.hookPostBack();

            button.addEventListener('click', function () {
                // The inline onclick has already run by now: it validated,
                // disabled the button, and called __doPostBack. A still-
                // enabled button means validation rejected the click, so
                // there's no save in flight to watch.
                if (!button.disabled) return;

                self.state.startedAt = Date.now();
                self.state.timer = setTimeout(function () {
                    self.report(button, seconds);
                }, seconds * 1000);
            });
        },

        // Record how far each save attempt actually gets. __doPostBack and
        // theForm.submit are wrapped rather than observed, because calling
        // form.submit() from script does NOT raise a submit event, so
        // there's no way to listen for it.
        hookPostBack: function () {
            if (this.hooked) return;
            this.hooked = true;

            var state = this.state = {
                postBackCalled: false,
                onSubmitResult: null,
                submitted: false,
                navigating: false,
                scriptError: null,
                startedAt: 0,
                timer: null
            };

            if (typeof window.__doPostBack === 'function') {
                var originalDoPostBack = window.__doPostBack;
                window.__doPostBack = function (target, argument) {
                    state.postBackCalled = true;
                    return originalDoPostBack.apply(this, arguments);
                };
            }

            var form = document.forms[0];
            if (form) {
                if (typeof form.onsubmit === 'function') {
                    var originalOnSubmit = form.onsubmit;
                    form.onsubmit = function () {
                        var result = originalOnSubmit.apply(this, arguments);
                        state.onSubmitResult = result;
                        return result;
                    };
                }
                var originalSubmit = form.submit;
                form.submit = function () {
                    state.submitted = true;
                    return originalSubmit.apply(this, arguments);
                };
            }

            // The page unloading means the postback is on its way and the
            // response is replacing this page — nothing to warn about.
            window.addEventListener('pagehide', function () { state.navigating = true; });
            window.addEventListener('beforeunload', function () { state.navigating = true; });

            window.addEventListener('error', function (e) {
                state.scriptError = (e && e.message) ? e.message : 'script error';
            });
        },

        // Which validators are currently unhappy. These are what silently
        // cancel the submit, so naming them turns "it just spins" into
        // something the user can act on.
        invalidValidators: function () {
            var messages = [];
            if (typeof window.Page_Validators === 'undefined') return messages;
            for (var i = 0; i < Page_Validators.length; i++) {
                var v = Page_Validators[i];
                if (v && v.isvalid === false) {
                    messages.push(v.errormessage || v.innerText || v.id || 'unnamed validator');
                }
            }
            return messages;
        },

        diagnose: function (seconds) {
            var state = this.state;

            if (state.navigating) {
                return 'The save was sent and the page started to navigate, but it has not finished after ' +
                    seconds + ' seconds. The server may still be working or the connection may have dropped.';
            }
            if (!state.postBackCalled) {
                return 'The save was never submitted: the page did not call __doPostBack. ' +
                    'The ASP.NET postback script may not have loaded on this page.';
            }
            if (state.onSubmitResult === false) {
                return 'The save was cancelled before it reached the server. Page validation ran again ' +
                    'as the form was submitting and rejected it, so no request was sent.';
            }
            if (!state.submitted) {
                return 'The save was never submitted: __doPostBack ran but the form submit did not. ' +
                    'Something cancelled it before the request was created.';
            }
            return 'The save request reached the server but there has been no response for ' +
                seconds + ' seconds. The server is still processing it or the request was lost.';
        },

        report: function (button, seconds) {
            var state = this.state;

            var lines = [this.diagnose(seconds)];

            var invalid = this.invalidValidators();
            if (invalid.length) {
                lines.push('Fields rejected by validation: ' + invalid.join('; ') + '.');
            } else if (state.onSubmitResult === false) {
                lines.push('No specific field reported an error, so the rejection came from a ' +
                    'validator that has no message or from custom submit handling.');
            }

            if (state.scriptError) {
                lines.push('A script error also occurred: ' + state.scriptError);
            }

            this.show(button, lines);

            // Let the user try again rather than leaving a dead button.
            button.disabled = false;
            if (button.value === 'Saving…') button.value = 'Save';
        },

        show: function (button, lines) {
            var existing = document.getElementById('saveWatchdogAlert');
            if (existing) existing.parentNode.removeChild(existing);

            var alert = document.createElement('div');
            alert.id = 'saveWatchdogAlert';
            alert.className = 'alert alert-warning mt-2';

            var heading = document.createElement('strong');
            heading.textContent = 'The save did not complete.';
            alert.appendChild(heading);

            for (var i = 0; i < lines.length; i++) {
                var p = document.createElement('p');
                p.className = 'mb-0';
                p.textContent = lines[i];
                alert.appendChild(p);
            }

            var container = button.parentNode;
            container.insertBefore(alert, button.nextSibling);

            try { console.warn('SaveWatchdog:', lines.join(' ')); } catch (e) { }
        }
    };

    window.SaveWatchdog = SaveWatchdog;
})();
