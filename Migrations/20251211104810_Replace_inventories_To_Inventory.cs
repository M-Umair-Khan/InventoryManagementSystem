using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Replace_inventories_To_Inventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Products_ProductID",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Warehouses_WarehouseID",
                table: "Inventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories");

            migrationBuilder.RenameTable(
                name: "Inventories",
                newName: "Inventory");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_WarehouseID",
                table: "Inventory",
                newName: "IX_Inventory_WarehouseID");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_ProductID_WarehouseID",
                table: "Inventory",
                newName: "IX_Inventory_ProductID_WarehouseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventory",
                table: "Inventory",
                column: "InventoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Products_ProductID",
                table: "Inventory",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Warehouses_WarehouseID",
                table: "Inventory",
                column: "WarehouseID",
                principalTable: "Warehouses",
                principalColumn: "WarehouseID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Products_ProductID",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Warehouses_WarehouseID",
                table: "Inventory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventory",
                table: "Inventory");

            migrationBuilder.RenameTable(
                name: "Inventory",
                newName: "Inventories");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_WarehouseID",
                table: "Inventories",
                newName: "IX_Inventories_WarehouseID");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_ProductID_WarehouseID",
                table: "Inventories",
                newName: "IX_Inventories_ProductID_WarehouseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories",
                column: "InventoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Products_ProductID",
                table: "Inventories",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Warehouses_WarehouseID",
                table: "Inventories",
                column: "WarehouseID",
                principalTable: "Warehouses",
                principalColumn: "WarehouseID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
