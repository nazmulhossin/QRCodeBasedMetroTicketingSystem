$(document).ready(function () {
// Filter dropdown
    // Generic function for filtering related dropdown pairs
    function setupPairedDropdowns(selector1, selector2) {
        const $dropdowns = $(`${selector1}, ${selector2}`);

        function filterOppositeDropdown() {
            const $current = $(this);
            const selectedValue = $current.val();
            const $opposite = $current.is(selector1) ? $(selector2) : $(selector1);

            $opposite.find('option').each(function () {
                $(this).toggle(!(selectedValue && $(this).val() === selectedValue));
            });

            // Reset opposite dropdown if values match
            if ($opposite.val() === selectedValue && selectedValue !== "") {
                $opposite.val("");
            }
        }

        $dropdowns.on('change', filterOppositeDropdown);
    }

    // Initialize both pairs of dropdowns with the same logic
    setupPairedDropdowns('#fromStation', '#toStation');
    setupPairedDropdowns('#buyQrTicketFromStation', '#buyQrTicketToStation');

// Fare Calculation
    $('#fareForm').submit(function (e) {
        e.preventDefault();

        const fromStationId = $('#fromStation').val();
        const toStationId = $('#toStation').val();

        if (!fromStationId || !toStationId) {
            alert('Please select both departure and destination stations');
            return;
        }

        if (fromStationId === toStationId) {
            alert('Departure and destination stations cannot be the same');
            return;
        }

        // Show loading animation on button
        const submitBtn = $(this).find('button[type="submit"]');
        const originalButtonText = submitBtn.text();
        submitBtn.html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span> Calculating...');
        submitBtn.prop('disabled', true);
        submitBtn.addClass('disabled');

        $.ajax({
            url: '/GetFare',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ fromStationId: fromStationId, toStationId: toStationId }),
            success: function (response) {
                // Update the pre-defined fare result section
                $('#fareAmount').text(response.fare);
                $('#fromStationDisplay').text(response.fromStationName);
                $('#toStationDisplay').text(response.toStationName);
                $("#fareResult").show();

                // Reset button state
                submitBtn.html(originalButtonText);
                submitBtn.prop('disabled', false);
                submitBtn.removeClass('disabled');
            },
            error: function (xhr, status, error) {
                // Handle error
                let errorMessage = 'An error occurred while calculating the fare';

                if (xhr.responseJSON?.message) {
                    errorMessage = xhr.responseJSON.message;
                }

                // Create error alert
                const alertDiv = $(`
                    <div class="alert alert-danger alert-dismissible fade show mt-3" role="alert">
                        <strong>Error!</strong> ${errorMessage}
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </div>
                `);

                // Remove any existing alerts first
                $('.alert').remove();
                $('#fareForm').after(alertDiv);

                // Hide the fare result section in case of error
                $('#fareResult').hide();

                // Reset button state
                submitBtn.html(originalButtonText);
                submitBtn.prop('disabled', false);
                submitBtn.removeClass('disabled');
            }
        });
    });
});