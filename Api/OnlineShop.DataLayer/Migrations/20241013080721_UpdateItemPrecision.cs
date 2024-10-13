using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.DataLayer.Migrations
{
    public partial class UpdateItemPrecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itemTraits_Items_ItemId",
                table: "itemTraits");

            migrationBuilder.DropForeignKey(
                name: "FK_itemTraits_Traits_TraitId",
                table: "itemTraits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_itemTraits",
                table: "itemTraits");

            migrationBuilder.RenameTable(
                name: "itemTraits",
                newName: "ItemTraits");

            migrationBuilder.RenameIndex(
                name: "IX_itemTraits_TraitId",
                table: "ItemTraits",
                newName: "IX_ItemTraits_TraitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTraits",
                table: "ItemTraits",
                columns: new[] { "ItemId", "TraitId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTraits_Items_ItemId",
                table: "ItemTraits",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTraits_Traits_TraitId",
                table: "ItemTraits",
                column: "TraitId",
                principalTable: "Traits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemTraits_Items_ItemId",
                table: "ItemTraits");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTraits_Traits_TraitId",
                table: "ItemTraits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTraits",
                table: "ItemTraits");

            migrationBuilder.RenameTable(
                name: "ItemTraits",
                newName: "itemTraits");

            migrationBuilder.RenameIndex(
                name: "IX_ItemTraits_TraitId",
                table: "itemTraits",
                newName: "IX_itemTraits_TraitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_itemTraits",
                table: "itemTraits",
                columns: new[] { "ItemId", "TraitId" });

            migrationBuilder.AddForeignKey(
                name: "FK_itemTraits_Items_ItemId",
                table: "itemTraits",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_itemTraits_Traits_TraitId",
                table: "itemTraits",
                column: "TraitId",
                principalTable: "Traits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
