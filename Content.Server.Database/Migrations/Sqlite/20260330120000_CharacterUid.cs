using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    [DbContext(typeof(SqliteServerDbContext))]
    [Migration("20260330120000_CharacterUid")]
    public partial class CharacterUid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "character_uid",
                table: "profile",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE profile
                SET character_uid = lower(
                    hex(randomblob(4)) || '-' ||
                    hex(randomblob(2)) || '-' ||
                    '4' || substr(hex(randomblob(2)), 2) || '-' ||
                    substr('89ab', abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)), 2) || '-' ||
                    hex(randomblob(6))
                )
                WHERE character_uid IS NULL;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_profile_character_uid",
                table: "profile",
                column: "character_uid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_profile_character_uid",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "character_uid",
                table: "profile");
        }
    }
}
