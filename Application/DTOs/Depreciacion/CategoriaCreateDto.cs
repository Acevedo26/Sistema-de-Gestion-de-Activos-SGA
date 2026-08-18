using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;

public class CategoriaCreateDto
{
    [Required, MaxLength(100)] public string Nombre { get; set; } = string.Empty;
    [Range(1, 100)] public int VidaUtil { get; set; }
    [Range(0, double.MaxValue)] public decimal ValorResidual { get; set; }
}
