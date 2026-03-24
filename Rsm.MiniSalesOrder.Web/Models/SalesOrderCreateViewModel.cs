using System.ComponentModel.DataAnnotations;

namespace Rsm.MiniSalesOrder.Web.Models
{
    public class SalesOrderCreateViewModel
    {
        [Display(Name = "Cliente")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente.")]
        public int CustomerId { get; set; }

        [Display(Name = "Notas")]
        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres.")]
        public string? Notes { get; set; }
    }
}