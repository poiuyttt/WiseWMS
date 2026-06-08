using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WiseWMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_products_CategoryId",
                table: "products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_outbound_orders_CustomerId",
                table: "outbound_orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_outbound_orders_OperatorId",
                table: "outbound_orders",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_outbound_items_ProductId",
                table: "outbound_items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_OperatorId",
                table: "inventory_transactions",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_ProductId",
                table: "inventory_transactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_inbound_orders_OperatorId",
                table: "inbound_orders",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_inbound_orders_SupplierId",
                table: "inbound_orders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_inbound_items_ProductId",
                table: "inbound_items",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_inbound_items_products_ProductId",
                table: "inbound_items",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inbound_orders_suppliers_SupplierId",
                table: "inbound_orders",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inbound_orders_users_OperatorId",
                table: "inbound_orders",
                column: "OperatorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_products_ProductId",
                table: "inventory_transactions",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_users_OperatorId",
                table: "inventory_transactions",
                column: "OperatorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_outbound_items_products_ProductId",
                table: "outbound_items",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_outbound_orders_customers_CustomerId",
                table: "outbound_orders",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_outbound_orders_users_OperatorId",
                table: "outbound_orders",
                column: "OperatorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inbound_items_products_ProductId",
                table: "inbound_items");

            migrationBuilder.DropForeignKey(
                name: "FK_inbound_orders_suppliers_SupplierId",
                table: "inbound_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_inbound_orders_users_OperatorId",
                table: "inbound_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_products_ProductId",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_users_OperatorId",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_outbound_items_products_ProductId",
                table: "outbound_items");

            migrationBuilder.DropForeignKey(
                name: "FK_outbound_orders_customers_CustomerId",
                table: "outbound_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_outbound_orders_users_OperatorId",
                table: "outbound_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_CategoryId",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_outbound_orders_CustomerId",
                table: "outbound_orders");

            migrationBuilder.DropIndex(
                name: "IX_outbound_orders_OperatorId",
                table: "outbound_orders");

            migrationBuilder.DropIndex(
                name: "IX_outbound_items_ProductId",
                table: "outbound_items");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_OperatorId",
                table: "inventory_transactions");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_ProductId",
                table: "inventory_transactions");

            migrationBuilder.DropIndex(
                name: "IX_inbound_orders_OperatorId",
                table: "inbound_orders");

            migrationBuilder.DropIndex(
                name: "IX_inbound_orders_SupplierId",
                table: "inbound_orders");

            migrationBuilder.DropIndex(
                name: "IX_inbound_items_ProductId",
                table: "inbound_items");
        }
    }
}
