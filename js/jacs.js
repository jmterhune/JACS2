$(function () {
    // Function to initialize collapse state from localStorage for menus
    function initializeCollapseState(menuId, storageKey, isSidebar = false) {
        const $menu = $(`#${menuId}`);
        const $toggle = isSidebar
            ? $('#btnToggleMenu')
            : $(`[data-bs-toggle="collapse"][href="#${menuId}"], [data-bs-toggle="collapse"][data-bs-target="#${menuId}"]`);
        const savedState = getFromLocalStorage(storageKey);

        // Restore state from localStorage, default to expanded for sidebar if no state
        const isExpanded = savedState === 'expanded' || (isSidebar && savedState === null);
        if (isExpanded) {
            $menu.addClass('show');
            $toggle.attr('aria-expanded', 'true');
            if (!isSidebar) $toggle.removeClass('collapsed');
        } else {
            $menu.removeClass('show');
            $toggle.attr('aria-expanded', 'false');
            if (!isSidebar) $toggle.addClass('collapsed');
        }
        if (storageKey != 'sidebarState') { 
        // Save state to localStorage and update toggle class on collapse toggle (only for submenus)
            $menu.on('shown.bs.collapse', () => {
                saveToLocalStorage(storageKey, 'expanded');
                $toggle.attr('aria-expanded', 'true').removeClass('collapsed');
            });

            $menu.on('hidden.bs.collapse', () => {
                saveToLocalStorage(storageKey, 'collapsed');
                $toggle.attr('aria-expanded', 'false').addClass('collapsed');
            });
        }
    }

    // Initialize sidebar state (default to expanded if no state saved)
    initializeCollapseState('sidebarMenu', 'sidebarState', true);

    // Initialize submenu states
    initializeCollapseState('authMenu', 'authMenuState');
    initializeCollapseState('jacsMenu', 'jacsMenuState');
    $('#sidebarMenu').on('shown.bs.collapse', function () {
        saveToLocalStorage('sidebarState', 'expanded');
    });

    $('#sidebarMenu').on('hidden.bs.collapse', function () {
        saveToLocalStorage('sidebarState', 'collapsed');
    });
});

function setActiveLink(linkId) {
    // Remove active class from all nav links
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
    });
    // Add active class to the specified link
    const targetLink = document.getElementById(linkId);
    if (targetLink) {
        targetLink.classList.add('active');
    }
}

function ShowAlert(title, text) {
    $.dnnAlert({
        okText: 'OK',
        title: title,
        text: text
    });
}

function ShowNotification(title, message, type) {
    new Noty({
        type: type.toLowerCase(),
        text: `<strong>${title}</strong><br>${message}`,
        timeout: 5000,
        theme: "bootstrap-v4",
        layout: "topRight",
        progressBar: true
    }).show();
}

// Saving to local storage
function saveToLocalStorage(key, value) {
    try {
        // Convert value to JSON string if it's an object
        const serializedValue = JSON.stringify(value);
        localStorage.setItem(key, serializedValue);
    } catch (error) {
        ShowNotification('Error saving to local storage:', error,'error');
    }
}

// Retrieving from local storage
function getFromLocalStorage(key) {
    try {
        const serializedValue = localStorage.getItem(key);
        if (serializedValue === null) {
            return null;
        }
        // Parse JSON string back to original format
        return JSON.parse(serializedValue);
    } catch (error) {
        ShowNotification('Error retrieving from local storage:', error,'error');
        return null;
    }
}

function getValueFromUrl(param) {
    try {
        const urlObj = new URL(window.location.href);
        const pathSegments = urlObj.pathname.split('/');
        const paramIndex = pathSegments.indexOf(param);
        if (paramIndex !== -1 && paramIndex + 1 < pathSegments.length) {
            return pathSegments[paramIndex + 1];
        }
        return null;
    } catch (e) {
        console.error('Error Retrieving URL parameter:', e);
        return null;
    }
}

function getQueryStringParam(key) {
    const params = new URLSearchParams(window.location.search);
    return params.get(key);
}
// ---------------------------------------------------------------------------
// Date/time display
//
// Court business runs on Eastern time, so every timestamp is shown in EST/EDT
// no matter how the viewer's machine is configured.
//
// Two kinds of value arrive from the server and they must be handled
// differently:
//
//   Zone-bearing ("2026-09-16T18:29:44Z", "...-04:00") is a real instant, e.g.
//   a clerk token expiration. It is converted into Eastern.
//
//   Zone-less ("2026-09-16 13:29:44") is ALREADY an Eastern wall-clock reading
//   — a hearing time, a log stamp written by the server. Its digits are printed
//   as-is. Running such a value through a timeZone conversion would shift it by
//   the viewer's own offset and show the wrong hearing time outside Eastern.
// ---------------------------------------------------------------------------
const JACS_TIME_ZONE = 'America/New_York';

function jacsHasExplicitTimeZone(value) {
    return typeof value === 'string' && /(?:Z|[+-]\d{2}:?\d{2})\s*$/i.test(value.trim());
}

// "EST" or "EDT" for the given instant, so a zone-less value can still be labelled.
function jacsEasternAbbreviation(date) {
    try {
        const part = new Intl.DateTimeFormat('en-US', {
            timeZone: JACS_TIME_ZONE,
            timeZoneName: 'short'
        }).formatToParts(date).find(p => p.type === 'timeZoneName');
        return part ? part.value : '';
    } catch (e) {
        return '';
    }
}

function formatEasternDateTime(value, options) {
    if (!value && value !== 0) return '';

    const parsed = value instanceof Date ? value : new Date(value);
    if (isNaN(parsed.getTime())) return value;

    const opts = Object.assign({
        year: 'numeric', month: 'numeric', day: 'numeric',
        hour: 'numeric', minute: '2-digit', second: '2-digit'
    }, options || {});

    // A Date object and a zone-bearing string both identify a real instant.
    if (value instanceof Date || jacsHasExplicitTimeZone(value)) {
        try {
            return parsed.toLocaleString('en-US',
                Object.assign({ timeZone: JACS_TIME_ZONE, timeZoneName: 'short' }, opts));
        } catch (e) {
            return parsed.toLocaleString();   // no IANA zone support
        }
    }

    // Zone-less: print the literal reading and name the zone it is already in.
    const abbr = jacsEasternAbbreviation(parsed);
    return parsed.toLocaleString('en-US', opts) + (abbr ? ' ' + abbr : '');
}

function formatEasternDate(value) {
    if (!value) return '';

    // A zone-less date has no time to shift, so read the calendar parts directly
    // rather than letting the browser reinterpret midnight in its own zone.
    if (typeof value === 'string') {
        const ymd = value.trim().match(/^(\d{4})-(\d{2})-(\d{2})/);
        if (ymd && !jacsHasExplicitTimeZone(value)) {
            return `${parseInt(ymd[2], 10)}/${parseInt(ymd[3], 10)}/${ymd[1]}`;
        }
    }

    const parsed = value instanceof Date ? value : new Date(value);
    if (isNaN(parsed.getTime())) return value;

    try {
        return parsed.toLocaleDateString('en-US', { timeZone: JACS_TIME_ZONE });
    } catch (e) {
        return parsed.toLocaleDateString();
    }
}
