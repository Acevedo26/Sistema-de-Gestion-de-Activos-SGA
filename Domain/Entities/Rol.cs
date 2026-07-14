using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("roles")]
    public class Rol
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [Column("nombre")]
        public NombreRol Nombre { get; set; }

        // Navegación
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
