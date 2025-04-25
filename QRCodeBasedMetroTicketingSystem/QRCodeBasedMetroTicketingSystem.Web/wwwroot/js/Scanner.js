document.addEventListener('DOMContentLoaded', async function () {
    const video = document.getElementById('video');
    const scanInstruction = document.getElementById('scanInstruction');
    const statusCard = document.getElementById('statusCard');
    const statusIcon = document.getElementById('statusIcon');
    const statusTitle = document.getElementById('statusTitle');
    const journeyInfo = document.getElementById('journeyInfo');
    const stationId = document.querySelector('[data-station-id]').dataset.stationId;

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
        scanInstruction.className = 'text-muted text-center';
        scanInstruction.textContent = 'Position the QR code inside the box';
        statusCard.className = 'status-card status-scanning';
        statusIcon.className = "fa-solid fa-expand";
        statusTitle.textContent = 'Scanner Your QR Code';
        startScan();
    } catch (err) {
        statusCard.className = 'status-card status-error';
        statusTitle.textContent = 'Camera Error';
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
                        statusCard.className = 'status-card status-processing';
                        statusIcon.className = 'fa-solid fa-hourglass-start';
                        statusTitle.textContent = 'Processing...';

                        // Flash effect for the scan area
                        const scanArea = document.querySelector('.scan-area');
                        scanArea.style.boxShadow = '0 0 0 100px rgba(255, 255, 255, 0.5)';
                        setTimeout(() => {
                            scanArea.style.boxShadow = 'none';
                        }, 300);

                        // Process the QR code
                        processQRCode(stationId, code.data);

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

    async function processQRCode(stationId, qrCodeData) {
        try {
            const response = await fetch('/System/Scanner/ScanTicket', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ StationId: stationId, QRCodeData: qrCodeData })
            });

            const result = await response.json();

            if (result.isValid) {
                // Success case
                statusCard.className = 'status-card status-success';
                statusTitle.textContent = 'GO! GO!';

                // Add tick icon
                statusIcon.className = 'fa-solid fa-check text-success';

                // Show journey info if available
                if (result.Trip) {
                    showJourneyInfo(result.Trip);
                }

            } else {
                // Error case
                statusCard.className = 'status-card status-error';
                statusTitle.textContent = result.message;

                // Add cross icon
                statusIcon.className = 'fa-solid fa-xmark text-danger';
                journeyInfo.classList.remove('d-none');
            }

            // Reset scanner UI
            setTimeout(() => {
                statusCard.className = 'status-card status-scanning';
                statusIcon.className = 'fa-solid fa-expand';
                statusTitle.textContent = 'Scan Your QR Code';
            }, 1500);
        } catch (err) {
            // Exception handling
            statusCard.className = 'status-card status-error';
            statusTitle.textContent = 'System Error';
        }
    }

    function showJourneyInfo(trip) {
        document.getElementById('fromStationName').textContent = trip.EntryStationName;
        document.getElementById('fromTime').textContent = trip.EntryTime;
        document.getElementById('toStationName').textContent = trip.ExitStationName;
        document.getElementById('toTime').textContent = trip.ExitTime;
        document.getElementById('fareAmount').textContent = trip.FareAmount;
        journeyInfo.classList.remove('d-none')
    }

    // Stop scanner when page is unloaded
    window.addEventListener('beforeunload', () => {
        if (stream) {
            stream.getTracks().forEach(track => track.stop());
        }
    });
});
