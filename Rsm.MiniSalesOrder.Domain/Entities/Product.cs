using System;
using System.Collections.Generic;

namespace Rsm.MiniSalesOrder.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<SalesOrderItem> OrderItems { get; set; } = new List<SalesOrderItem>();
    }
}
