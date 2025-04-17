$(document).ready(function () {
    // Sidebar toggle
    const $userSidebar = $('#userSidebar');

    $('#openUserSidebarBtn').on('click', function () {
        $userSidebar.addClass('show-user-sidebar');
    });

    $('#closeUserSidebarBtn').on('click', function () {
        $userSidebar.removeClass('show-user-sidebar');
    });

    // Refresh button animation
    $('.refresh-btn').on('click', function () {
        const $icon = $(this).find('i');
        $icon.addClass('fa-spin');
        setTimeout(function () {
            $icon.removeClass('fa-spin');
        }, 1000);
    });
});