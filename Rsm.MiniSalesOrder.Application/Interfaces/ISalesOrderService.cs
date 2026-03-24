using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Domain.Enums;

namespace Rsm.MiniSalesOrder.Application.Interfaces
{
    public interface ISalesOrderService
    {
        Task<IEnumerable<SalesOrder>> GetAllSalesOrdersAsync();
        Task<SalesOrder?> GetSalesOrderByIdAsync(int id);
        Task<SalesOrder> CreateSalesOrderAsync(SalesOrder salesOrder);
        Task<SalesOrder?> UpdateSalesOrderAsync(SalesOrder salesOrder);
        Task<bool> DeleteSalesOrderAsync(int id);
        Task<SalesOrder?> AddItemToOrderAsync(int orderId, SalesOrderItem item);
        Task<SalesOrder?> RemoveItemFromOrderAsync(int orderId, int itemId);
        Task<SalesOrder?> UpdateOrderStatusAsync(int orderId, SalesOrderStatus newStatus);
        Task<Dictionary<SalesOrderStatus, int>> GetOrderCountsByStatusAsync();
        Task<int> GetTotalOrderCountAsync();
        Task<string> GenerateOrderNumberAsync();
        Task<bool> CanChangeStatusAsync(int orderId, SalesOrderStatus newStatus);
    }
}