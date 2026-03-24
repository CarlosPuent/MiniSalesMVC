using System;
using System.ComponentModel.DataAnnotations;

namespace Rsm.MiniSalesOrder.Web.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, 99999.99, ErrorMessage = "El precio debe estar entre $0.01 y $99,999.99")]
        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, 9999, ErrorMessage = "El stock debe estar entre 0 y 9,999")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; }

        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime CreatedAt { get; set; }
    }
}