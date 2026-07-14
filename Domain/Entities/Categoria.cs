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

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(1, 100)]
        [Column("vida_util")]
        public int VidaUtil { get; set; }

        [Required]
        [Column("valor_residual", TypeName = "decimal(18,2)")]
        public decimal ValorResidual { get; set; } = 0m;
    }
}
