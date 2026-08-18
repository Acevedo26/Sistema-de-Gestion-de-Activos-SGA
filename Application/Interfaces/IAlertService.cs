using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IAlertService
    {
        Task<bool> ConfirmAsync(string title, string text, string icon = "warning", string confirmButtonText = "Confirmar", string cancelButtonText = "Cancelar");
        Task ShowSuccessAsync(string title, string text = "");
        Task ShowErrorAsync(string title, string text = "");
        Task ShowWarningAsync(string title, string text = "");
    }
}