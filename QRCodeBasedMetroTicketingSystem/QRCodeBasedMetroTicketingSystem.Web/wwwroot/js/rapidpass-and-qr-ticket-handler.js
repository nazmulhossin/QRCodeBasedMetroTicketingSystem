$(document).ready(function () {
    const qrModal = new bootstrap.Modal(document.getElementById('generateQRModal'));

    // Change active RapidPass button color onload
    const $statusElement = $('#rapidPassStatus');

    if ($statusElement.length > 0) {
        const rapidPassStatusOnLoad = $statusElement.data('rapid-pass-status');

        if (rapidPassStatusOnLoad) {
            $('#rapidPassToggleBtn').addClass('active-rapid-pass-btn-color');
        }
    }

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
                startExpiryCountdown(new Date(data.expiryTime), data.ticketStatus);
                qrModal.show();

                // Restore button text (hide loadig animation)
                hideLoading(btn, originalText);
            },
            error: function (xhr) {
                if (xhr.status === 401) {
                    // Redirect to login if not authorized
                    window.location.href = '/Login';
                } else {
                    alert('Failed to load QR code. Please try again.');
                    qrModal.hide();
                    hideLoading(btn, originalText);
                }
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
        const rapidPassStatus = $("#rapidPassStatus").data('rapid-pass-status');
        qrModal.hide();

        // Save original button text and show loading
        const originalText = btn.html();
        if (rapidPassStatus == 0) {
            btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Generating QR Code...');
        } else {
            btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Showing QR Code...');
        }
        btn.prop('disabled', true);

        // Hide the Cancel RapidPass button if the RapidPass is in use
        if (rapidPassStatus == 2) {
            $('#cancelRapidPassBtn').addClass('d-none');
        } else {
            $('#cancelRapidPassBtn').removeClass('d-none');
        }

        $.ajax({
            url: '/User/Ticket/GetOrGenerateRapidPass',
            type: 'GET',
            success: function (data) {
                $('#qrCodeImage').attr('src', data.qrCodeImage);
                $('#downloadQRBtn').attr('data-ticket-id', data.rapidPassTicketId);
                $('#cancelRapidPassBtn').attr('data-ticket-id', data.rapidPassTicketId);
                $('#rapidPassStatus').data('rapid-pass-status', data.status);

                startExpiryCountdown(new Date(data.expiryTime), data.ticketStatus); // Start the countdown timer for expiry
                qrModal.show();
                updateRapidPassBtnText();
                hideLoading(btn, originalText); // Restore button text (hide loading animation)
            },
            error: function (xhr) {
                if (xhr.status === 401) {
                    // Redirect to login if not authorized
                    window.location.href = '/Login';
                } else {
                    alert('Failed to load RapidPass QR code. Please try again.');
                    qrModal.hide();
                    hideLoading(btn, originalText);
                }
            }
        });
    });

    // Cancel Rapid-Pass
    $('#cancelRapidPassBtn').click(function () {
        const btn = $(this);
        const rapidPassStatus = $("#rapidPassStatus").data('rapid-pass-status');

        // Save original button text and show loading
        const originalText = btn.html();
        if (rapidPassStatus == 2) {
            alert('RapidPass is in use. Cannot cancel.');
            return;
        }

        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Canceling...');
        btn.prop('disabled', true);

        $.ajax({
            url: '/User/Ticket/CancelRapidPass',
            type: 'GET',
            success: function (data) {
                if (data.isSuccess) {
                    $('#rapidPassStatus').data('rapid-pass-status', 0);
                    updateRapidPassBtnText();
                    $('#cancelRapidPassBtn').text(data.message);
                } else {
                    alert(data.message);
                }

                qrModal.hide();
                hideLoading(btn, originalText);
            },
            error: function (xhr) {
                if (xhr.status === 401) {
                    // Redirect to login if not authorized
                    window.location.href = '/Login';
                } else {
                    alert('Failed to cancel RapidPass. Please try again.');
                    qrModal.hide();
                    hideLoading(btn, originalText);
                }
            }
        });
    });

    // Close Rapid-Pass QR Code Modal
    $('#rapidPassModalCloseBtn').on('click', function () {
        // Clear count down interval
        if (window.countdownInterval) {
            clearInterval(window.countdownInterval);
        }

        // Get RapidPass Status from server
        $.ajax({
            url: '/User/Ticket/GetRapidPassStatus',
            type: 'GET',
            success: function (data) {
                $('#rapidPassStatus').data('rapid-pass-status', data.status);
                updateRapidPassBtnText();
                qrModal.hide();
            },
            error: function (xhr) {
                if (xhr.status === 401) {
                    // Redirect to login if not authorized
                    window.location.href = '/Login';
                } else {
                    qrModal.hide();
                }
            }
        });
    });

    // Function to start countdown timer
    function startExpiryCountdown(expiryDate, ticketStatus) {
        // Clear any existing interval
        if (window.countdownInterval) {
            clearInterval(window.countdownInterval);
        }

        function updateTimer() {
            const now = new Date();
            const timeDiff = expiryDate - now;

            if (timeDiff <= 0) {
                // Timer expired
                $('#expiryInfo').html('Trip duration limit exceeded');
                if (ticketStatus == 1) {
                    $('#expiryInfo').html('This QR code has <strong>expired</strong>');
                }

                clearInterval(window.countdownInterval);
                return;
            }

            // Calculate time units
            const hours = Math.floor(timeDiff / (1000 * 60 * 60));
            const minutes = Math.floor((timeDiff % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((timeDiff % (1000 * 60)) / 1000);

            // Format display text
            let timerText = 'Expires in ';
            if (ticketStatus == 2) {
                timerText = 'Scan at the exit gate within ';
            }

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

    function updateRapidPassBtnText() {
        const rapidPassStatus = $("#rapidPassStatus").data('rapid-pass-status');

        // Update Button text on status
        if (rapidPassStatus == 0) {
            $('#rapidPassStatus').text("Get RapidPass QR Code");
            $('#rapidPassToggleBtn').removeClass('active-rapid-pass-btn-color');
        } else {
            $('#rapidPassStatus').text("Show RapidPass QR Code");
            $('#rapidPassToggleBtn').addClass('active-rapid-pass-btn-color');
        }
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

        if (ticketData.ticketType == 0) {
            doc.text(`From: ${ticketData.originStationName}`, 10, 112);
            doc.text(`To: ${ticketData.destinationStationName}`, 10, 118);
        }

        doc.text(`Valid Until: ${convertToHumanReadableDate(ticketData.expiryTime)}`, 10, 124);

        // Save PDF
        if (ticketData.ticketType == 0) {
            doc.save(`Metro-Ticket-${ticketData.ticketId}_${ticketData.originStationName} to ${ticketData.destinationStationName}.pdf`);
        } else {
            doc.save(`Metro-RapidPass-${ticketData.ticketId}_${ticketData.expiryTime}.pdf`);
        }
    }

    // Helper function to hide loading animation
    function hideLoading(btn, originalText) {
        btn.html(originalText);
        btn.prop('disabled', false);
    }

    function convertToHumanReadableDate(isoString) {
        const date = new Date(isoString)
        return date.toLocaleString('en-US', {
            weekday: 'short', 
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: 'numeric',
            minute: '2-digit',
            hour12: true 
        });
    }
});