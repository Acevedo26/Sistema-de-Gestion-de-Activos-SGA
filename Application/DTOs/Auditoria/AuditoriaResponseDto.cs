using System;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Auditoria
{
    public class AuditoriaResponseDto
    {
        public Guid Id { get; set; }
        public Guid? UsuarioId { get; set; }
        public string TablaAfectada { get; set; } = string.Empty;
        public AccionAuditoria Accion { get; set; }
        public string? ValoresAnteriores { get; set; }
        public string? ValoresNuevos { get; set; }
        public DateTime FechaAccion { get; set; }
    }
}
