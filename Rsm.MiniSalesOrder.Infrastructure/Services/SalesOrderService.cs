using Microsoft.EntityFrameworkCore;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Domain.Enums;
using Rsm.MiniSalesOrder.Infrastructure.Data;

namespace Rsm.MiniSalesOrder.Infrastructure.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;

        private const decimal TaxRate = 0.13m;

        public SalesOrderService(ApplicationDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<IEnumerable<SalesOrder>> GetAllSalesOrdersAsync()
        {
            return await _context.SalesOrders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Shipment)
                .Include(o => o.Invoice)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<SalesOrder?> GetSalesOrderByIdAsync(int id)
        {
            return await _context.SalesOrders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Shipment)
                .Include(o => o.Invoice)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<SalesOrder> CreateSalesOrderAsync(SalesOrder salesOrder)
        {
            salesOrder.OrderDate = DateTime.UtcNow;
            salesOrder.Status = SalesOrderStatus.New;
            salesOrder.OrderNumber = await GenerateOrderNumberAsync();
            salesOrder.Subtotal = 0m;
            salesOrder.Tax = 0m;
            salesOrder.Total = 0m;
            salesOrder.Notes ??= string.Empty;

            _context.SalesOrders.Add(salesOrder);
            await _context.SaveChangesAsync();

            return salesOrder;
        }

        public async Task<SalesOrder?> UpdateSalesOrderAsync(SalesOrder salesOrder)
        {
            var existingOrder = await _context.SalesOrders.FindAsync(salesOrder.Id);
            if (existingOrder is null)
            {
                return null;
            }

            existingOrder.Notes = salesOrder.Notes ?? string.Empty;

            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<bool> DeleteSalesOrderAsync(int id)
        {
            var order = await _context.SalesOrders
                .Include(o => o.Items)
                .Include(o => o.Shipment)
                .Include(o => o.Invoice)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
            {
                return false;
            }

            if (order.Status != SalesOrderStatus.New)
            {
                return false;
            }

            if (order.Invoice is not null)
            {
                _context.Invoices.Remove(order.Invoice);
            }

            if (order.Shipment is not null)
            {
                _context.Shipments.Remove(order.Shipment);
            }

            if (order.Items.Any())
            {
                _context.SalesOrderItems.RemoveRange(order.Items);
            }

            _context.SalesOrders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<SalesOrder?> AddItemToOrderAsync(int orderId, SalesOrderItem item)
        {
            if (item.Quantity <= 0)
            {
                return null;
            }

            var order = await _context.SalesOrders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return null;
            }

            if (!CanModifyItems(order.Status))
            {
                return null;
            }

            var product = await _context.Products.FindAsync(item.ProductId);
            if (product is null || !product.IsActive)
            {
                return null;
            }

            if (product.Stock < item.Quantity)
            {
                return null;
            }

            item.SalesOrderId = orderId;
            item.UnitPrice = product.UnitPrice;
            item.LineTotal = product.UnitPrice * item.Quantity;

            _context.SalesOrderItems.Add(item);
            await _context.SaveChangesAsync();

            await RecalculateOrderTotalsAsync(order.Id);

            return await GetTrackedSalesOrderByIdAsync(orderId);
        }

        public async Task<SalesOrder?> RemoveItemFromOrderAsync(int orderId, int itemId)
        {
            var order = await _context.SalesOrders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return null;
            }

            if (!CanModifyItems(order.Status))
            {
                return null;
            }

            var item = order.Items.FirstOrDefault(i => i.Id == itemId);
            if (item is null)
            {
                return null;
            }

            _context.SalesOrderItems.Remove(item);
            await _context.SaveChangesAsync();

            await RecalculateOrderTotalsAsync(order.Id);

            return await GetTrackedSalesOrderByIdAsync(orderId);
        }

        public async Task<SalesOrder?> UpdateOrderStatusAsync(int orderId, SalesOrderStatus newStatus)
        {
            var order = await _context.SalesOrders
                .Include(o => o.Items)
                .Include(o => o.Shipment)
                .Include(o => o.Invoice)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return null;
            }

            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                return null;
            }

            switch (newStatus)
            {
                case SalesOrderStatus.Processing:
                    if (!order.Items.Any())
                    {
                        return null;
                    }
                    break;

                case SalesOrderStatus.Shipped:
                    if (!order.Items.Any())
                    {
                        return null;
                    }

                    if (order.Shipment is null)
                    {
                        return null;
                    }

                    foreach (var item in order.Items)
                    {
                        var stockUpdated = await _productService.UpdateProductStockAsync(item.ProductId, item.Quantity);
                        if (!stockUpdated)
                        {
                            return null;
                        }
                    }
                    break;

                case SalesOrderStatus.Invoiced:
                    if (!order.Items.Any())
                    {
                        return null;
                    }

                    if (order.Shipment is null)
                    {
                        return null;
                    }

                    if (order.Invoice is null)
                    {
                        return null;
                    }
                    break;
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return await GetTrackedSalesOrderByIdAsync(orderId);
        }

        public async Task<Dictionary<SalesOrderStatus, int>> GetOrderCountsByStatusAsync()
        {
            var result = Enum.GetValues<SalesOrderStatus>()
                .ToDictionary(status => status, _ => 0);

            var counts = await _context.SalesOrders
                .AsNoTracking()
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            foreach (var item in counts)
            {
                result[item.Status] = item.Count;
            }

            return result;
        }

        public async Task<int> GetTotalOrderCountAsync()
        {
            return await _context.SalesOrders.CountAsync();
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var now = DateTime.UtcNow;
            var count = await _context.SalesOrders.CountAsync() + 1;

            return $"ORD-{now:yyyyMM}-{count:D4}";
        }

        public async Task<bool> CanChangeStatusAsync(int orderId, SalesOrderStatus newStatus)
        {
            var order = await _context.SalesOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return false;
            }

            return IsValidStatusTransition(order.Status, newStatus);
        }

        private static bool CanModifyItems(SalesOrderStatus status)
        {
            return status == SalesOrderStatus.New || status == SalesOrderStatus.Processing;
        }

        private static bool IsValidStatusTransition(SalesOrderStatus currentStatus, SalesOrderStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (SalesOrderStatus.New, SalesOrderStatus.Processing) => true,
                (SalesOrderStatus.Processing, SalesOrderStatus.Shipped) => true,
                (SalesOrderStatus.Shipped, SalesOrderStatus.Invoiced) => true,
                _ => false
            };
        }

        private async Task RecalculateOrderTotalsAsync(int orderId)
        {
            var order = await _context.SalesOrders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return;
            }

            order.Subtotal = order.Items.Sum(i => i.LineTotal);
            order.Tax = Math.Round(order.Subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);
            order.Total = order.Subtotal + order.Tax;

            await _context.SaveChangesAsync();
        }

        private async Task<SalesOrder?> GetTrackedSalesOrderByIdAsync(int id)
        {
            return await _context.SalesOrders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Shipment)
                .Include(o => o.Invoice)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}