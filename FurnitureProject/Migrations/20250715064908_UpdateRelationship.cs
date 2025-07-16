using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_item_products_ProductId",
                table: "order_item");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId1",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_CategoryId1",
                table: "products",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_order_item_products_ProductId",
                table: "order_item",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryId1",
                table: "products",
                column: "CategoryId1",
                principalTable: "categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_item_products_ProductId",
                table: "order_item");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId1",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_CategoryId1",
                table: "products");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "products");

            migrationBuilder.AddForeignKey(
                name: "FK_order_item_products_ProductId",
                table: "order_item",
                column: "ProductId",
                principalTable: "products",
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
    }
}
