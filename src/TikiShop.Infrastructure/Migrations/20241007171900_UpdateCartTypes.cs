using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikiShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_CardType_CardTypeId",
                table: "PaymentMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardType",
                table: "CardType");

            migrationBuilder.RenameTable(
                name: "CardType",
                newName: "CardTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardTypes",
                table: "CardTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_CardTypes_CardTypeId",
                table: "PaymentMethods",
                column: "CardTypeId",
                principalTable: "CardTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_CardTypes_CardTypeId",
                table: "PaymentMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardTypes",
                table: "CardTypes");

            migrationBuilder.RenameTable(
                name: "CardTypes",
                newName: "CardType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardType",
                table: "CardType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_CardType_CardTypeId",
                table: "PaymentMethods",
                column: "CardTypeId",
                principalTable: "CardType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
