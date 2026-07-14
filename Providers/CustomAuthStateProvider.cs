using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Providers
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionResult = await _sessionStorage.GetAsync<string>("UserSession");
                var userSessionJson = userSessionResult.Success ? userSessionResult.Value : null;

                if (string.IsNullOrEmpty(userSessionJson))
                    return new AuthenticationState(_anonymous);

                var claimsData = JsonSerializer.Deserialize<Dictionary<string, string>>(userSessionJson);
                
                var claims = new List<Claim>();
                if (claimsData != null)
                {
                    foreach (var kvp in claimsData)
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value));
                    }
                }

                var identity = new ClaimsIdentity(claims, "CustomAuth", ClaimTypes.Name, ClaimTypes.Role);
                var principal = new ClaimsPrincipal(identity);

                return new AuthenticationState(principal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task MarkUserAsAuthenticated(Usuario usuario)
        {
            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.Name, usuario.Nombre },
                { ClaimTypes.Email, usuario.Correo },
                { ClaimTypes.Role, usuario.Rol.Nombre.ToString() }
            };

            var json = JsonSerializer.Serialize(claims);
            await _sessionStorage.SetAsync("UserSession", json);

            var identity = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value)), "CustomAuth", ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _sessionStorage.DeleteAsync("UserSession");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }
    }
}
