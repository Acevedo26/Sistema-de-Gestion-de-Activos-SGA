import re

with open('Components/Pages/Users.razor', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace('@inject NavigationManager Navigation', '@inject NavigationManager Navigation\n@inject Sistema_de_Gestion_de_Activos.Application.Interfaces.IAlertService AlertService')

new_method = '''private async Task ToggleEstado(UsuarioResponseDto usuario)
    {
        string action = usuario.Estado == EstadoUsuario.Activo ? "desactivar" : "activar";
        string title = usuario.Estado == EstadoUsuario.Activo ? "Desactivar usuario" : "Activar usuario";
        var confirmed = await AlertService.ConfirmAsync(
            title,
            $"¿Está seguro que desea {action} al usuario {usuario.Nombre}?"
        );
        if (confirmed)
        {
            try
            {
                if (usuario.Estado == EstadoUsuario.Activo)
                    await UsuarioService.DesactivarUsuarioAsync(usuario.Id);
                else
                    await UsuarioService.ActivarUsuarioAsync(usuario.Id);
                await AlertService.ShowSuccessAsync("Operación exitosa", "Usuario actualizado correctamente.");
                await LoadData();
            }
            catch (Exception ex)
            {
                await AlertService.ShowErrorAsync("Error", ex.Message);
            }
        }
    }'''

content = re.sub(r'private async Task ToggleEstado\(UsuarioResponseDto usuario\).*?\}', new_method, content, flags=re.DOTALL)

with open('Components/Pages/Users.razor', 'w', encoding='utf-8') as f:
    f.write(content)