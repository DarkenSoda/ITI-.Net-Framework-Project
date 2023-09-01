using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProjectITI.Migrations
{
    /// <inheritdoc />
    public partial class removingCartItemAndShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.DropTable(
                name: "ShoppingCart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => new { x.Id, x.CustomerID });
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    ShoppingCartID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ShoppingCartCustomerID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => new { x.ProductID, x.ShoppingCartID });
                    table.ForeignKey(
                        name: "FK_CartItem_ShoppingCart_ShoppingCartID_ShoppingCartCustomerID",
                        columns: x => new { x.ShoppingCartID, x.ShoppingCartCustomerID },
                        principalTable: "ShoppingCart",
                        principalColumns: new[] { "Id", "CustomerID" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ShoppingCartID_ShoppingCartCustomerID",
                table: "CartItem",
                columns: new[] { "ShoppingCartID", "ShoppingCartCustomerID" });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_CustomerID",
                table: "ShoppingCart",
                column: "CustomerID",
                unique: true);
        }
    }
}
