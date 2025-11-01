namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class UserDashboardStatsDto
    {
        public decimal TotalTopUps { get; set; }
        public int TotalTrips { get; set; }
        public int ValidTickets { get; set; }
    }
}
