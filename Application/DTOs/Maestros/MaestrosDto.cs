using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Maestros
{
    public class UbicacionDto
    {
        public Guid Id { get; set; }
        public string Edificio { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Oficina { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }

    public class CategoriaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int VidaUtil { get; set; }
    }

    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Sistema_de_Gestion_de_Activos.Domain.Enums.NombreRol Rol { get; set; }
    }

    public class NotificacionDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; }
        public bool Leida { get; set; }
    }
}
