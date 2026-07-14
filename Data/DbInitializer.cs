using System;
using System.Linq;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;
using BCrypt.Net;

namespace Sistema_de_Gestion_de_Activos.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SgaDbContext context)
        {
            if (context.Roles.Any())
            {
                return;
            }

            // 1. Crear Roles
            var roles = new Rol[]
            {
                new Rol { Id = Guid.NewGuid(), Nombre = NombreRol.Administrador },
                new Rol { Id = Guid.NewGuid(), Nombre = NombreRol.Gestor },
                new Rol { Id = Guid.NewGuid(), Nombre = NombreRol.Tecnico },
                new Rol { Id = Guid.NewGuid(), Nombre = NombreRol.Visualizador }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();

            // 2. Crear Usuario Administrador
            var adminRole = roles.First(r => r.Nombre == NombreRol.Administrador);
            var adminUser = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = "Admin de Prueba",
                Correo = "admin@sga.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                RolId = adminRole.Id,
                Estado = EstadoUsuario.Activo
            };

            context.Usuarios.Add(adminUser);
            context.SaveChanges();
        }
    }
}
