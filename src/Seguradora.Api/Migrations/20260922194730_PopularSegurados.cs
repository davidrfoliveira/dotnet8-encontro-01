using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Seguradora.api.Migrations
{
    /// <inheritdoc />
    public partial class PopularSegurados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Segurados",
                columns: new[] { "Id", "DataCadastro", "DataNascimento", "Cpf", "Nome" },
                values: new object[,]
                {
                    { "SEG00001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1988, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "52998224725", "Maria Silva" },
                    { "SEG00002", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2004, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "11144477735", "João Souza" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Segurados",
                keyColumn: "Id",
                keyValue: "SEG00001");

            migrationBuilder.DeleteData(
                table: "Segurados",
                keyColumn: "Id",
                keyValue: "SEG00002");
        }
    }
}
