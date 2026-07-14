using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("movimientos")]
    public class Movimiento
    {
        [Key]
        public Guid Id { get; set; }
    }
}
