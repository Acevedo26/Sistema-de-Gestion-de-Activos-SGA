using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_Gestion_de_Activos.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenRecuperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tokens_recuperacion_usuarios_usuario_id",
                table: "tokens_recuperacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tokens_recuperacion",
                table: "tokens_recuperacion");

            migrationBuilder.RenameTable(
                name: "tokens_recuperacion",
                newName: "TokensRecuperacion");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "TokensRecuperacion",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TokensRecuperacion",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "TokensRecuperacion",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "fecha_expiracion",
                table: "TokensRecuperacion",
                newName: "FechaExpiracion");

            migrationBuilder.RenameColumn(
                name: "utilizado",
                table: "TokensRecuperacion",
                newName: "Usado");

            migrationBuilder.RenameIndex(
                name: "IX_tokens_recuperacion_usuario_id",
                table: "TokensRecuperacion",
                newName: "IX_TokensRecuperacion_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_tokens_recuperacion_token",
                table: "TokensRecuperacion",
                newName: "IX_TokensRecuperacion_Token");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "TokensRecuperacion",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TokensRecuperacion",
                table: "TokensRecuperacion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TokensRecuperacion_usuarios_UsuarioId",
                table: "TokensRecuperacion",
                column: "UsuarioId",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokensRecuperacion_usuarios_UsuarioId",
                table: "TokensRecuperacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TokensRecuperacion",
                table: "TokensRecuperacion");

            migrationBuilder.RenameTable(
                name: "TokensRecuperacion",
                newName: "tokens_recuperacion");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "tokens_recuperacion",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tokens_recuperacion",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "tokens_recuperacion",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "FechaExpiracion",
                table: "tokens_recuperacion",
                newName: "fecha_expiracion");

            migrationBuilder.RenameColumn(
                name: "Usado",
                table: "tokens_recuperacion",
                newName: "utilizado");

            migrationBuilder.RenameIndex(
                name: "IX_TokensRecuperacion_UsuarioId",
                table: "tokens_recuperacion",
                newName: "IX_tokens_recuperacion_usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_TokensRecuperacion_Token",
                table: "tokens_recuperacion",
                newName: "IX_tokens_recuperacion_token");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "tokens_recuperacion",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tokens_recuperacion",
                table: "tokens_recuperacion",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tokens_recuperacion_usuarios_usuario_id",
                table: "tokens_recuperacion",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
