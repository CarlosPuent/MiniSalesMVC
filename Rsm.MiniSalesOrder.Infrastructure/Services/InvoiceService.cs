using Microsoft.EntityFrameworkCore;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Domain.Enums;
using Rsm.MiniSalesOrder.Infrastructure.Data;

namespace Rsm.MiniSalesOrder.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISalesOrderService _salesOrderService;

        public InvoiceService(ApplicationDbContext context, ISalesOrderService salesOrderService)
        {
            _context = context;
            _salesOrderService = salesOrderService;
        }

        public async Task<Invoice?> GetInvoiceByOrderIdAsync(int orderId)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Include(i => i.SalesOrder)
                .FirstOrDefaultAsync(i => i.SalesOrderId == orderId);
        }

        public async Task<Invoice?> CreateInvoiceAsync(Invoice invoice)
        {
            var order = await _context.SalesOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == invoice.SalesOrderId);

            if (order is null || order.Status != SalesOrderStatus.Shipped)
            {
                return null;
            }

            var alreadyExists = await _context.Invoices
                .AnyAsync(i => i.SalesOrderId == invoice.SalesOrderId);

            if (alreadyExists)
            {
                return null;
            }

            invoice.InvoiceNumber = await GenerateInvoiceNumberAsync();
            invoice.InvoiceDate = DateTime.UtcNow;
            invoice.Amount = order.Total;
            invoice.Paid = false;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            await _salesOrderService.UpdateOrderStatusAsync(invoice.SalesOrderId, SalesOrderStatus.Invoiced);

            return invoice;
        }

        public async Task<Invoice?> UpdateInvoiceAsync(Invoice invoice)
        {
            var existingInvoice = await _context.Invoices.FindAsync(invoice.Id);
            if (existingInvoice is null)
            {
                return null;
            }

            existingInvoice.Paid = invoice.Paid;

            await _context.SaveChangesAsync();
            return existingInvoice;
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var now = DateTime.UtcNow;
            var count = await _context.Invoices.CountAsync() + 1;

            return $"INV-{now:yyyyMM}-{count:D4}";
        }
    }
}