/* ExpertWitness admin — API + modal/notify helpers.
 *
 * window.ew:
 *   notifySuccess/notifyError/notifyInfo  — Noty toasts
 *   confirmDelete(text)                   — SweetAlert2 -> Promise<boolean>
 *   api.{get,post,put,del}                — fetch wrappers to the module Web API,
 *                                           adding the ModuleId / TabId /
 *                                           RequestVerificationToken headers a
 *                                           DnnApiController requires.
 *   showModal/hideModal                   — Bootstrap 5 modal
 *   esc(s)                                — HTML-escape for row templates
 *
 * Context comes from window.__ewCtx = { moduleId, tabId } emitted by each view,
 * plus the hidden __RequestVerificationToken input that
 * ServicesFramework.RequestAjaxAntiForgerySupport() writes to the page.
 */
(function (window, $) {
    "use strict";
    var ew = window.ew || {};

    /* ---------- notifications ---------- */
    function noty(type, text, timeout) {
        if (typeof window.Noty !== "function") {
            try { console.log("[ew " + type + "] " + text); } catch (e) {}
            return;
        }
        new window.Noty({
            type: type, text: text, theme: "bootstrap-v4",
            layout: "topRight", timeout: timeout == null ? 3500 : timeout, progressBar: true
        }).show();
    }
    ew.notifySuccess = function (t) { noty("success", t, 3000); };
    ew.notifyError = function (t) { noty("error", t, 6000); };
    ew.notifyInfo = function (t) { noty("info", t, 3500); };

    ew.confirmDelete = function (text) {
        if (!window.Swal || typeof window.Swal.fire !== "function") {
            return new Promise(function (resolve) { resolve(window.confirm(text || "Delete this item?")); });
        }
        return window.Swal.fire({
            title: "Are you sure?",
            text: text || "This cannot be undone.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, delete",
            cancelButtonText: "Cancel",
            confirmButtonColor: "#d33"
        }).then(function (r) { return !!r.isConfirmed; });
    };

    /* ---------- API wrapper ---------- */
    function ctx() {
        var c = window.__ewCtx || {};
        var token = document.querySelector('input[name="__RequestVerificationToken"]');
        return {
            moduleId: c.moduleId == null ? "" : String(c.moduleId),
            tabId: c.tabId == null ? "" : String(c.tabId),
            token: token ? token.value : ""
        };
    }

    function apiCall(method, urlSuffix, body) {
        var c = ctx();
        var opts = {
            method: method,
            credentials: "same-origin",
            headers: {
                "Accept": "application/json",
                "ModuleId": c.moduleId,
                "TabId": c.tabId,
                "RequestVerificationToken": c.token
            }
        };
        if (body !== undefined && body !== null) {
            opts.headers["Content-Type"] = "application/json";
            opts.body = JSON.stringify(body);
        }
        return fetch("/DesktopModules/ExpertWitness/API/" + urlSuffix, opts).then(function (resp) {
            if (resp.status === 204) return null;
            var ct = resp.headers.get("Content-Type") || "";
            var parse = ct.indexOf("application/json") >= 0 ? resp.json() : resp.text();
            if (!resp.ok) {
                return parse.then(function (payload) {
                    var msg = (payload && (payload.Message || payload.message || payload)) ||
                              (resp.status + " " + resp.statusText);
                    throw new Error(typeof msg === "string" ? msg : JSON.stringify(msg));
                });
            }
            return parse;
        });
    }
    ew.api = {
        get: function (u) { return apiCall("GET", u); },
        post: function (u, b) { return apiCall("POST", u, b); },
        put: function (u, b) { return apiCall("PUT", u, b); },
        del: function (u) { return apiCall("DELETE", u); }
    };

    /* ---------- modal helpers (Bootstrap 5 under Porto) ---------- */
    ew.showModal = function (id) {
        var el = document.getElementById(id);
        if (!el || !window.bootstrap) return null;
        var existing = window.bootstrap.Modal.getInstance(el);
        if (existing) { try { existing.dispose(); } catch (e) {} }
        var m = new window.bootstrap.Modal(el);
        m.show();
        return m;
    };
    ew.hideModal = function (id) {
        var el = document.getElementById(id);
        if (!el || !window.bootstrap) return;
        var inst = window.bootstrap.Modal.getInstance(el);
        if (inst) inst.hide();
        // Clean up any orphaned backdrop the Porto skin leaves behind.
        setTimeout(function () {
            document.querySelectorAll(".modal-backdrop").forEach(function (b) { b.remove(); });
            document.body.classList.remove("modal-open");
            document.body.style.paddingRight = "";
            document.body.style.overflow = "";
        }, 350);
    };

    ew.esc = function (s) {
        return (s == null ? "" : String(s))
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    };

    window.ew = ew;
})(window, jQuery);
