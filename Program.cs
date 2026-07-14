using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sistema_de_Gestion_de_Activos.Data;
using System.Text;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Services;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;
using Sistema_de_Gestion_de_Activos.Repositories.Implementations;
using Sistema_de_Gestion_de_Activos.Domain.Enums;
using Sistema_de_Gestion_de_Activos.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5000") });
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAntiforgery();

// 1. Inyectar HttpContextAccessor y el Interceptor de Auditoría
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Sistema_de_Gestion_de_Activos.Data.Interceptors.AuditoriaInterceptor>();

// 2. Configurar DbContext con SQL Server e Interceptors
builder.Services.AddDbContext<SgaDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<Sistema_de_Gestion_de_Activos.Data.Interceptors.AuditoriaInterceptor>();
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(interceptor);
});

// 3. Inyección de Dependencias (Repositorios y Servicios)
// Repositorio genérico 
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
// Repositorio y Servicio específicos de Usuario
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Módulo 1 - Gestión de Activos
builder.Services.AddScoped<IActivoService, ActivoService>();

// Módulo 2 - Movimientos
builder.Services.AddScoped<IMovimientoService, MovimientoService>();

// Servicios de Autenticación, Correo y Auditoría
builder.Services.AddScoped<IEmailService, EmailServiceMock>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

// Servicios del Módulo 6 — Depreciación
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IDepreciacionService, DepreciacionService>();
builder.Services.AddScoped<IHistorialDepreciacionService, HistorialDepreciacionService>();
builder.Services.AddScoped<IReporteDepreciacionService, ReporteDepreciacionService>();
// builder.Services.AddScoped<IExportacionContableService, ExportacionContableService>();
builder.Services.AddScoped<IAnalisisIntegradoService, AnalisisIntegradoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 3. Configurar Swagger para soportar JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SGA API", Version = "v1" });

    // Definición de seguridad JWT para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 4. Configurar Autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Solo para desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey!)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Elimina el margen de 5 min por defecto para la expiración
    };
});

// Registrar Custom AuthenticationStateProvider para Blazor
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, Sistema_de_Gestion_de_Activos.Providers.CustomAuthStateProvider>();

// 4.1 Configurar Políticas de Autorización basadas en Roles (RF-17)
builder.Services.AddAuthorization(options =>
{
    // Política: Creación/Edición/Baja de activos (Gestor y Admin)
    options.AddPolicy("ModificarActivosPolicy", policy => 
        policy.RequireRole(nameof(NombreRol.Administrador), nameof(NombreRol.Gestor)));

    // Política: Gestión de mantenimientos (Técnico, Gestor, Admin)
    options.AddPolicy("ModificarMantenimientosPolicy", policy => 
        policy.RequireRole(nameof(NombreRol.Administrador), nameof(NombreRol.Gestor), nameof(NombreRol.Tecnico)));

    // Política: Solo lectura general (Todos los roles)
    options.AddPolicy("SoloLecturaPolicy", policy => 
        policy.RequireRole(nameof(NombreRol.Administrador), nameof(NombreRol.Gestor), nameof(NombreRol.Tecnico), nameof(NombreRol.Visualizador)));
});

// Background Jobs — Módulo 6
builder.Services.AddHostedService<DepreciacionDailyJob>();
builder.Services.AddHostedService<SnapshotMensualJob>();

var app = builder.Build();

// 5. Migración de la Base de Datos al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SgaDbContext>();
    dbContext.Database.EnsureCreated();
    
    DbInitializer.Initialize(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGA API v1"));
}

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<Sistema_de_Gestion_de_Activos.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
