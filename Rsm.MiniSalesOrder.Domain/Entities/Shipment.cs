using Rsm.MiniSalesOrder.Domain.Enums;
using System;

namespace Rsm.MiniSalesOrder.Domain.Entities
{
    public class Shipment
    {
        public int Id { get; set; }
        public int SalesOrderId { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime ShippingDate { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public ShipmentStatus Status { get; set; }

        // Navigation properties
        public SalesOrder SalesOrder { get; set; }
    }
}
