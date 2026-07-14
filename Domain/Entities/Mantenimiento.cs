using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("mantenimientos")]
    public class Mantenimiento
    {
        [Key]
        public Guid Id { get; set; }
    }
}
