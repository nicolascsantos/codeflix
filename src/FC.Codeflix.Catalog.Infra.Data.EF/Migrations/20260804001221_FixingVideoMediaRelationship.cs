using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FC.Codeflix.Catalog.Infra.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class FixingVideoMediaRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Videos_MediaId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Videos_TrailerId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_MediaId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_TrailerId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "TrailerId",
                table: "Media");

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "Videos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "TrailerId",
                table: "Videos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_MediaId",
                table: "Videos",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_TrailerId",
                table: "Videos",
                column: "TrailerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Media_MediaId",
                table: "Videos",
                column: "MediaId",
                principalTable: "Media",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Media_TrailerId",
                table: "Videos",
                column: "TrailerId",
                principalTable: "Media",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Media_MediaId",
                table: "Videos");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Media_TrailerId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_MediaId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_TrailerId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "TrailerId",
                table: "Videos");

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "Media",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "TrailerId",
                table: "Media",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Media_MediaId",
                table: "Media",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_TrailerId",
                table: "Media",
                column: "TrailerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Videos_MediaId",
                table: "Media",
                column: "MediaId",
                principalTable: "Videos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Videos_TrailerId",
                table: "Media",
                column: "TrailerId",
                principalTable: "Videos",
                principalColumn: "Id");
        }
    }
}
