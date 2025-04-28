$(document).ready(function () {
    // Sidebar toggle
    const $userSidebar = $('#userSidebar');

    $('#openUserSidebarBtn').on('click', function () {
        $userSidebar.addClass('show-user-sidebar');
    });

    $('#closeUserSidebarBtn').on('click', function () {
        $userSidebar.removeClass('show-user-sidebar');
    });
});