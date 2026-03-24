using Rsm.MiniSalesOrder.Domain.Enums;
using System.Collections.Generic;

namespace Rsm.MiniSalesOrder.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }

        public Dictionary<SalesOrderStatus, int> OrdersByStatus { get; set; } = new();

        public int PendingInvoiceCount =>
            OrdersByStatus.TryGetValue(SalesOrderStatus.Shipped, out var count) ? count : 0;
    }
}