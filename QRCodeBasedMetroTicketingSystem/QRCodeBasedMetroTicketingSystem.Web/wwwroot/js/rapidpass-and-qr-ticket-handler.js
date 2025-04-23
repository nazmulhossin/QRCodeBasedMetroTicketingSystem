$(document).ready(function () {
    const qrModal = new bootstrap.Modal(document.getElementById('generateQRModal'));

    $('.show-qr-ticket-btn').click(function () {
        var btn = $(this);
        var ticketId = $(this).data('ticket-id');
        qrModal.hide();

        // Save original button text and show loading
        var originalText = btn.html();
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Showing...');
        btn.prop('disabled', true);

        $.ajax({
            url: '/User/Ticket/GetQRCode',
            type: 'GET',
            data: { ticketId: ticketId },
            success: function (data) {
                $('#qrCodeImage').attr('src', data.qrCodeImage);
                $('#originStationName').text(data.originStationName);
                $('#destinationStationName').text(data.destinationStationName);
                $('#expiryInfo').text('This QR code will expire on ' + data.expiryTime + '.');
                $('#downloadQRBtn').attr('href', 'User/Ticket/DownloadQR?ticketId=' + ticketId);
                qrModal.show();

                // Restore button text
                btn.html(originalText);
                btn.prop('disabled', false);
            },
            error: function () {
                alert('Failed to load QR code. Please try again.');
                qrModal.hide();

                // Restore button text
                btn.html(originalText);
                btn.prop('disabled', false);
            }
        });
    });

    // Add event listener for the close button (refresh the page)
    $('#generateQRModal .btn-close').on('click', function () {
        location.reload();
    });
});