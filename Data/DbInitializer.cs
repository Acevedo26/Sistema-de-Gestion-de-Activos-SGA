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
            if (!context.Roles.Any())
            {
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

            // 3. Crear Datos de Prueba - Módulo 6 (Depreciación)
            if (!context.Categorias.Any())
            {
                var catComputo = new Categoria { Id = Guid.NewGuid(), Nombre = "Equipos de Cómputo", VidaUtil = 5, ValorResidual = 10m };
                var catMobiliario = new Categoria { Id = Guid.NewGuid(), Nombre = "Mobiliario", VidaUtil = 10, ValorResidual = 5m };
                context.Categorias.AddRange(catComputo, catMobiliario);

                var ubiSistemas = new Ubicacion { Id = Guid.NewGuid(), Edificio = "Edificio Central", Departamento = "Sistemas", Oficina = "101" };
                var ubiVentas = new Ubicacion { Id = Guid.NewGuid(), Edificio = "Edificio Central", Departamento = "Ventas", Oficina = "202" };
                context.Ubicaciones.AddRange(ubiSistemas, ubiVentas);

                var paramMetodo = new ParametroSistema { Id = Guid.NewGuid(), Clave = "METODO_DEPRECIACION", Valor = "LineaRecta", Descripcion = "Método de depreciación por defecto" };
                context.ParametrosSistema.Add(paramMetodo);

                context.SaveChanges();

                var activo1 = new Activo 
                { 
                    Id = Guid.NewGuid(), 
                    Codigo = "COMP-001", 
                    Descripcion = "Laptop Dell XPS 15", 
                    CategoriaId = catComputo.Id, 
                    UbicacionId = ubiSistemas.Id, 
                    Estado = EstadoActivo.Activo,
                    FechaAdquisicion = DateTime.UtcNow.AddYears(-2),
                    CostoInicial = 25000m
                };
                var activo2 = new Activo 
                { 
                    Id = Guid.NewGuid(), 
                    Codigo = "MOB-001", 
                    Descripcion = "Silla Ergonómica Herman Miller", 
                    CategoriaId = catMobiliario.Id, 
                    UbicacionId = ubiVentas.Id, 
                    Estado = EstadoActivo.Activo,
                    FechaAdquisicion = DateTime.UtcNow.AddMonths(-6),
                    CostoInicial = 3000m
                };
                context.Activos.AddRange(activo1, activo2);
                context.SaveChanges();

                var dep1 = new Depreciacion 
                { 
                    Id = Guid.NewGuid(), 
                    ActivoId = activo1.Id, 
                    VidaUtilAsignada = 5, 
                    ValorResidual = 2500m, 
                    ValorActual = 15000m, 
                    PorcentajeConsumido = 40,
                    FechaUltimoCalculo = DateTime.UtcNow
                };
                var dep2 = new Depreciacion 
                { 
                    Id = Guid.NewGuid(), 
                    ActivoId = activo2.Id, 
                    VidaUtilAsignada = 10, 
                    ValorResidual = 150m, 
                    ValorActual = 2850m, 
                    PorcentajeConsumido = 5,
                    FechaUltimoCalculo = DateTime.UtcNow
                };
                context.Depreciaciones.AddRange(dep1, dep2);
                context.SaveChanges();

                var hist1 = new HistorialDepreciacion { Id = Guid.NewGuid(), DepreciacionId = dep1.Id, FechaConsulta = DateTime.UtcNow.AddMonths(-1), ValorActual = 15500m };
                var hist2 = new HistorialDepreciacion { Id = Guid.NewGuid(), DepreciacionId = dep2.Id, FechaConsulta = DateTime.UtcNow.AddMonths(-1), ValorActual = 2900m };
                context.HistorialDepreciaciones.AddRange(hist1, hist2);

                var notif1 = new Notificacion 
                { 
                    Id = Guid.NewGuid(), 
                    ActivoId = activo1.Id, 
                    Tipo = TipoNotificacion.DepreciacionProxima, 
                    Mensaje = "El activo COMP-001 se encuentra cerca del fin de su vida útil.", 
                    FechaGeneracion = DateTime.UtcNow, 
                    Leida = false 
                };
                context.Notificaciones.Add(notif1);

                context.SaveChanges();
            }
        }
    }
}
