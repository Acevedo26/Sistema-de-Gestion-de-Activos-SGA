using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion
{
    public class CategoriaVidaUtilUpdateDto
    {
        [Required]
        [Range(1, 100)]
        public int VidaUtil { get; set; }
    }
}
