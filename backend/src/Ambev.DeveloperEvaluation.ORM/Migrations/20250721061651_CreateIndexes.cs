using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class CreateIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_BranchDate",
                table: "Sales",
                columns: new[] { "BranchId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_BranchId",
                table: "Sales",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerBranch",
                table: "Sales",
                columns: new[] { "CustomerId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerBranchDate",
                table: "Sales",
                columns: new[] { "CustomerId", "BranchId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Date",
                table: "Sales",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SaleNumber",
                table: "Sales",
                column: "SaleNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sales_BranchDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_BranchId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CustomerBranch",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CustomerBranchDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_Date",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_SaleNumber",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");
        }
    }
}
