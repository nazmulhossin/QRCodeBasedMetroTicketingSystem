
document.addEventListener('DOMContentLoaded', async function () {
    const video = document.getElementById('video');
    const scannerStatus = document.getElementById('scannerStatus');
    const statusCard = document.getElementById('statusCard');
    const statusTitle = document.getElementById('statusTitle');
    const statusMessage = document.getElementById('statusMessage');
    const ticketInfo = document.getElementById('ticketInfo');
    const statusIcon = document.getElementById('statusIcon');

    let scanning = false;
    let stream = null;
    const currentCamera = 'environment'; // Default to back camera

    // Start scanner automatically
    try {
        stream = await navigator.mediaDevices.getUserMedia({
            video: {
                facingMode: currentCamera,
                width: { ideal: 1280 },
                height: { ideal: 720 }
            }
        });
        video.srcObject = stream;
        await video.play();
        scanning = true;
        scannerStatus.className = 'alert alert-success mt-3 w-100';
        scannerStatus.textContent = 'Scanner active - position QR code in view';
        statusCard.className = 'status-card status-scanning';
        statusTitle.textContent = 'Scanner Active';
        statusMessage.textContent = 'Ready to scan QR codes';
        startScan();
    } catch (err) {
        scannerStatus.className = 'alert alert-danger mt-3 w-100';
        scannerStatus.textContent = 'Error accessing camera: ' + err.message;
        statusCard.className = 'status-card status-error';
        statusTitle.textContent = 'Camera Error';
        statusMessage.textContent = 'Unable to access camera: ' + err.message;
    }

    function startScan() {
        if (!scanning) return;

        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d');

        function tick() {
            if (video.readyState === video.HAVE_ENOUGH_DATA) {
                canvas.height = video.videoHeight;
                canvas.width = video.videoWidth;
                context.drawImage(video, 0, 0, canvas.width, canvas.height);
                const imageData = context.getImageData(0, 0, canvas.width, canvas.height);

                try {
                    const code = jsQR(imageData.data, imageData.width, imageData.height, {
                        inversionAttempts: "dontInvert",
                    });

                    if (code) {
                        // QR code detected
                        scannerStatus.className = 'alert alert-warning mt-3 w-100';
                        scannerStatus.textContent = 'QR code detected! Processing...';
                        statusCard.className = 'status-card status-scanning';
                        statusTitle.textContent = 'Processing QR Code';
                        statusMessage.textContent = 'Please wait while we validate the ticket...';

                        // Flash effect for the scan area
                        const scanArea = document.querySelector('.scan-area');
                        scanArea.style.boxShadow = '0 0 0 100px rgba(255, 255, 255, 0.5)';
                        setTimeout(() => {
                            scanArea.style.boxShadow = 'none';
                        }, 300);

                        // Process the QR code
                        processQRCode(code.data);

                        // Briefly pause scanning to prevent multiple scans
                        scanning = false;
                        setTimeout(() => {
                            if (stream) {
                                scanning = true;
                                requestAnimationFrame(tick);
                            }
                        }, 3000);
                        return;
                    }
                } catch (e) {
                    console.error("QR scanning error:", e);
                }
            }

            if (scanning) {
                requestAnimationFrame(tick);
            }
        }

        tick();
    }

    async function processQRCode(token) {
        try {
            const response = await fetch('/ticket/scan', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Token: token })
            });

            const result = await response.json();
            const timestamp = new Date().toLocaleTimeString();

            if (result.isValid) {
                // Success case
                statusCard.className = 'status-card status-success';
                statusTitle.textContent = 'Valid Ticket';
                statusMessage.textContent = result.Message;

                // Add tick icon
                statusIcon.className = 'fas fa-check-circle text-success';

                // Show ticket info if available
                if (result.Ticket) {
                    showTicketInfo(result.Ticket, timestamp);
                }

            } else {
                // Error case
                statusCard.className = 'status-card status-error';
                statusTitle.textContent = 'Invalid Ticket';
                statusMessage.textContent = result.Message;

                // Add cross icon
                statusIcon.className = 'fas fa-times-circle text-danger';
                ticketInfo.style.display = 'none';
            }
        } catch (err) {
            // Exception handling
            statusCard.className = 'status-card status-error';
            statusTitle.textContent = 'System Error';
            statusMessage.textContent = 'Error processing QR code: ' + err.message;
        }
    }

    function showTicketInfo(ticket, timestamp) {
        document.getElementById('ticketId').textContent = ticket.Id;
        document.getElementById('userId').textContent = ticket.UserId;
        document.getElementById('ticketStatus').textContent = ticket.Status;
        document.getElementById('scanTime').textContent = timestamp;
        ticketInfo.style.display = 'block';
    }

    // Stop scanner when page is unloaded
    window.addEventListener('beforeunload', () => {
        if (stream) {
            stream.getTracks().forEach(track => track.stop());
        }
    });
});
