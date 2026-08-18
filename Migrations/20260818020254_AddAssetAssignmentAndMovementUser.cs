using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_Gestion_de_Activos.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetAssignmentAndMovementUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "usuario_id",
                table: "movimientos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_usuario_id",
                table: "movimientos",
                column: "usuario_id");

            migrationBuilder.AddForeignKey(
                name: "FK_movimientos_usuarios_usuario_id",
                table: "movimientos",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_movimientos_usuarios_usuario_id",
                table: "movimientos");

            migrationBuilder.DropIndex(
                name: "IX_movimientos_usuario_id",
                table: "movimientos");

            migrationBuilder.DropColumn(
                name: "usuario_id",
                table: "movimientos");
        }
    }
}
