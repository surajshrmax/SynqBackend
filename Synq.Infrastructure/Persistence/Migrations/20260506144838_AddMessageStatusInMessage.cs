using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synq.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageStatusInMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_MessageId",
                table: "MessageStatuses",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageStatuses_MessageId",
                table: "MessageStatuses");
        }
    }
}
