using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IStationService
    {
        Task<DataTablesResponse<StationDto>> GetStationsDataTableAsync(DataTablesRequest request);
    }
}
