using Rsm.MiniSalesOrder.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rsm.MiniSalesOrder.Web.Models
{
    public class ShipmentViewModel
    {
        public int Id { get; set; }

        public int SalesOrderId { get; set; }

        [Required(ErrorMessage = "La dirección de envío es requerida")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        [Display(Name = "Dirección de Envío")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Display(Name = "Fecha de Envío")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime ShippingDate { get; set; }

        [StringLength(50, ErrorMessage = "El número de seguimiento no puede exceder 50 caracteres")]
        [Display(Name = "Número de Seguimiento")]
        public string? TrackingNumber { get; set; }

        [Required(ErrorMessage = "El transportista es requerido")]
        [StringLength(50, ErrorMessage = "El transportista no puede exceder 50 caracteres")]
        [Display(Name = "Transportista")]
        public string Carrier { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public ShipmentStatus Status { get; set; }

        [Display(Name = "Estado")]
        public string StatusName => Status switch
        {
            ShipmentStatus.Pending => "Pendiente",
            ShipmentStatus.Shipped => "Enviado",
            ShipmentStatus.Delivered => "Entregado",
            _ => Status.ToString()
        };
    }
}