// Shared EmployeeDB module helpers.
// Most view-specific JS lives inline in each .ascx. This file is for
// cross-view utilities that multiple pages need.

(function ($) {
    "use strict";

    // Force text uppercase on fields tagged .upperCase
    $(document).on("input change paste keyup", ".upperCase", function () {
        var $el = $(this);
        var v = $el.val() || "";
        var u = v.toUpperCase();
        if (v !== u) $el.val(u);
    });

    // On page load, upper-case any pre-populated .upperCase fields
    $(function () {
        $(".upperCase").each(function () {
            var $el = $(this);
            $el.val(($el.val() || "").toUpperCase());
        });
    });
})(jQuery);
