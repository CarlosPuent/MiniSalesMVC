using Rsm.MiniSalesOrder.Domain.Entities;

namespace Rsm.MiniSalesOrder.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetInvoiceByOrderIdAsync(int orderId);
        Task<Invoice?> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice?> UpdateInvoiceAsync(Invoice invoice);
        Task<string> GenerateInvoiceNumberAsync();
    }
}