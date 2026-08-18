using Microsoft.JSInterop;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class AlertService : IAlertService
    {
        private readonly IJSRuntime _jsRuntime;

        public AlertService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> ConfirmAsync(string title, string text, string icon = "warning", string confirmButtonText = "Confirmar", string cancelButtonText = "Cancelar")
        {
            return await _jsRuntime.InvokeAsync<bool>("sgaAlerts.confirm", title, text, icon, confirmButtonText, cancelButtonText);
        }

        public async Task ShowSuccessAsync(string title, string text = "")
        {
            await _jsRuntime.InvokeVoidAsync("sgaAlerts.success", title, text);
        }

        public async Task ShowErrorAsync(string title, string text = "")
        {
            await _jsRuntime.InvokeVoidAsync("sgaAlerts.error", title, text);
        }

        public async Task ShowWarningAsync(string title, string text = "")
        {
            await _jsRuntime.InvokeVoidAsync("sgaAlerts.warning", title, text);
        }
    }
}