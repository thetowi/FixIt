using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RediseñarChatConConversaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Ordenes_OrdenId",
                table: "Mensajes");

            migrationBuilder.AddColumn<Guid>(
                name: "ConversacionId",
                table: "Ordenes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenId",
                table: "Mensajes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Contenido",
                table: "Mensajes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "ConversacionId",
                table: "Mensajes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Mensajes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoOferta",
                table: "Mensajes",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OfertaVigente",
                table: "Mensajes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Mensajes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Conversaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrestadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    CreadoEn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversaciones_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversaciones_Usuarios_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversaciones_Usuarios_PrestadorId",
                        column: x => x.PrestadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_ConversacionId",
                table: "Ordenes",
                column: "ConversacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_ConversacionId",
                table: "Mensajes",
                column: "ConversacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_CategoriaId",
                table: "Conversaciones",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_ClienteId_PrestadorId_CategoriaId",
                table: "Conversaciones",
                columns: new[] { "ClienteId", "PrestadorId", "CategoriaId" });

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_PrestadorId",
                table: "Conversaciones",
                column: "PrestadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Conversaciones_ConversacionId",
                table: "Mensajes",
                column: "ConversacionId",
                principalTable: "Conversaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Ordenes_OrdenId",
                table: "Mensajes",
                column: "OrdenId",
                principalTable: "Ordenes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_Conversaciones_ConversacionId",
                table: "Ordenes",
                column: "ConversacionId",
                principalTable: "Conversaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Conversaciones_ConversacionId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Ordenes_OrdenId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_Conversaciones_ConversacionId",
                table: "Ordenes");

            migrationBuilder.DropTable(
                name: "Conversaciones");

            migrationBuilder.DropIndex(
                name: "IX_Ordenes_ConversacionId",
                table: "Ordenes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_ConversacionId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "ConversacionId",
                table: "Ordenes");

            migrationBuilder.DropColumn(
                name: "ConversacionId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "MontoOferta",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "OfertaVigente",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Mensajes");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenId",
                table: "Mensajes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Contenido",
                table: "Mensajes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Ordenes_OrdenId",
                table: "Mensajes",
                column: "OrdenId",
                principalTable: "Ordenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
