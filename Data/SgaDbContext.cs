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
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<TokenRecuperacion> TokensRecuperacion { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Activo> Activos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Depreciacion> Depreciaciones { get; set; }
        public DbSet<HistorialDepreciacion> HistorialDepreciaciones { get; set; }
        public DbSet<ParametroSistema> ParametrosSistema { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<ProgramaMantenimiento> ProgramasMantenimiento { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ══════════════════════════════════════════
            // 1. CONVERSIÓN DE ENUMS A STRING
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

            modelBuilder.Entity<Notificacion>()
                .Property(e => e.Tipo)
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
                .HasIndex(a => a.TablaAfectada);
            modelBuilder.Entity<Auditoria>()
                .HasIndex(a => a.FechaAccion);

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

            modelBuilder.Entity<Notificacion>()
                .HasIndex(n => new { n.Tipo, n.ActivoId });
            modelBuilder.Entity<Notificacion>()
                .HasIndex(n => new { n.Tipo, n.ProgramaMantenimientoId });
            modelBuilder.Entity<Notificacion>()
                .HasIndex(n => n.FechaGeneracion);

			modelBuilder.Entity<Mantenimiento>()
	            .HasIndex(m => new { m.ActivoId, m.FechaInicio });
			modelBuilder.Entity<Mantenimiento>()
				.HasIndex(m => new { m.TecnicoId, m.FechaInicio });

			modelBuilder.Entity<ProgramaMantenimiento>()
				.HasIndex(p => new { p.ActivoId, p.ProximaFecha });

			// ══════════════════════════════════════════
			// 3. RELACIONES
			// ══════════════════════════════════════════

			modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TokenRecuperacion>()
                .HasOne(t => t.Usuario)
                .WithMany()
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Auditoria>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Activo>()
                .HasOne(a => a.Categoria)
                .WithMany()
                .HasForeignKey(a => a.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Activo>()
                .HasOne(a => a.Ubicacion)
                .WithMany()
                .HasForeignKey(a => a.UbicacionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Activo>()
                .HasOne(a => a.Depreciacion)
                .WithOne(d => d.Activo)
                .HasForeignKey<Depreciacion>(d => d.ActivoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistorialDepreciacion>()
                .HasOne(h => h.Depreciacion)
                .WithMany(d => d.Historial)
                .HasForeignKey(h => h.DepreciacionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Activo)
                .WithMany()
                .HasForeignKey(n => n.ActivoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.ProgramaMantenimiento)
                .WithMany()
                .HasForeignKey(n => n.ProgramaMantenimientoId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Mantenimiento>()
                .HasOne(m => m.Activo)
                .WithMany()
                .HasForeignKey(m => m.ActivoId)
                .OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Mantenimiento>()
				.HasOne(m => m.Tecnico)
				.WithMany()
				.HasForeignKey(m => m.TecnicoId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ProgramaMantenimiento>()
				.HasOne(p => p.Activo)
				.WithMany()
				.HasForeignKey(p => p.ActivoId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ProgramaMantenimiento>()
				.HasOne(p => p.TecnicoAsignado)
				.WithMany()
				.HasForeignKey(p => p.TecnicoAsignadoId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Movimiento>()
				.HasOne(m => m.Activo)
				.WithMany()
				.HasForeignKey(m => m.ActivoId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Movimiento>()
				.HasOne(m => m.UbicacionOrigen)
				.WithMany()
				.HasForeignKey(m => m.UbicacionOrigenId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Movimiento>()
				.HasOne(m => m.UbicacionDestino)
				.WithMany()
				.HasForeignKey(m => m.UbicacionDestinoId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
