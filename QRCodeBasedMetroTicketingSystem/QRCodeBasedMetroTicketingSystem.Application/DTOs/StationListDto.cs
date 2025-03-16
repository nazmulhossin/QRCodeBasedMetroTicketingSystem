using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationListDto
    {
        public int StationId { get; set; }
        public string? StationName { get; set; }
        public int Order { get; set; }
    }
}
