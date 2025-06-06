$(function () {
    // Manage Sidebar state on page load
    const $sidebarToggle = $('[data-bs-target="#sidebarMenu"]');
    const $sidebarMenu = $('#sidebarMenu');
    const savedSideBarState = getFromLocalStorage('sidebarState');
    // Restore state of SideBar from localStorage on page load
    if (savedSideBarState === 'collapsed') {
        $sidebarMenu.removeClass('show');
        $sidebarToggle.attr('aria-expanded', 'false');
    } else {
        $sidebarMenu.addClass('show');
        $sidebarToggle.attr('aria-expanded', 'true');
    }
    // Save state of SideBar to localStorage on collapse toggle
    $sidebarMenu.on('shown.bs.collapse', () => {
        saveToLocalStorage('sidebarState', 'expanded');
        $sidebarToggle.attr('aria-expanded', 'true');
    });

    $sidebarMenu.on('hidden.bs.collapse', () => {
        saveToLocalStorage('sidebarState', 'collapsed');
        $sidebarToggle.attr('aria-expanded', 'false');
    });
   
    // Manage Jacs Menu state on page load
    const $jacsToggle = $('[data-bs-target="#jacsMenu"], [href="#jacsMenu"]');
    const $jacsMenu = $('#jacsMenu');
    const savedJacsMenuState = getFromLocalStorage('jacsMenuState');

    // Restore state of Jacs Menu from localStorage on page load
    if (savedJacsMenuState === 'collapsed') {
        $jacsMenu.removeClass('show');
        $jacsToggle.attr('aria-expanded', 'false');
    } else {
        $jacsMenu.addClass('show');
        $jacsToggle.attr('aria-expanded', 'true');
    }

    // Save state of Jacs Menu to localStorage on collapse toggle
    $jacsMenu.on('shown.bs.collapse', () => {
        saveToLocalStorage('jacsMenuState', 'expanded');
        $jacsToggle.attr('aria-expanded', 'true');
    });

    $jacsMenu.on('hidden.bs.collapse', () => {
        saveToLocalStorage('jacsMenuState', 'collapsed');
        $jacsToggle.attr('aria-expanded', 'false');
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
// Saving to local storage
function saveToLocalStorage(key, value) {
    try {
        // Convert value to JSON string if it's an object
        const serializedValue = JSON.stringify(value);
        localStorage.setItem(key, serializedValue);
    } catch (error) {
        ShowAlert('Error saving to local storage:', error);
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
        ShowAlert('Error retrieving from local storage:', error);
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
