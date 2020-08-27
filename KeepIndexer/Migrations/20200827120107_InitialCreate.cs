using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KeepIndexer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    Address = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    LastBlock = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "Op",
                columns: table => new
                {
                    Tx = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ContractAddress = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Block = table.Column<ulong>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    TDT_ID = table.Column<string>(nullable: true),
                    TDTUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Op", x => x.Tx);
                    table.ForeignKey(
                        name: "FK_Op_Contract_ContractAddress",
                        column: x => x.ContractAddress,
                        principalTable: "Contract",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Op_ContractAddress",
                table: "Op",
                column: "ContractAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Op");

            migrationBuilder.DropTable(
                name: "Contract");
        }
    }
}
