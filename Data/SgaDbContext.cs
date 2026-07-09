using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Domain.Entities;

namespace Sistema_de_Gestion_de_Activos.Data
{
    /// <summary>
    /// Contexto principal de base de datos para el Sistema de Gestión de Mantenimiento de Activos (SGA).
    /// Configuración Code First con Fluent API para SQL Server.
    /// </summary>
    public class SgaDbContext : DbContext
    {
        public SgaDbContext(DbContextOptions<SgaDbContext> options) : base(options)
        {
        }

        // ──────────────────────────────────────────────
        // DbSets (Tablas)
        // ──────────────────────────────────────────────
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TokenRecuperacion> TokensRecuperacion { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<ParametroSistema> ParametrosSistema { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Activo> Activos { get; set; }
        public DbSet<Depreciacion> Depreciaciones { get; set; }
        public DbSet<HistorialDepreciacion> HistorialDepreciaciones { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<ProgramaMantenimiento> ProgramasMantenimiento { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ══════════════════════════════════════════
            // 1. CONVERSIÓN DE ENUMS A STRING
            //    Persiste los enums como nvarchar legible
            //    en lugar de enteros.
            // ══════════════════════════════════════════
            modelBuilder.Entity<Rol>()
                .Property(e => e.Nombre)
                .HasConversion<string>();

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Auditoria>()
                .Property(e => e.Accion)
                .HasConversion<string>();

            modelBuilder.Entity<Activo>()
                .Property(e => e.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Mantenimiento>()
                .Property(e => e.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<Mantenimiento>()
                .Property(e => e.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<ProgramaMantenimiento>()
                .Property(e => e.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Notificacion>()
                .Property(e => e.Tipo)
                .HasConversion<string>();

            // ══════════════════════════════════════════
            // 2. ÍNDICES Y RESTRICCIONES DE UNICIDAD
            // ══════════════════════════════════════════
            modelBuilder.Entity<Rol>()
                .HasIndex(r => r.Nombre)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            modelBuilder.Entity<TokenRecuperacion>()
                .HasIndex(t => t.Token)
                .IsUnique();

            modelBuilder.Entity<Auditoria>()
                .HasIndex(a => new { a.NombreTabla, a.RegistroId });
            modelBuilder.Entity<Auditoria>()
                .HasIndex(a => a.Fecha);

            modelBuilder.Entity<ParametroSistema>()
                .HasIndex(p => p.Clave)
                .IsUnique();

            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Nombre)
                .IsUnique();

            modelBuilder.Entity<Activo>()
                .HasIndex(a => a.Codigo)
                .IsUnique();

            modelBuilder.Entity<HistorialDepreciacion>()
                .HasIndex(h => new { h.DepreciacionId, h.FechaConsulta })
                .IsUnique();

            modelBuilder.Entity<Movimiento>()
                .HasIndex(m => new { m.ActivoId, m.FechaMovimiento });

            modelBuilder.Entity<Mantenimiento>()
                .HasIndex(m => new { m.ActivoId, m.FechaInicio });
            modelBuilder.Entity<Mantenimiento>()
                .HasIndex(m => new { m.TecnicoId, m.FechaInicio });

            modelBuilder.Entity<ProgramaMantenimiento>()
                .HasIndex(p => new { p.ActivoId, p.ProximaFecha });

            modelBuilder.Entity<Notificacion>()
                .HasIndex(n => new { n.Tipo, n.ActivoId });
            modelBuilder.Entity<Notificacion>()
                .HasIndex(n => new { n.Tipo, n.ProgramaMantenimientoId });
            modelBuilder.Entity<Notificacion>()
                .HasIndex(n => n.FechaGeneracion);

            // ══════════════════════════════════════════
            // 3. RELACIONES (18 FK del DBML)
            //    Todas con Restrict para evitar
            //    Multiple Cascade Paths en SQL Server,
            //    excepto la 1:1 Activo→Depreciación.
            // ══════════════════════════════════════════

            // ── Relación 1: Usuario → Rol (Muchos a Uno) ──
            // DBML: usuarios.rol_id → roles.id
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 2: TokenRecuperacion → Usuario (Muchos a Uno) ──
            // DBML: tokens_recuperacion.usuario_id → usuarios.id
            modelBuilder.Entity<TokenRecuperacion>()
                .HasOne(t => t.Usuario)
                .WithMany()
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 3: Auditoria → Usuario (Muchos a Uno) ──
            // DBML: auditoria.usuario_id → usuarios.id
            modelBuilder.Entity<Auditoria>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 4: Activo → Categoria (Muchos a Uno) ──
            // DBML: activos.categoria_id → categorias.id
            modelBuilder.Entity<Activo>()
                .HasOne(a => a.Categoria)
                .WithMany()
                .HasForeignKey(a => a.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 5: Activo → Ubicacion (Muchos a Uno) ──
            // DBML: activos.ubicacion_id → ubicaciones.id
            modelBuilder.Entity<Activo>()
                .HasOne(a => a.Ubicacion)
                .WithMany()
                .HasForeignKey(a => a.UbicacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 6: Activo ↔ Depreciacion (Uno a Uno) ──
            // DBML: depreciaciones.activo_id → activos.id (unique, 1:1 estricta)
            modelBuilder.Entity<Activo>()
                .HasOne(a => a.Depreciacion)
                .WithOne(d => d.Activo)
                .HasForeignKey<Depreciacion>(d => d.ActivoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Relación 7: HistorialDepreciacion → Depreciacion (Muchos a Uno) ──
            // DBML: historial_depreciacion.depreciacion_id → depreciaciones.id
            modelBuilder.Entity<HistorialDepreciacion>()
                .HasOne(h => h.Depreciacion)
                .WithMany(d => d.Historial)
                .HasForeignKey(h => h.DepreciacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 8: Movimiento → Activo (Muchos a Uno) ──
            // DBML: movimientos.activo_id → activos.id
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.Activo)
                .WithMany()
                .HasForeignKey(m => m.ActivoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 9: Movimiento → Usuario Operador (Muchos a Uno) ──
            // DBML: movimientos.usuario_operador_id → usuarios.id
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.UsuarioOperador)
                .WithMany()
                .HasForeignKey(m => m.UsuarioOperadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 10: Movimiento → Responsable Anterior (Muchos a Uno, Nullable) ──
            // DBML: movimientos.responsable_anterior_id → usuarios.id (opcional)
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.ResponsableAnterior)
                .WithMany()
                .HasForeignKey(m => m.ResponsableAnteriorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 11: Movimiento → Responsable Nuevo (Muchos a Uno) ──
            // DBML: movimientos.responsable_nuevo_id → usuarios.id
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.ResponsableNuevo)
                .WithMany()
                .HasForeignKey(m => m.ResponsableNuevoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 12: Movimiento → Ubicación Anterior (Muchos a Uno, Nullable) ──
            // DBML: movimientos.ubicacion_anterior_id → ubicaciones.id (opcional)
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.UbicacionAnterior)
                .WithMany()
                .HasForeignKey(m => m.UbicacionAnteriorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 13: Movimiento → Ubicación Nueva (Muchos a Uno) ──
            // DBML: movimientos.ubicacion_nueva_id → ubicaciones.id
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.UbicacionNueva)
                .WithMany()
                .HasForeignKey(m => m.UbicacionNuevaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 14: Mantenimiento → Activo (Muchos a Uno) ──
            // DBML: mantenimientos.activo_id → activos.id
            modelBuilder.Entity<Mantenimiento>()
                .HasOne(m => m.Activo)
                .WithMany()
                .HasForeignKey(m => m.ActivoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 15: Mantenimiento → Técnico (Muchos a Uno) ──
            // DBML: mantenimientos.tecnico_id → usuarios.id
            modelBuilder.Entity<Mantenimiento>()
                .HasOne(m => m.Tecnico)
                .WithMany()
                .HasForeignKey(m => m.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 16: ProgramaMantenimiento → Activo (Muchos a Uno) ──
            // DBML: programas_mantenimiento.activo_id → activos.id
            modelBuilder.Entity<ProgramaMantenimiento>()
                .HasOne(p => p.Activo)
                .WithMany()
                .HasForeignKey(p => p.ActivoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 17: Notificacion → Activo (Muchos a Uno, Nullable) ──
            // DBML: notificaciones.activo_id → activos.id (opcional)
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Activo)
                .WithMany()
                .HasForeignKey(n => n.ActivoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relación 18: Notificacion → ProgramaMantenimiento (Muchos a Uno, Nullable) ──
            // DBML: notificaciones.programa_mantenimiento_id → programas_mantenimiento.id (opcional)
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.ProgramaMantenimiento)
                .WithMany()
                .HasForeignKey(n => n.ProgramaMantenimientoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
