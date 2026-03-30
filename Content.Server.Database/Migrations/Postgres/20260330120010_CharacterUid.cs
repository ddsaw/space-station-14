using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    [DbContext(typeof(PostgresServerDbContext))]
    [Migration("20260330120010_CharacterUid")]
    public partial class CharacterUid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "character_uid",
                table: "profile",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE profile
                SET character_uid = md5(random()::text || clock_timestamp()::text)::uuid
                WHERE character_uid IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "character_uid",
                table: "profile",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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
