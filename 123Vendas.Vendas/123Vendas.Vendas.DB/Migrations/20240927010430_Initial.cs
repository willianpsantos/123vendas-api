using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _123Vendas.Vendas.DB.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    company_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    banch_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    saler_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sale_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sale_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Canceled = table.Column<bool>(type: "bit", nullable: true),
                    CanceledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CanceledBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    included_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, defaultValue: new DateTimeOffset(new DateTime(2024, 9, 27, 1, 4, 29, 938, DateTimeKind.Unspecified).AddTicks(3757), new TimeSpan(0, 0, 0, 0, 0))),
                    included_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    updated_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sale_products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sale_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    canceled = table.Column<bool>(type: "bit", nullable: true),
                    canceled_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    canceled_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    included_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, defaultValue: new DateTimeOffset(new DateTime(2024, 9, 27, 1, 4, 29, 939, DateTimeKind.Unspecified).AddTicks(3825), new TimeSpan(0, 0, 0, 0, 0))),
                    included_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    updated_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_products_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sale_products_sale_id",
                table: "sale_products",
                column: "sale_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sale_products");

            migrationBuilder.DropTable(
                name: "sales");
        }
    }
}
