using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diploma.Apt.Warehouse.Core.Migrations
{
    public partial class statusmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Stocks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "now()::timestamp(0) at time zone 'utc'",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Stocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "now()::timestamp(0) at time zone 'utc'",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Batches",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "now()::timestamp(0) at time zone 'utc'",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Batches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_DepartmentId",
                table: "Stocks",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_OrganizationId",
                table: "Stocks",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Status",
                table: "Stocks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SearchVector",
                table: "Products",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_Status",
                table: "Batches",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stocks_DepartmentId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_OrganizationId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_Status",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Products_SearchVector",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Batches_Status",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Batches");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Stocks",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "now()::timestamp(0) at time zone 'utc'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "now()::timestamp(0) at time zone 'utc'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Batches",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "now()::timestamp(0) at time zone 'utc'");
        }
    }
}
