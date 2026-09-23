using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seguradora.api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarApolicesESinistros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apolices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SeguradoId = table.Column<string>(type: "TEXT", nullable: false),
                    Inicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VigenciaEmMeses = table.Column<int>(type: "INTEGER", nullable: false),
                    Situacao = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ValorSegurado = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    AnoFabricacao = table.Column<int>(type: "INTEGER", nullable: true),
                    AreaConstruida = table.Column<decimal>(type: "TEXT", nullable: true),
                    PossuiAlarme = table.Column<bool>(type: "INTEGER", nullable: true),
                    Fumante = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apolices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apolices_Segurados_SeguradoId",
                        column: x => x.SeguradoId,
                        principalTable: "Segurados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Coberturas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LimiteIndenizacao = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ApoliceId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coberturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coberturas_Apolices_ApoliceId",
                        column: x => x.ApoliceId,
                        principalTable: "Apolices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sinistros",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ApoliceId = table.Column<string>(type: "TEXT", nullable: false),
                    TipoOcorrencia = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    DataOcorrencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ValorPleiteado = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Situacao = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MotivoNegativa = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sinistros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sinistros_Apolices_ApoliceId",
                        column: x => x.ApoliceId,
                        principalTable: "Apolices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apolices_SeguradoId",
                table: "Apolices",
                column: "SeguradoId");

            migrationBuilder.CreateIndex(
                name: "IX_Coberturas_ApoliceId",
                table: "Coberturas",
                column: "ApoliceId");

            migrationBuilder.CreateIndex(
                name: "IX_Sinistros_ApoliceId",
                table: "Sinistros",
                column: "ApoliceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coberturas");

            migrationBuilder.DropTable(
                name: "Sinistros");

            migrationBuilder.DropTable(
                name: "Apolices");
        }
    }
}
