using System.ComponentModel.DataAnnotations;

namespace Rsm.MiniSalesOrder.Web.Models
{
    public class SalesOrderItemViewModel
    {
        public int Id { get; set; }

        public int SalesOrderId { get; set; }

        [Display(Name = "Producto")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductId { get; set; }

        [Display(Name = "Producto")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, 999, ErrorMessage = "La cantidad debe estar entre 1 y 999")]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }

        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Total Línea")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal LineTotal { get; set; }
    }
}