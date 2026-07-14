using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("categorias")]
    public class Categoria
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La vida útil es obligatoria.")]
        [Range(1, 100, ErrorMessage = "La vida útil debe estar entre 1 y 100 años.")]
        [Column("vida_util")]
        public int VidaUtil { get; set; }

        [Required(ErrorMessage = "El valor residual es obligatorio.")]
        [Column("valor_residual", TypeName = "decimal(18,2)")]
        public decimal ValorResidual { get; set; } = 0m;
    }
}
