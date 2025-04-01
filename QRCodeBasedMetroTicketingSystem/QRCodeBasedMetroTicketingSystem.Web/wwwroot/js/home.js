// Fare Calculator
document.getElementById('fareForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const fromStation = document.getElementById('fromStation').value;
    const toStation = document.getElementById('toStation').value;

    // Simple fare calculation (in a real app, this would be more complex)
    let fare = 20; // Base fare

    // Calculate fare based on distance (simplified example)
    const stations = {
        'uttara_north': 1,
        'uttara_center': 2,
        'uttara_south': 3,
        'pallabi': 4,
        'mirpur_11': 5,
        'mirpur_10': 6,
        'kazipara': 7,
        'shewrapara': 8,
        'agargaon': 9,
        'bijoy_sarani': 10,
        'farmgate': 11,
        'karwan_bazar': 12,
        'shahbag': 13,
        'dhaka_university': 14,
        'bangladesh_secretariat': 15,
        'motijheel': 16
    };

    const distance = Math.abs(stations[fromStation] - stations[toStation]);
    fare += distance * 5; // 5 taka per station

    // Display the result
    document.getElementById('fareAmount').textContent = fare;
    document.getElementById('fromStationDisplay').textContent = fromStation.replace('_', ' ').toUpperCase();
    document.getElementById('toStationDisplay').textContent = toStation.replace('_', ' ').toUpperCase();
    document.getElementById('fareResult').style.display = 'block';
});

// QR Code Generator
document.getElementById('qrForm').addEventListener('submit', function (e) {
    e.preventDefault();

    // In a real app, this would generate a unique QR code
    document.getElementById('qrCodeContainer').style.display = 'block';

    // Scroll to QR code
    document.getElementById('qrCodeContainer').scrollIntoView({ behavior: 'smooth' });
});