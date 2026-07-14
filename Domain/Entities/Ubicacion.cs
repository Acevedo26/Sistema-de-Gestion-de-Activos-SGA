using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("ubicaciones")]
    public class Ubicacion
    {
        [Key]
        public Guid Id { get; set; }
        
        [Column("edificio")]
        public string Edificio { get; set; } = string.Empty;

        [Column("departamento")]
        public string Departamento { get; set; } = string.Empty;
        
        [Column("oficina")]
        public string Oficina { get; set; } = string.Empty;
    }
}
