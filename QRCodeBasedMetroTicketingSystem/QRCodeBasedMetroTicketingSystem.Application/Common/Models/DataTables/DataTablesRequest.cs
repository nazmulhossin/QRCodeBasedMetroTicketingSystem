namespace QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables
{
    public class DataTablesRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTablesSearch? Search { get; set; }
        public List<DataTablesOrder>? Order { get; set; }
        public List<DataTablesColumn>? Columns { get; set; }
    }
}
