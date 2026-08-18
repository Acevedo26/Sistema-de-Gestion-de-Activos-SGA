using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_Gestion_de_Activos.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    vida_util = table.Column<int>(type: "int", nullable: false),
                    valor_residual = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parametros_sistema",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    valor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parametros_sistema", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ubicaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    edificio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    oficina = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ubicaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    rol_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "activos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    proveedor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    costo_inicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    fecha_adquisicion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    numero_serie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    categoria_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ubicacion_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    responsable_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    fecha_asignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    observaciones_asignacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activos", x => x.id);
                    table.ForeignKey(
                        name: "FK_activos_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_activos_ubicaciones_ubicacion_id",
                        column: x => x.ubicacion_id,
                        principalTable: "ubicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_activos_usuarios_responsable_id",
                        column: x => x.responsable_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auditoria",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    tabla_afectada = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valores_anteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valores_nuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_accion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria", x => x.id);
                    table.ForeignKey(
                        name: "FK_auditoria_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tokens_recuperacion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    token = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    utilizado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens_recuperacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_recuperacion_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "depreciaciones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activo_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    vida_util_asignada = table.Column<int>(type: "int", nullable: false),
                    valor_residual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    valor_actual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    porcentaje_consumido = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    fecha_ultimo_calculo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_depreciaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_depreciaciones_activos_activo_id",
                        column: x => x.activo_id,
                        principalTable: "activos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mantenimientos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activo_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tecnico_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mantenimientos", x => x.id);
                    table.ForeignKey(
                        name: "FK_mantenimientos_activos_activo_id",
                        column: x => x.activo_id,
                        principalTable: "activos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mantenimientos_usuarios_tecnico_id",
                        column: x => x.tecnico_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activo_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ubicacion_origen_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ubicacion_destino_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha_movimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movimientos_activos_activo_id",
                        column: x => x.activo_id,
                        principalTable: "activos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimientos_ubicaciones_ubicacion_destino_id",
                        column: x => x.ubicacion_destino_id,
                        principalTable: "ubicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimientos_ubicaciones_ubicacion_origen_id",
                        column: x => x.ubicacion_origen_id,
                        principalTable: "ubicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "programas_mantenimiento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activo_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    frecuencia_dias = table.Column<int>(type: "int", nullable: false),
                    proxima_fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tecnico_asignado_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programas_mantenimiento", x => x.id);
                    table.ForeignKey(
                        name: "FK_programas_mantenimiento_activos_activo_id",
                        column: x => x.activo_id,
                        principalTable: "activos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_programas_mantenimiento_usuarios_tecnico_asignado_id",
                        column: x => x.tecnico_asignado_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historial_depreciacion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    depreciacion_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha_consulta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    valor_actual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    valor_residual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    porcentaje_consumido = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    depreciacion_acumulada = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_depreciacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_historial_depreciacion_depreciaciones_depreciacion_id",
                        column: x => x.depreciacion_id,
                        principalTable: "depreciaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notificaciones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    activo_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    programa_mantenimiento_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    mensaje = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    fecha_generacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leida = table.Column<bool>(type: "bit", nullable: false),
                    enviada_por_correo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_notificaciones_activos_activo_id",
                        column: x => x.activo_id,
                        principalTable: "activos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notificaciones_programas_mantenimiento_programa_mantenimiento_id",
                        column: x => x.programa_mantenimiento_id,
                        principalTable: "programas_mantenimiento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activos_categoria_id",
                table: "activos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_activos_codigo",
                table: "activos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_activos_responsable_id",
                table: "activos",
                column: "responsable_id");

            migrationBuilder.CreateIndex(
                name: "IX_activos_ubicacion_id",
                table: "activos",
                column: "ubicacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_fecha_accion",
                table: "auditoria",
                column: "fecha_accion");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_tabla_afectada",
                table: "auditoria",
                column: "tabla_afectada");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_usuario_id",
                table: "auditoria",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_categorias_nombre",
                table: "categorias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_depreciaciones_activo_id",
                table: "depreciaciones",
                column: "activo_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_historial_depreciacion_depreciacion_id_fecha_consulta",
                table: "historial_depreciacion",
                columns: new[] { "depreciacion_id", "fecha_consulta" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mantenimientos_activo_id_fecha_inicio",
                table: "mantenimientos",
                columns: new[] { "activo_id", "fecha_inicio" });

            migrationBuilder.CreateIndex(
                name: "IX_mantenimientos_tecnico_id_fecha_inicio",
                table: "mantenimientos",
                columns: new[] { "tecnico_id", "fecha_inicio" });

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_activo_id",
                table: "movimientos",
                column: "activo_id");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_ubicacion_destino_id",
                table: "movimientos",
                column: "ubicacion_destino_id");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_ubicacion_origen_id",
                table: "movimientos",
                column: "ubicacion_origen_id");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_activo_id",
                table: "notificaciones",
                column: "activo_id");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_fecha_generacion",
                table: "notificaciones",
                column: "fecha_generacion");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_programa_mantenimiento_id",
                table: "notificaciones",
                column: "programa_mantenimiento_id");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_tipo_activo_id",
                table: "notificaciones",
                columns: new[] { "tipo", "activo_id" });

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_tipo_programa_mantenimiento_id",
                table: "notificaciones",
                columns: new[] { "tipo", "programa_mantenimiento_id" });

            migrationBuilder.CreateIndex(
                name: "IX_parametros_sistema_clave",
                table: "parametros_sistema",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_programas_mantenimiento_activo_id_proxima_fecha",
                table: "programas_mantenimiento",
                columns: new[] { "activo_id", "proxima_fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_programas_mantenimiento_tecnico_asignado_id",
                table: "programas_mantenimiento",
                column: "tecnico_asignado_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tokens_recuperacion_token",
                table: "tokens_recuperacion",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tokens_recuperacion_usuario_id",
                table: "tokens_recuperacion",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_correo",
                table: "usuarios",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_rol_id",
                table: "usuarios",
                column: "rol_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria");

            migrationBuilder.DropTable(
                name: "historial_depreciacion");

            migrationBuilder.DropTable(
                name: "mantenimientos");

            migrationBuilder.DropTable(
                name: "movimientos");

            migrationBuilder.DropTable(
                name: "notificaciones");

            migrationBuilder.DropTable(
                name: "parametros_sistema");

            migrationBuilder.DropTable(
                name: "tokens_recuperacion");

            migrationBuilder.DropTable(
                name: "depreciaciones");

            migrationBuilder.DropTable(
                name: "programas_mantenimiento");

            migrationBuilder.DropTable(
                name: "activos");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "ubicaciones");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
