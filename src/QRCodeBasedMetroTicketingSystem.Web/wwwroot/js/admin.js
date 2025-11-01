document.addEventListener("DOMContentLoaded", function () {
    // Toggle Sidebar
    const sidebar = document.querySelector('.sidebar');
    const sidebarMenu = document.querySelector('.sidebar-menu-icon');
    const closeSidebarIcon = document.querySelector('.close-sidebar-icon');
    const mainContainer = document.querySelector('.main-container');

    function toggleSidebar() {
        if (window.innerWidth >= 768) {
            sidebar.classList.toggle('collapsed');
            mainContainer.classList.toggle('expanded');
        } else {
            sidebar.classList.toggle('show-sidebar');
        }
    }

    function handleResize() {
        if (window.innerWidth >= 768) { // Ensure sidebar is not in "show-sidebar" mode on large screens
            sidebar.classList.remove('show-sidebar');
        } else { // Ensure sidebar is not in "collapsed" mode on small screens
            sidebar.classList.remove('collapsed');
            mainContainer.classList.remove('expanded');
        }
    }

    sidebarMenu.addEventListener('click', toggleSidebar);
    closeSidebarIcon?.addEventListener('click', () => {
        sidebar.classList.remove('show-sidebar');
    });

    window.addEventListener('resize', handleResize);
    handleResize(); // Initialize sidebar state based on screen size

    // Header dropdown toggle
    const dropdownToggle = document.querySelector('.dropdown-toggle');
    const dropdownMenu = document.querySelector('.dropdown-menu');

    dropdownToggle.addEventListener('click', (e) => {
        e.stopPropagation();
        dropdownMenu.classList.toggle('show-dropdown-menu');
    });

    // Close dropdown when clicking outside
    document.addEventListener('click', (e) => {
        if (!dropdownMenu.contains(e.target) && !dropdownToggle.contains(e.target)) {
            dropdownMenu.classList.remove('show-dropdown-menu');
        }
    });

    // Enter and exit fullscreen
    const fullscreenToggle = document.getElementById("fullscreen");
    const enterFullscreenIcon = document.getElementById("enter-fullscreen");
    const exitFullscreenIcon = document.getElementById("exit-fullscreen");

    fullscreenToggle.addEventListener("click", function () {
        if (!document.fullscreenElement) {
            document.documentElement.requestFullscreen();
        } else {
            document.exitFullscreen();
        }
    });

    // Update Icon: Listen for fullscreen changes (Esc key, system UI, etc.)
    document.addEventListener("fullscreenchange", function () {
        if (document.fullscreenElement) {
            enterFullscreenIcon.style.display = "none";
            exitFullscreenIcon.style.display = "inline";
        } else {
            enterFullscreenIcon.style.display = "inline";
            exitFullscreenIcon.style.display = "none";
        }
    });

    // Theme switcher functionality
    const themes = ['dark-slate', 'dark-blue', 'charcoal-black', 'midnight-navy', 'deep-navy', 'white', 'midnight-blue', 'dark-gray'];
    const defaultSidebarTheme = themes[0];
    const defaultHeaderTheme = themes[5];

    // Fetch stored themes or set defaults
    let storedSidebarTheme = localStorage.getItem('sidebarTheme') || defaultSidebarTheme;
    let storedHeaderTheme = localStorage.getItem('headerTheme') || defaultHeaderTheme;

    // Validate themes
    storedSidebarTheme = themes.includes(storedSidebarTheme) ? storedSidebarTheme : defaultSidebarTheme;
    storedHeaderTheme = themes.includes(storedHeaderTheme) ? storedHeaderTheme : defaultHeaderTheme;

    // Theme styles configuration
    const themeStyles = {
        "dark-slate": { bg: "#212631", icon: "#ffffff61", text: "#ffffffde", linkText: "#ffffffde", linkHoverBg: "#2a303d", linkHoverText: "#fff", headerHoverText: "#fff", border: "#323a49" },
        "dark-blue": { bg: "#153657", icon: "#ffffffa3", text: "#ffffffde", linkText: "#ffffffde", linkHoverBg: "#2d5f91", linkHoverText: "#fff", headerHoverText: "#fff", border: "#12293f" },
        "charcoal-black": { bg: "#212529", icon: "#ffffff80", text: "#ffffffde", linkText: "#ffffffde", linkHoverBg: "#343a40", linkHoverText: "#fff", headerHoverText: "#fff", border: "#323940" },
        "midnight-navy": { bg: "#121D3E", icon: "#ffffffa3", text: "#ffffffde", linkText: "#c0c0c1", linkHoverBg: "#2E4071", linkHoverText: "#fff", headerHoverText: "#fff", border: "#2a3d76" },
        "deep-navy": { bg: "#1f283e", icon: "#ffffffa3", text: "#ffffffde", linkText: "#c0c0c1", linkHoverBg: "#283555", linkHoverText: "#fff", headerHoverText: "#fff", border: "#273455" },
        "white": { bg: "#ffffff", icon: "#314252", text: "#293846", linkText: "#293846", linkHoverBg: "#c8c8c833", linkHoverText: "#293846", headerHoverText: "#293846", border: "#dadada" },
        "midnight-blue": { bg: "#1a2035", icon: "#ffffffa3", text: "#ffffffde", linkText: "#c0c0c1", linkHoverBg: "#232b47", linkHoverText: "#fff", headerHoverText: "#fff", border: "#252d4c" },
        "dark-gray": { bg: "#343a40", icon: "#ffffffa3", text: "#c2c7d0", linkText: "#c2c7d0", linkHoverBg: "#ffffff1a", linkHoverText: "#fff", headerHoverText: "#fff", border: "#495057" },
    };

    // Applies the selected theme to the given section ("sidebar" or "header")
    function applyTheme(section, themeName) {
        if (!themeStyles[themeName]) return;

        // Update theme styles
        const styles = themeStyles[themeName];
        Object.entries(styles).forEach(([property, value]) => {
            document.documentElement.style.setProperty(`--${section}-${property}Color`, value);
        });

        // Update UI selection state
        const capitalizedSection = section.charAt(0).toUpperCase() + section.slice(1);
        document.querySelector(`.selected-${section}-theme`)?.classList.remove(`selected-${section}-theme`);
        document.querySelector(`.change${capitalizedSection}Theme[data-theme="${themeName}"]`)?.classList.add(`selected-${section}-theme`);
    }

    // Handles theme change events for sidebar and header.
    function handleThemeChange(event) {
        const button = event.target.closest('.changeSidebarTheme, .changeHeaderTheme');
        if (!button) return;

        const section = button.classList.contains('changeSidebarTheme') ? 'sidebar' : 'header';
        const theme = button.getAttribute('data-theme');
        if (!themes.includes(theme)) return;

        applyTheme(section, theme);
        localStorage.setItem(`${section}Theme`, theme);
    }

    // Apply stored themes on load
    applyTheme("sidebar", storedSidebarTheme);
    applyTheme("header", storedHeaderTheme);

    // Applies theme on click
    document.addEventListener('click', handleThemeChange);
});