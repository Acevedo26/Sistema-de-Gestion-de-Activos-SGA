import os

base_dir = r"c:\Users\josea\source\repos\Sistema-de-Gestion-de-Activos"

enums_dir = os.path.join(base_dir, "Domain", "Enums")
entities_dir = os.path.join(base_dir, "Domain", "Entities")
data_dir = os.path.join(base_dir, "Data")

os.makedirs(enums_dir, exist_ok=True)
os.makedirs(entities_dir, exist_ok=True)
os.makedirs(data_dir, exist_ok=True)

enums = {
    "EstadoActivo": ["Activo", "Mantenimiento", "Inactivo", "Baja", "Descartado"],
    "NombreRol": ["Administrador", "Gestor", "Tecnico", "Visualizador"],
    "EstadoUsuario": ["Activo", "Inactivo"],
    "TipoMantenimiento": ["Preventivo", "Correctivo"],
    "EstadoMantenimiento": ["Pendiente", "EnProgreso", "Finalizado"],
    "EstadoPrograma": ["Activo", "Suspendido"],
    "AccionAuditoria": ["INSERT", "UPDATE", "DELETE"],
    "TipoNotificacion": ["VencimientoMantenimiento", "DepreciacionProxima", "CostoMantenimientoExcesivo"]
}

for enum_name, values in enums.items():
    content = f"""namespace Sistema_de_Gestion_de_Activos.Domain.Enums
{{
    /// <summary>
    /// Representa los valores posibles para {enum_name}.
    /// Se persiste como cadena (string) en la base de datos para mayor claridad y mantenimiento.
    /// </summary>
    public enum {enum_name}
    {{
"""
    for val in values:
        if val == "EnProgreso":
            content += f"        // En dbml era 'En Progreso', en código se usa EnProgreso\n        EnProgreso,\n"
        else:
            content += f"        {val},\n"
    content += """    }
}
"""
    with open(os.path.join(enums_dir, f"{enum_name}.cs"), "w", encoding="utf-8") as f:
        f.write(content)

entities = {
    "Rol": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Catálogo de roles del sistema (RF-16, RF-17).
    /// </summary>
    [Table("roles")]
    public class Rol
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Administrador, Gestor, Tecnico, Visualizador
        /// </summary>
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [Column("nombre")]
        public NombreRol Nombre { get; set; }

        // Navegación
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
""",
    "Usuario": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Cuentas de acceso al sistema (RF-16).
    /// </summary>
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [MaxLength(150)]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Column("correo")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El hash de la contraseña es obligatorio.")]
        [MaxLength(300)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("rol_id")]
        public Guid RolId { get; set; }

        [Required]
        [Column("estado")]
        public EstadoUsuario Estado { get; set; } = EstadoUsuario.Activo;

        // Navegación
        [ForeignKey(nameof(RolId))]
        public Rol Rol { get; set; } = null!;
    }
}
""",
    "TokenRecuperacion": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Tokens de recuperación de contraseña, de un solo uso (RF-19).
    /// </summary>
    [Table("tokens_recuperacion")]
    public class TokenRecuperacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("token")]
        public string Token { get; set; } = string.Empty;

        [Required]
        [Column("fecha_expiracion")]
        public DateTime FechaExpiracion { get; set; }

        [Required]
        [Column("utilizado")]
        public bool Utilizado { get; set; } = false;

        // Navegación
        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;
    }
}
""",
    "Auditoria": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Registro inmutable de operaciones críticas (RF-03, RF-18).
    /// </summary>
    [Table("auditoria")]
    public class Auditoria
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Required]
        [Column("accion")]
        public AccionAuditoria Accion { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre_tabla")]
        public string NombreTabla { get; set; } = string.Empty;

        [Required]
        [Column("registro_id")]
        public Guid RegistroId { get; set; }

        /// <summary>
        /// JSON, nulo en INSERT
        /// </summary>
        [Column("valores_anteriores", TypeName = "nvarchar(max)")]
        public string? ValoresAnteriores { get; set; }

        /// <summary>
        /// JSON, nulo en DELETE
        /// </summary>
        [Column("valores_nuevos", TypeName = "nvarchar(max)")]
        public string? ValoresNuevos { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; }

        // Navegación
        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;
    }
}
""",
    "ParametroSistema": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Umbrales configurables del sistema (RF-10, RF-15, RF-28, RF-29).
    /// </summary>
    [Table("parametros_sistema")]
    public class ParametroSistema
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("clave")]
        public string Clave { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("valor")]
        public string Valor { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Required]
        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; }
    }
}
""",
    "Categoria": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Catálogo de categorías de activos y su política contable (RF-20).
    /// </summary>
    [Table("categorias")]
    public class Categoria
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Años de vida útil vigentes. (1-100)
        /// </summary>
        [Required]
        [Column("vida_util")]
        [Range(1, 100, ErrorMessage = "La vida útil debe estar entre 1 y 100 años.")]
        public int VidaUtil { get; set; }

        /// <summary>
        /// Valor residual por defecto para activos nuevos.
        /// </summary>
        [Required]
        [Column("valor_residual", TypeName = "decimal(18,2)")]
        public decimal ValorResidual { get; set; } = 0m;
    }
}
""",
    "Ubicacion": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Jerarquía física de ubicación de los activos (RF-06).
    /// </summary>
    [Table("ubicaciones")]
    public class Ubicacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("departamento")]
        public string Departamento { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("oficina")]
        public string? Oficina { get; set; }

        [MaxLength(20)]
        [Column("piso")]
        public string? Piso { get; set; }

        [MaxLength(100)]
        [Column("area")]
        public string? Area { get; set; }
    }
}
""",
    "Activo": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Registro principal transaccional de cada bien físico (RF-01 a RF-04).
    /// </summary>
    [Table("activos")]
    public class Activo
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("proveedor")]
        public string? Proveedor { get; set; }

        [Required]
        [Column("costo_inicial", TypeName = "decimal(18,2)")]
        public decimal CostoInicial { get; set; }

        [Required]
        [Column("fecha_adquisicion")]
        public DateTime FechaAdquisicion { get; set; }

        [MaxLength(100)]
        [Column("numero_serie")]
        public string? NumeroSerie { get; set; }

        [Required]
        [Column("estado")]
        public EstadoActivo Estado { get; set; } = EstadoActivo.Activo;

        [Required]
        [Column("categoria_id")]
        public Guid CategoriaId { get; set; }

        [Required]
        [Column("ubicacion_id")]
        public Guid UbicacionId { get; set; }

        // Navegación
        [ForeignKey(nameof(CategoriaId))]
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey(nameof(UbicacionId))]
        public Ubicacion Ubicacion { get; set; } = null!;

        public Depreciacion? Depreciacion { get; set; }
    }
}
""",
    "Depreciacion": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Estado financiero vigente e instantáneo del activo, método de línea recta (RF-21).
    /// </summary>
    [Table("depreciaciones")]
    public class Depreciacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("activo_id")]
        public Guid ActivoId { get; set; }

        [Required]
        [Column("vida_util_asignada")]
        public int VidaUtilAsignada { get; set; }

        [Required]
        [Column("valor_residual", TypeName = "decimal(18,2)")]
        public decimal ValorResidual { get; set; } = 0m;

        [Required]
        [Column("valor_actual", TypeName = "decimal(18,2)")]
        public decimal ValorActual { get; set; }

        [Required]
        [Column("porcentaje_consumido", TypeName = "decimal(5,2)")]
        public decimal PorcentajeConsumido { get; set; } = 0m;

        [Required]
        [Column("fecha_ultimo_calculo")]
        public DateTime FechaUltimoCalculo { get; set; }

        // Navegación
        [ForeignKey(nameof(ActivoId))]
        public Activo Activo { get; set; } = null!;

        public ICollection<HistorialDepreciacion> Historial { get; set; } = new List<HistorialDepreciacion>();
    }
}
""",
    "HistorialDepreciacion": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Snapshots mensuales, solo inserción (append-only), para auditoría contable (RF-24).
    /// </summary>
    [Table("historial_depreciacion")]
    public class HistorialDepreciacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("depreciacion_id")]
        public Guid DepreciacionId { get; set; }

        [Required]
        [Column("fecha_consulta")]
        public DateTime FechaConsulta { get; set; }

        [Required]
        [Column("valor_actual", TypeName = "decimal(18,2)")]
        public decimal ValorActual { get; set; }

        [Required]
        [Column("valor_residual", TypeName = "decimal(18,2)")]
        public decimal ValorResidual { get; set; }

        [Required]
        [Column("porcentaje_consumido", TypeName = "decimal(5,2)")]
        public decimal PorcentajeConsumido { get; set; }

        [Required]
        [Column("depreciacion_acumulada", TypeName = "decimal(18,2)")]
        public decimal DepreciacionAcumulada { get; set; }

        // Navegación
        [ForeignKey(nameof(DepreciacionId))]
        public Depreciacion Depreciacion { get; set; } = null!;
    }
}
""",
    "Movimiento": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Historial inmutable de asignaciones y traslados físicos (RF-05 a RF-07).
    /// </summary>
    [Table("movimientos")]
    public class Movimiento
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("activo_id")]
        public Guid ActivoId { get; set; }

        [Required]
        [Column("usuario_operador_id")]
        public Guid UsuarioOperadorId { get; set; }

        [Column("responsable_anterior_id")]
        public Guid? ResponsableAnteriorId { get; set; }

        [Required]
        [Column("responsable_nuevo_id")]
        public Guid ResponsableNuevoId { get; set; }

        [Column("ubicacion_anterior_id")]
        public Guid? UbicacionAnteriorId { get; set; }

        [Required]
        [Column("ubicacion_nueva_id")]
        public Guid UbicacionNuevaId { get; set; }

        [Required]
        [Column("fecha_movimiento")]
        public DateTime FechaMovimiento { get; set; }

        [MaxLength(500)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        // Navegación
        [ForeignKey(nameof(ActivoId))]
        public Activo Activo { get; set; } = null!;

        [ForeignKey(nameof(UsuarioOperadorId))]
        public Usuario UsuarioOperador { get; set; } = null!;

        [ForeignKey(nameof(ResponsableAnteriorId))]
        public Usuario? ResponsableAnterior { get; set; }

        [ForeignKey(nameof(ResponsableNuevoId))]
        public Usuario ResponsableNuevo { get; set; } = null!;

        [ForeignKey(nameof(UbicacionAnteriorId))]
        public Ubicacion? UbicacionAnterior { get; set; }

        [ForeignKey(nameof(UbicacionNuevaId))]
        public Ubicacion UbicacionNueva { get; set; } = null!;
    }
}
""",
    "Mantenimiento": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Intervenciones preventivas y correctivas realizadas sobre los activos (RF-08, RF-11).
    /// </summary>
    [Table("mantenimientos")]
    public class Mantenimiento
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("activo_id")]
        public Guid ActivoId { get; set; }

        [Required]
        [Column("tecnico_id")]
        public Guid TecnicoId { get; set; }

        [Required]
        [Column("tipo")]
        public TipoMantenimiento Tipo { get; set; }

        [Required]
        [Column("estado")]
        public EstadoMantenimiento Estado { get; set; } = EstadoMantenimiento.Pendiente;

        [Required]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Required]
        [Column("costo", TypeName = "decimal(18,2)")]
        public decimal Costo { get; set; } = 0m;

        [Required]
        [MaxLength(500)]
        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        // Navegación
        [ForeignKey(nameof(ActivoId))]
        public Activo Activo { get; set; } = null!;

        [ForeignKey(nameof(TecnicoId))]
        public Usuario Tecnico { get; set; } = null!;
    }
}
""",
    "ProgramaMantenimiento": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Cronogramas recurrentes de mantenimiento preventivo (RF-09).
    /// </summary>
    [Table("programas_mantenimiento")]
    public class ProgramaMantenimiento
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("activo_id")]
        public Guid ActivoId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [Column("frecuencia_dias")]
        [Range(1, 3650)]
        public int FrecuenciaDias { get; set; }

        [Required]
        [Column("proxima_fecha")]
        public DateTime ProximaFecha { get; set; }

        [Required]
        [Column("estado")]
        public EstadoPrograma Estado { get; set; } = EstadoPrograma.Activo;

        // Navegación
        [ForeignKey(nameof(ActivoId))]
        public Activo Activo { get; set; } = null!;
    }
}
""",
    "Notificacion": """using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    /// <summary>
    /// Alertas de vencimiento de mantenimiento, depreciación próxima y costo excesivo (RF-10, RF-28, RF-29).
    /// </summary>
    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("tipo")]
        public TipoNotificacion Tipo { get; set; }

        [Column("activo_id")]
        public Guid? ActivoId { get; set; }

        [Column("programa_mantenimiento_id")]
        public Guid? ProgramaMantenimientoId { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [Column("fecha_generacion")]
        public DateTime FechaGeneracion { get; set; }

        [Required]
        [Column("leida")]
        public bool Leida { get; set; } = false;

        [Required]
        [Column("enviada_por_correo")]
        public bool EnviadaPorCorreo { get; set; } = false;

        // Navegación
        [ForeignKey(nameof(ActivoId))]
        public Activo? Activo { get; set; }

        [ForeignKey(nameof(ProgramaMantenimientoId))]
        public ProgramaMantenimiento? ProgramaMantenimiento { get; set; }
    }
}
"""
}

for entity_name, content in entities.items():
    with open(os.path.join(entities_dir, f"{entity_name}.cs"), "w", encoding="utf-8") as f:
        f.write(content)

dbcontext_content = """using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Domain.Entities;

namespace Sistema_de_Gestion_de_Activos.Data
{
    /// <summary>
    /// Contexto principal de base de datos para el Sistema de Gestión de Mantenimiento de Activos (SGA).
    /// </summary>
    public class SgaDbContext : DbContext
    {
        public SgaDbContext(DbContextOptions<SgaDbContext> options) : base(options)
        {
        }

        // DbSets (Tablas)
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

            // 1. Configuraciones de Enums a string (Para legibilidad en DB)
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

            // 2. Configuraciones de Índices y Unicidad (Fluent API complemento de Data Annotations)
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

            // 3. Relaciones Complejas (One-to-One y Múltiples referencias al mismo Entity)
            
            // Activo -> Depreciacion (1:1)
            modelBuilder.Entity<Activo>()
                .HasOne(a => a.Depreciacion)
                .WithOne(d => d.Activo)
                .HasForeignKey<Depreciacion>(d => d.ActivoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Movimiento -> Usuarios (Múltiples relaciones a la misma tabla)
            // Evitar Multiple Cascade Paths
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.UsuarioOperador)
                .WithMany()
                .HasForeignKey(m => m.UsuarioOperadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.ResponsableAnterior)
                .WithMany()
                .HasForeignKey(m => m.ResponsableAnteriorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.ResponsableNuevo)
                .WithMany()
                .HasForeignKey(m => m.ResponsableNuevoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.UbicacionAnterior)
                .WithMany()
                .HasForeignKey(m => m.UbicacionAnteriorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.UbicacionNueva)
                .WithMany()
                .HasForeignKey(m => m.UbicacionNuevaId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Mantenimiento>()
                .HasOne(m => m.Tecnico)
                .WithMany()
                .HasForeignKey(m => m.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
"""

with open(os.path.join(data_dir, "SgaDbContext.cs"), "w", encoding="utf-8") as f:
    f.write(dbcontext_content)

print("Files generated successfully.")
