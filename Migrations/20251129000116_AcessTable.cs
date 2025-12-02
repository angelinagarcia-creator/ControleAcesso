using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CondoHub.Migrations
{
    /// <inheritdoc />
    public partial class AcessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Access",
                columns: table => new
                {
                    AcessoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroApartamento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InformacaoVisitante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataHoraAcesso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoAcesso = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Access", x => x.AcessoId);
                    table.ForeignKey(
                        name: "FK_Access_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Access_UserId",
                table: "Access",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Access");
        }
    }
}
