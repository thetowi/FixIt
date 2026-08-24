using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace FixIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CambiarUbicacionAGeography : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "UbicacionGeo",
                table: "Usuarios",
                type: "geography (point)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "UbicacionGeo",
                table: "Usuarios",
                type: "geometry",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography (point)",
                oldNullable: true);
        }
    }
}
