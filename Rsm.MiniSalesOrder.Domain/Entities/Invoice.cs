using System;

namespace Rsm.MiniSalesOrder.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public int SalesOrderId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; }

        // Navigation properties
        public SalesOrder SalesOrder { get; set; }
    }
}
