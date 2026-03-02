using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Serviços.Migrations
{
    /// <inheritdoc />
    public partial class Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tb_Users_cpf",
                table: "Tb_Users",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tb_Users_Email",
                table: "Tb_Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tb_Users_cpf",
                table: "Tb_Users");

            migrationBuilder.DropIndex(
                name: "IX_Tb_Users_Email",
                table: "Tb_Users");
        }
    }
}
