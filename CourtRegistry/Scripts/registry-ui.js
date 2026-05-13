/*
 * Shared UI helpers for the Court Registry module.
 * Lazy-loads SweetAlert2 and Noty from CDN once per page and exposes
 * Registry.confirmDelete() / Registry.notify().
 */
(function () {
    function loadCss(href) {
        if (document.querySelector('link[data-registry="' + href + '"]')) return;
        var link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = href;
        link.setAttribute('data-registry', href);
        document.head.appendChild(link);
    }
    function loadScript(src) {
        if (document.querySelector('script[data-registry="' + src + '"]')) return;
        var s = document.createElement('script');
        s.src = src;
        s.async = false;
        s.setAttribute('data-registry', src);
        document.head.appendChild(s);
    }

    loadCss('https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css');
    loadCss('https://cdn.jsdelivr.net/npm/noty@3.1.4/lib/noty.css');
    loadCss('https://cdn.jsdelivr.net/npm/noty@3.1.4/lib/themes/mint.css');
    loadScript('https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.js');
    loadScript('https://cdn.jsdelivr.net/npm/noty@3.1.4/lib/noty.min.js');

    var Registry = window.Registry || (window.Registry = {});

    function whenReady(globalName, cb) {
        if (typeof window[globalName] !== 'undefined') { cb(); return; }
        var tries = 0;
        var iv = setInterval(function () {
            if (typeof window[globalName] !== 'undefined') {
                clearInterval(iv);
                cb();
            } else if (++tries > 50) {
                clearInterval(iv);
            }
        }, 100);
    }

    /**
     * Use as OnClientClick on a delete LinkButton:
     *   OnClientClick="return Registry.confirmDelete(this,'Location');"
     * Always returns false to suppress the immediate postback; if the user
     * confirms, the postback target is extracted from the link's href and
     * fired manually.
     */
    Registry.confirmDelete = function (elem, label) {
        var href = elem.href || '';
        whenReady('Swal', function () {
            Swal.fire({
                title: 'Delete ' + (label || 'this record') + '?',
                text: 'This action cannot be undone.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, delete',
                cancelButtonText: 'Cancel',
                confirmButtonColor: '#d33'
            }).then(function (result) {
                if (result.isConfirmed) {
                    var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
                    if (m && typeof __doPostBack === 'function') __doPostBack(m[1], m[2]);
                }
            });
        });
        return false;
    };

    /**
     * Confirm an arbitrary action; invokes onConfirm on Yes.
     */
    Registry.confirm = function (opts, onConfirm) {
        whenReady('Swal', function () {
            Swal.fire({
                title: opts.title || 'Are you sure?',
                text: opts.text || '',
                icon: opts.icon || 'question',
                showCancelButton: true,
                confirmButtonText: opts.confirmText || 'Yes',
                cancelButtonText: opts.cancelText || 'Cancel',
                confirmButtonColor: opts.confirmColor || '#3085d6'
            }).then(function (result) {
                if (result.isConfirmed && typeof onConfirm === 'function') onConfirm();
            });
        });
    };

    /**
     * type: 'success' | 'info' | 'warning' | 'error'
     */
    Registry.notify = function (text, type) {
        whenReady('Noty', function () {
            new Noty({
                text: text,
                type: type || 'info',
                timeout: 3000,
                layout: 'topRight',
                theme: 'mint'
            }).show();
        });
    };
})();
