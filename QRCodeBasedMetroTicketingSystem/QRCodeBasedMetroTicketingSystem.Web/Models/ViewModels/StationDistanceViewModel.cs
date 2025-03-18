namespace QRCodeBasedMetroTicketingSystem.Web.Models.ViewModels
{
    public class StationDistanceViewModel
    {
        public int Station1Id { get; set; }
        public int Station2Id { get; set; }
        public string Station1Name { get; set; }
        public string Station2Name { get; set; }
        public decimal Distance { get; set; }
        public string ErrorMessage { get; set; }
    }
}
