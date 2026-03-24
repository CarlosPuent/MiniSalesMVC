using Microsoft.EntityFrameworkCore;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Domain.Enums;
using Rsm.MiniSalesOrder.Infrastructure.Data;

namespace Rsm.MiniSalesOrder.Infrastructure.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISalesOrderService _salesOrderService;

        public ShipmentService(ApplicationDbContext context, ISalesOrderService salesOrderService)
        {
            _context = context;
            _salesOrderService = salesOrderService;
        }

        public async Task<Shipment?> GetShipmentByOrderIdAsync(int orderId)
        {
            return await _context.Shipments
                .AsNoTracking()
                .Include(s => s.SalesOrder)
                .FirstOrDefaultAsync(s => s.SalesOrderId == orderId);
        }

        public async Task<Shipment?> CreateShipmentAsync(Shipment shipment)
        {
            var order = await _context.SalesOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == shipment.SalesOrderId);

            if (order is null || order.Status != SalesOrderStatus.Processing)
            {
                return null;
            }

            var alreadyExists = await _context.Shipments
                .AnyAsync(s => s.SalesOrderId == shipment.SalesOrderId);

            if (alreadyExists)
            {
                return null;
            }

            shipment.ShippingDate = DateTime.UtcNow;
            shipment.Status = ShipmentStatus.Pending;

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            await _salesOrderService.UpdateOrderStatusAsync(shipment.SalesOrderId, SalesOrderStatus.Shipped);

            return shipment;
        }

        public async Task<Shipment?> UpdateShipmentAsync(Shipment shipment)
        {
            var existingShipment = await _context.Shipments.FindAsync(shipment.Id);
            if (existingShipment is null)
            {
                return null;
            }

            existingShipment.ShippingAddress = shipment.ShippingAddress;
            existingShipment.TrackingNumber = shipment.TrackingNumber;
            existingShipment.Carrier = shipment.Carrier;
            existingShipment.Status = shipment.Status;

            await _context.SaveChangesAsync();
            return existingShipment;
        }
    }
}