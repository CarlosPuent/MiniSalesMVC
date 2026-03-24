using Rsm.MiniSalesOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rsm.MiniSalesOrder.Web.Models
{
    public class SalesOrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de Orden")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cliente es requerido")]
        [Display(Name = "Cliente")]
        public int CustomerId { get; set; }

        [Display(Name = "Cliente")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Fecha de Orden")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Estado")]
        public SalesOrderStatus Status { get; set; }

        [Display(Name = "Estado")]
        public string StatusName => Status switch
        {
            SalesOrderStatus.New => "Nueva",
            SalesOrderStatus.Processing => "En Proceso",
            SalesOrderStatus.Shipped => "Enviada",
            SalesOrderStatus.Invoiced => "Facturada",
            _ => Status.ToString()
        };

        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Impuesto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Tax { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Total { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        [Display(Name = "Notas")]
        public string? Notes { get; set; }

        [Display(Name = "Artículos")]
        public List<SalesOrderItemViewModel> Items { get; set; } = new();

        public ShipmentViewModel? Shipment { get; set; }

        public InvoiceViewModel? Invoice { get; set; }

        public bool CanProcessOrder => Status == SalesOrderStatus.New && Items.Any();
        public bool CanShipOrder => Status == SalesOrderStatus.Processing;
        public bool CanInvoiceOrder => Status == SalesOrderStatus.Shipped;
        public bool CanAddItems => Status == SalesOrderStatus.New || Status == SalesOrderStatus.Processing;
        public bool HasItems => Items.Any();
        public bool HasShipment => Shipment is not null;
        public bool HasInvoice => Invoice is not null;
        public bool IsInvoicePendingPayment => Invoice is not null && !Invoice.Paid;
    }
}