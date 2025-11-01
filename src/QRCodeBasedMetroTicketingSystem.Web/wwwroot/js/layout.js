$(document).ready(function () {
    // Makes the dropdown menu accessible via the keyboard
    const $dropdownElement = $('#profileDropdown');

    if ($dropdownElement.length) {
        const dropdown = new bootstrap.Dropdown($dropdownElement[0]);

        $dropdownElement.on('keydown', function (event) {
            if (event.key === 'Enter' || event.key === ' ') {
                event.preventDefault();
                dropdown.toggle();
            }
        });
    }

    // Show open sidebar icon when sidebar available
    if ($('#userSidebar').length === 0) {
        $('#openUserSidebarBtn').hide();
    }

    // Show the balance at the header
    let balanceTimeout = null; // Track the timeout

    $("#balanceBtn").click(function () {
        const $balanceContainer = $(".balance-container");
        const $balanceText = $("#balanceText");
        const originalText = "Tap for Balance";

        // Prevent rapid clicks while blinking
        if ($balanceContainer.hasClass("blink")) return;

        // Clear any previous timeout to avoid overlap
        if (balanceTimeout) {
            clearTimeout(balanceTimeout);
            balanceTimeout = null;
        }

        // Start blink animation
        $balanceContainer.addClass("blink");

        $.ajax({
            url: '/User/Wallet/GetBalance',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                $balanceContainer.removeClass("blink");
                const finalBalance = parseFloat(response.balance);
                $balanceText.text(finalBalance.toFixed(2));

                // Set timeout to reset the text
                balanceTimeout = setTimeout(function () {
                    $balanceText.text(originalText);
                    balanceTimeout = null;
                }, 3000);
            },
            error: function (xhr, status, error) {
                $balanceContainer.removeClass("blink");
                $balanceText.text("Error loading balance");
                console.error("Error fetching balance:", error);

                // Set timeout to reset the text
                balanceTimeout = setTimeout(function () {
                    $balanceText.text(originalText);
                    balanceTimeout = null;
                }, 3000);
            }
        });
    });
});