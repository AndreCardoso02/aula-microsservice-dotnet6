using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekShopping.CartAPI.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Coupon_To_Allow_Null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "coupon_code",
                table: "cart_header",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "cart_header",
                keyColumn: "coupon_code",
                keyValue: null,
                column: "coupon_code",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "coupon_code",
                table: "cart_header",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
