using Rsm.MiniSalesOrder.Domain.Entities;

namespace Rsm.MiniSalesOrder.Application.Interfaces
{
    public interface IShipmentService
    {
        Task<Shipment?> GetShipmentByOrderIdAsync(int orderId);
        Task<Shipment?> CreateShipmentAsync(Shipment shipment);
        Task<Shipment?> UpdateShipmentAsync(Shipment shipment);
    }
}