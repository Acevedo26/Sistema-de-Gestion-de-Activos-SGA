using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("programas_mantenimiento")]
    public class ProgramaMantenimiento
    {
        [Key]
        public Guid Id { get; set; }
    }
}
