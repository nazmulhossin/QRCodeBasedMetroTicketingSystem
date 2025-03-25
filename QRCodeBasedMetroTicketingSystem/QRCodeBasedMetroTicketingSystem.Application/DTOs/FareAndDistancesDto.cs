namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class FareAndDistancesDto
    {
        public List<StationListDto> StationList { get; set; } = new List<StationListDto>();
        public IEnumerable<FareDistanceDto>? FareDistanceData { get; set; }
    }
}
