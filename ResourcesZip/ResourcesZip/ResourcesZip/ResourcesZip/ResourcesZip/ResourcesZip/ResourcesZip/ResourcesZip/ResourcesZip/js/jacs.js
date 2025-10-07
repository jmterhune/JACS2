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