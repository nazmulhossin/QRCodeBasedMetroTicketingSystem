namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalRegisteredUsers { get; set; }
        public int CurrentPassengerCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int QRTickets { get; set; }
        public int RapidPasses { get; set; }
        public List<ChartDataPoint> PassengerTraffic { get; set; }
        public List<ChartDataPoint> RevenueAnalysis { get; set; }
    }
}
