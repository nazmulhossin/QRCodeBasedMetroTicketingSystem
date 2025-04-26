$(document).ready(function () {
    const qrModal = new bootstrap.Modal(document.getElementById('generateQRModal'));

    // Show QR Ticket
    $('.show-qr-ticket-btn').click(function () {
        const btn = $(this);
        const ticketId = $(this).data('ticket-id');
        qrModal.hide();

        // Save original button text and show loading
        const originalText = btn.html();
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Showing...');
        btn.prop('disabled', true);

        $.ajax({
            url: '/User/Ticket/GetTicketDetailsWithQRCode',
            type: 'GET',
            data: { ticketId: ticketId },
            success: function (data) {
                $('#qrCodeImage').attr('src', data.qrCodeImage);
                $('#infoMessage').html(`Scan this QR code at the entry gate <span class="station-name">(${data.originStationName})</span> and the exit gate <span class="station-name">(${data.destinationStationName})</span>`);
                $('#originStationName').text(data.originStationName);
                $('#destinationStationName').text(data.destinationStationName);
                $('#downloadQRBtn').attr('data-ticket-id', data.ticketId);

                // Start the countdown timer for expiry
                startExpiryCountdown(new Date(data.expiryTime));
                qrModal.show();

                // Restore button text (hide loadig animation)
                hideLoading(btn, originalText);
            },
            error: function () {
                alert('Failed to load QR code. Please try again.');
                qrModal.hide();
                hideLoading(btn, originalText);
            }
        });
    });

    // Close QR Ticket Modal
    $('#generateQRModal .close-qr-modal-btn').on('click', function () {
        // Clear count down interval
        if (window.countdownInterval) {
            clearInterval(window.countdownInterval);
        }

        // refresh the page
        location.reload();
    });

    // Generate and show Rapid-Pass QR Code
    $('#rapidPassToggleBtn').click(function () {
        const btn = $(this);
        const ticketId = $("#rapidPassStatus").data('ticket-id');
        qrModal.hide();

        // Save original button text and show loading
        const originalText = btn.html();
        if (ticketId == 0) {
            btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Generating...');
            window.location.href = '/Login';
            return;
        } else {
            btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Showing...');
        }
        btn.prop('disabled', true);

        $.ajax({
            url: '/User/Ticket/GetOrGenerateRapidPassQr',
            type: 'GET',
            success: function (data) {
                $('#qrCodeImage').attr('src', data.qrCodeImage);
                $('#infoMessage').html('Scan this QR code at the entry gate and the exit gate');
                $('#downloadQRBtn').attr('data-ticket-id', data.ticketId);

                // Start the countdown timer for expiry
                startExpiryCountdown(new Date(data.expiryTime));
                qrModal.show();

                // Restore button text (hide loadig animation)
                hideLoading(btn, originalText);
            },
            error: function () {
                alert('Failed to load QR code. Please try again.');
                qrModal.hide();
                hideLoading(btn, originalText);
            }
        });
    });

    // Function to start countdown timer
    function startExpiryCountdown(expiryDate) {
        // Clear any existing interval
        if (window.countdownInterval) {
            clearInterval(window.countdownInterval);
        }

        function updateTimer() {
            const now = new Date();
            const timeDiff = expiryDate - now;

            if (timeDiff <= 0) {
                // Timer expired
                $('#expiryInfo').html('This QR code has <strong>expired</strong>');
                clearInterval(window.countdownInterval);
                return;
            }

            // Calculate time units
            const hours = Math.floor(timeDiff / (1000 * 60 * 60));
            const minutes = Math.floor((timeDiff % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((timeDiff % (1000 * 60)) / 1000);

            // Format display text
            let timerText = 'Expires in ';

            if (hours > 0) {
                timerText += `<strong>${hours}</strong> hour${hours !== 1 ? 's' : ''} `;
            }

            if (minutes > 0 || hours > 0) {
                timerText += `<strong>${minutes}</strong> minute${minutes !== 1 ? 's' : ''} `;
            }

            timerText += `and <strong>${seconds}</strong> second${seconds !== 1 ? 's' : ''}`;

            // Update the UI
            $('#expiryInfo').html(timerText);
        }

        // Initial call
        updateTimer();

        // Start countdown interval
        window.countdownInterval = setInterval(updateTimer, 1000);
    }

    // Download QR Ticket and RapidPass QR code as PDF (when clicking the download button on the ticket card or clicking the download button in the modal)
    $('.download-ticket').on('click', function () {
        const btn = $(this);
        const ticketId = $(this).data('ticket-id');

        // Save original button text and show loading
        const originalText = btn.html();
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading...');
        btn.prop('disabled', true);

        // Get ticket data
        $.ajax({
            url: '/User/Ticket/GetTicketDetailsWithQRCode',
            type: 'GET',
            data: { ticketId: ticketId },
            success: function (Data) {
                generateTicketPDF(Data); // Generate PDF with QR code
                hideLoading(btn, originalText);
            },
            error: function () {
                alert('Failed to download ticket. Please try again.');
                hideLoading(btn, originalText);
            }
        });
    });

    // Function to generate PDF with ticket details and QR code
    function generateTicketPDF(ticketData) {
        // Create a new jsPDF instance
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF('p', 'mm', [100, 135]); // Small ticket size

        // Add logo at the top
        // doc.addImage(logoBase64, 'PNG', 35, 10, 30, 30);

        // Add title
        doc.setFontSize(16);
        doc.setFont('helvetica', 'bold');
        doc.setTextColor('#006a4e');
        doc.text('Dhaka Metro Rail', 50, 15, { align: 'center' });

        // Add QR code to PDF
        doc.addImage(ticketData.qrCodeImage, 'PNG', 10, 20, 80, 80);

        // Add ticket information
        doc.setFontSize(11);
        doc.setFont('helvetica', 'normal');
        doc.setTextColor('#333333');
        doc.text(`From: ${ticketData.originStationName}`, 10, 112);
        doc.text(`To: ${ticketData.destinationStationName}`, 10, 118);
        doc.text(`Valid Until: ${ticketData.expiryTime}`, 10, 124);

        // Save PDF
        doc.save(`metro-ticket-${ticketData.ticketId}-${ticketData.originStationName} to ${ticketData.destinationStationName}.pdf`);
    }

    // Helper function to hide loading animation
    function hideLoading(btn, originalText) {
        btn.html(originalText);
        btn.prop('disabled', false);
    }
});