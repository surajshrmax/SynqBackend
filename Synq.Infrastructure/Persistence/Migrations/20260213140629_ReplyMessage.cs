using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synq.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplyMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReplyMessageId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyMessageId",
                table: "Messages");
        }
    }
}
