using System;
using System.Linq;
using System.Collections.Generic;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;
using BCrypt.Net;

namespace Sistema_de_Gestion_de_Activos.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SgaDbContext context, bool includeDevelopmentUser)
        {
            // 1. Roles
            if (!context.Roles.Any())
            {
                var roles = new Rol[]
                {
                    new Rol { Id = Guid.NewGuid(), Nombre = NombreRol.Administrador },
                    new Rol { Id = Guid.NewGuid(), Nombre = NombreRol.Gestor },
                    new Rol { Id = Guid.NewGuid(), Nombre = NombreRol.Tecnico },
                    new Rol { Id = Guid.NewGuid(), Nombre = NombreRol.Visualizador }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // 2. Usuarios
            if (!context.Usuarios.Any())
            {
                var adminRole = context.Roles.First(r => r.Nombre == NombreRol.Administrador);
                var gestorRole = context.Roles.First(r => r.Nombre == NombreRol.Gestor);
                var tecnicoRole = context.Roles.First(r => r.Nombre == NombreRol.Tecnico);
                var visualizadorRole = context.Roles.First(r => r.Nombre == NombreRol.Visualizador);

                var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

                var usuarios = new List<Usuario>
                {
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Ana Admin", Correo = "ana.admin@empresa.com", PasswordHash = passwordHash, RolId = adminRole.Id, Estado = EstadoUsuario.Activo },
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Carlos Admin", Correo = "carlos.admin@empresa.com", PasswordHash = passwordHash, RolId = adminRole.Id, Estado = EstadoUsuario.Activo },
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Maria Gestor", Correo = "maria.gestor@empresa.com", PasswordHash = passwordHash, RolId = gestorRole.Id, Estado = EstadoUsuario.Activo },
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Juan Gestor", Correo = "juan.gestor@empresa.com", PasswordHash = passwordHash, RolId = gestorRole.Id, Estado = EstadoUsuario.Activo },
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Pedro Tecnico", Correo = "pedro.tecnico@empresa.com", PasswordHash = passwordHash, RolId = tecnicoRole.Id, Estado = EstadoUsuario.Activo },
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Laura Tecnico", Correo = "laura.tecnico@empresa.com", PasswordHash = passwordHash, RolId = tecnicoRole.Id, Estado = EstadoUsuario.Activo },
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Roberto Tecnico", Correo = "roberto.tecnico@empresa.com", PasswordHash = passwordHash, RolId = tecnicoRole.Id, Estado = EstadoUsuario.Activo },
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Sofia Visual", Correo = "sofia.visual@empresa.com", PasswordHash = passwordHash, RolId = visualizadorRole.Id, Estado = EstadoUsuario.Activo },
                    new Usuario { Id = Guid.NewGuid(), Nombre = "Luis Visual", Correo = "luis.visual@empresa.com", PasswordHash = passwordHash, RolId = visualizadorRole.Id, Estado = EstadoUsuario.Activo }
                };

                if (includeDevelopmentUser)
                {
                    usuarios.Add(new Usuario { Id = Guid.NewGuid(), Nombre = "Administrador de desarrollo", Correo = "admin@sga.local", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), RolId = adminRole.Id, Estado = EstadoUsuario.Activo });
                }

                context.Usuarios.AddRange(usuarios);
                context.SaveChanges();
            }

            // 3. Categorías
            if (!context.Categorias.Any())
            {
                var categorias = new[]
                {
                    new Categoria { Id = Guid.NewGuid(), Nombre = "Equipos de Cómputo", VidaUtil = 5, ValorResidual = 10m },
                    new Categoria { Id = Guid.NewGuid(), Nombre = "Dispositivos Electrónicos", VidaUtil = 4, ValorResidual = 10m },
                    new Categoria { Id = Guid.NewGuid(), Nombre = "Mobiliario", VidaUtil = 10, ValorResidual = 15m },
                    new Categoria { Id = Guid.NewGuid(), Nombre = "Maquinaria", VidaUtil = 10, ValorResidual = 20m },
                    new Categoria { Id = Guid.NewGuid(), Nombre = "Herramientas", VidaUtil = 5, ValorResidual = 10m },
                    new Categoria { Id = Guid.NewGuid(), Nombre = "Vehículos", VidaUtil = 8, ValorResidual = 25m },
                    new Categoria { Id = Guid.NewGuid(), Nombre = "Equipos de Red", VidaUtil = 6, ValorResidual = 10m },
                    new Categoria { Id = Guid.NewGuid(), Nombre = "Electrodomésticos", VidaUtil = 7, ValorResidual = 10m }
                };
                context.Categorias.AddRange(categorias);
                context.SaveChanges();
            }

            // 4. Ubicaciones
            if (!context.Ubicaciones.Any())
            {
                var sedes = new[] { "San José", "Heredia", "Alajuela", "Cartago", "Guanacaste" };
                var ubicaciones = new List<Ubicacion>();

                foreach (var sede in sedes)
                {
                    for (int e = 1; e <= 3; e++)
                    {
                        var edificio = $"Edificio {(char)('A' + e - 1)}";
                        var depts = new[] { "Desarrollo", "Ventas", "RH", "Finanzas", "Operaciones", "Logística", "Mantenimiento", "IT", "Administración" };
                        foreach (var dept in depts.Take(3)) // 3 depts per edificio
                        {
                            ubicaciones.Add(new Ubicacion { Id = Guid.NewGuid(), Edificio = $"{edificio} - {sede}", Departamento = dept, Oficina = $"Oficina {(e * 100) + ubicaciones.Count % 10}" });
                        }
                    }
                }
                context.Ubicaciones.AddRange(ubicaciones);
                context.SaveChanges();
            }

            // 5. Parámetros del Sistema
            if (!context.ParametrosSistema.Any())
            {
                context.ParametrosSistema.AddRange(
                    new ParametroSistema { Id = Guid.NewGuid(), Clave = "METODO_DEPRECIACION", Valor = "LineaRecta", Descripcion = "Método de depreciación por defecto" },
                    new ParametroSistema { Id = Guid.NewGuid(), Clave = "DiasAnticipacionMantenimiento", Valor = "7,14,30", Descripcion = "Días de anticipación para alertas de mantenimiento" },
                    new ParametroSistema { Id = Guid.NewGuid(), Clave = "PorcentajeAlertaDepreciacion", Valor = "90", Descripcion = "Porcentaje para activar alerta de depreciación" },
                    new ParametroSistema { Id = Guid.NewGuid(), Clave = "UmbralCostoMantenimiento", Valor = "30", Descripcion = "Porcentaje de valor actual para alerta de costo excesivo" },
                    new ParametroSistema { Id = Guid.NewGuid(), Clave = "TiempoExpiracionToken", Valor = "30", Descripcion = "Minutos de validez del token de recuperación" }
                );
                context.SaveChanges();
            }

            // 6. Activos (50)
            if (!context.Activos.Any())
            {
                var categorias = context.Categorias.ToList();
                var ubicaciones = context.Ubicaciones.ToList();
                var gestores = context.Usuarios.Where(u => u.Rol.Nombre == NombreRol.Gestor).ToList();
                
                var random = new Random(1234);
                var activos = new List<Activo>();

                for (int i = 1; i <= 50; i++)
                {
                    var cat = categorias[random.Next(categorias.Count)];
                    var ubi = ubicaciones[random.Next(ubicaciones.Count)];
                    var resp = gestores[random.Next(gestores.Count)];

                    var fechaAdq = DateTime.UtcNow.AddDays(-random.Next(1, 5 * 365));
                    var costo = (decimal)(random.Next(100, 50000) * 1000);

                    EstadoActivo estado;
                    var randState = random.Next(100);
                    if (randState < 60) estado = EstadoActivo.Activo;
                    else if (randState < 70) estado = EstadoActivo.Mantenimiento;
                    else if (randState < 85) estado = EstadoActivo.Inactivo;
                    else estado = EstadoActivo.Baja;

                    var prefijo = cat.Nombre.Substring(0, 3).ToUpper();

                    var activo = new Activo
                    {
                        Id = Guid.NewGuid(),
                        Codigo = $"{prefijo}-{fechaAdq.Year}-{i:D3}",
                        Descripcion = $"Activo {i} de prueba - {cat.Nombre}",
                        CategoriaId = cat.Id,
                        UbicacionId = ubi.Id,
                        ResponsableId = resp.Id,
                        FechaAdquisicion = fechaAdq,
                        FechaAsignacion = fechaAdq.AddDays(random.Next(1, 30)),
                        CostoInicial = costo,
                        Estado = estado
                    };
                    activos.Add(activo);
                }
                context.Activos.AddRange(activos);
                context.SaveChanges();

                // 7. Depreciaciones y Movimientos Iniciales
                var depreciaciones = new List<Depreciacion>();
                var movimientos = new List<Movimiento>();
                var adminUser = context.Usuarios.First(u => u.Rol.Nombre == NombreRol.Administrador);

                foreach (var a in activos)
                {
                    var cat = categorias.First(c => c.Id == a.CategoriaId);
                    var mesesTotales = cat.VidaUtil * 12;
                    var mesesPasados = (int)((DateTime.UtcNow - a.FechaAdquisicion).TotalDays / 30.44);
                    if (mesesPasados > mesesTotales) mesesPasados = mesesTotales;
                    if (mesesPasados < 0) mesesPasados = 0;

                    var depreciacionMensual = a.CostoInicial * (1m - (cat.ValorResidual / 100m)) / mesesTotales;
                    var depreciacionAcumulada = depreciacionMensual * mesesPasados;
                    var valorActual = a.CostoInicial - depreciacionAcumulada;
                    var porcentaje = (depreciacionAcumulada / a.CostoInicial) * 100m;

                    var dep = new Depreciacion
                    {
                        Id = Guid.NewGuid(),
                        ActivoId = a.Id,
                        VidaUtilAsignada = cat.VidaUtil,
                        ValorResidual = a.CostoInicial * (cat.ValorResidual / 100m),
                        ValorActual = valorActual,
                        PorcentajeConsumido = Math.Round(porcentaje, 2),
                        FechaUltimoCalculo = DateTime.UtcNow
                    };
                    depreciaciones.Add(dep);

                    // Movimiento inicial
                    movimientos.Add(new Movimiento
                    {
                        Id = Guid.NewGuid(),
                        ActivoId = a.Id,
                        FechaMovimiento = a.FechaAdquisicion,
                        UbicacionOrigenId = a.UbicacionId,
                        UbicacionDestinoId = a.UbicacionId,
                        UsuarioId = adminUser.Id,
                        Observaciones = "Alta inicial en el sistema."
                    });

                    // Si ha pasado al menos 1 año, simular traslado o reasignación
                    if (mesesPasados > 12 && random.Next(2) == 0)
                    {
                        movimientos.Add(new Movimiento
                        {
                            Id = Guid.NewGuid(),
                            ActivoId = a.Id,
                            FechaMovimiento = a.FechaAdquisicion.AddYears(1),
                            UbicacionOrigenId = a.UbicacionId,
                            UbicacionDestinoId = ubicaciones[random.Next(ubicaciones.Count)].Id,
                            UsuarioId = adminUser.Id,
                            Observaciones = "Traslado simulado de prueba."
                        });
                    }
                }
                context.Depreciaciones.AddRange(depreciaciones);
                context.Movimientos.AddRange(movimientos);
                context.SaveChanges();

                // 8. Mantenimientos (30) y Programas
                var mantenimientos = new List<Mantenimiento>();
                var programas = new List<ProgramaMantenimiento>();
                var tecnicos = context.Usuarios.Where(u => u.Rol.Nombre == NombreRol.Tecnico).ToList();

                for (int i = 1; i <= 30; i++)
                {
                    var a = activos[random.Next(activos.Count)];
                    var tec = tecnicos[random.Next(tecnicos.Count)];
                    var tipo = random.Next(100) < 60 ? TipoMantenimiento.Preventivo : TipoMantenimiento.Correctivo;
                    var estado = i <= 20 ? EstadoMantenimiento.Finalizado : (i <= 25 ? EstadoMantenimiento.Progreso : EstadoMantenimiento.Pendiente);
                    var fechaProg = DateTime.UtcNow.AddDays(estado == EstadoMantenimiento.Finalizado ? -random.Next(10, 1000) : random.Next(1, 30));

                    var mant = new Mantenimiento
                    {
                        Id = Guid.NewGuid(),
                        ActivoId = a.Id,
                        TecnicoId = tec.Id,
                        Tipo = tipo,
                        Estado = estado,
                        FechaInicio = fechaProg,
                        FechaFin = estado == EstadoMantenimiento.Finalizado ? fechaProg.AddDays(random.Next(1, 5)) : null,
                        Descripcion = $"Mantenimiento {tipo} {i}",
                        Costo = estado == EstadoMantenimiento.Finalizado ? (decimal)random.Next(5, 500) * 1000 : 0
                    };
                    mantenimientos.Add(mant);

                    // Para algunos, crear programas recurrentes
                    if (tipo == TipoMantenimiento.Preventivo && i <= 10)
                    {
                        var frecuencias = new[] { 30, 90, 180, 365 };
                        programas.Add(new ProgramaMantenimiento
                        {
                            Id = Guid.NewGuid(),
                            ActivoId = a.Id,
                            Titulo = $"Mantenimiento Periódico - {a.Codigo}",
                            FrecuenciaDias = frecuencias[random.Next(frecuencias.Length)],
                            ProximaFecha = DateTime.UtcNow.AddDays(random.Next(10, 60)),
                            Estado = EstadoPrograma.Activo,
                            TecnicoAsignadoId = tec.Id
                        });
                    }
                }
                context.Mantenimientos.AddRange(mantenimientos);
                context.ProgramasMantenimiento.AddRange(programas);
                context.SaveChanges();

                // 9. Auditoría (Ejemplos)
                var auditorias = new List<Auditoria>();
                for (int i = 0; i < 20; i++)
                {
                    auditorias.Add(new Auditoria
                    {
                        Id = Guid.NewGuid(),
                        UsuarioId = adminUser.Id,
                        FechaAccion = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                        Accion = AccionAuditoria.INSERT,
                        TablaAfectada = "Activos",
                        ValoresNuevos = "Auditoría de prueba generada por seeder."
                    });
                }
                context.Auditorias.AddRange(auditorias);
                context.SaveChanges();

                // 10. Notificaciones
                var notificaciones = new List<Notificacion>();
                for(int i=0; i<10; i++)
                {
                    notificaciones.Add(new Notificacion
                    {
                        Id = Guid.NewGuid(),
                        ActivoId = activos[random.Next(activos.Count)].Id,
                        Tipo = TipoNotificacion.VencimientoMantenimiento,
                        Mensaje = "El mantenimiento programado se acerca.",
                        FechaGeneracion = DateTime.UtcNow.AddDays(-random.Next(1, 5)),
                        Leida = false
                    });
                }
                context.Notificaciones.AddRange(notificaciones);
                context.SaveChanges();
            }
        }
    }
}
