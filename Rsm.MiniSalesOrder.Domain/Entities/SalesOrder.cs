using Rsm.MiniSalesOrder.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Rsm.MiniSalesOrder.Domain.Entities
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public SalesOrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string? Notes { get; set; }

        public Customer? Customer { get; set; }
        public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
        public Shipment? Shipment { get; set; }
        public Invoice? Invoice { get; set; }
    }
}