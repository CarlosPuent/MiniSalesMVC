using System;
using System.ComponentModel.DataAnnotations;

namespace Rsm.MiniSalesOrder.Web.Models
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }

        public int SalesOrderId { get; set; }

        [Display(Name = "Número de Factura")]
        public string? InvoiceNumber { get; set; }

        [Display(Name = "Fecha de Factura")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime InvoiceDate { get; set; }

        [Display(Name = "Monto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Amount { get; set; }

        [Display(Name = "Pagada")]
        public bool Paid { get; set; }

        public string PaymentStatusName => Paid ? "Pagada" : "Pendiente de pago";
    }
}