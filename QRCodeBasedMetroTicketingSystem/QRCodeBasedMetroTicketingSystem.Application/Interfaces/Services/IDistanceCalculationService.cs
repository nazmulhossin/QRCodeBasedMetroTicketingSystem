using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IDistanceCalculationService
    {
        Task<decimal> GetDistanceBetweenAsync(int Station1Id, int Station2Id);
    }
}
