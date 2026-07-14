using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion
{
    public class CategoriaResponseDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int VidaUtil { get; set; }
        public decimal ValorResidual { get; set; }
    }
}
