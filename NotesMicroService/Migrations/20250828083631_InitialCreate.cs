using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NotesMicroService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Notes",
                columns: new[] { "Id", "Content", "PatientId" },
                values: new object[,]
                {
                    { new Guid("0e1a2b97-ed2e-48b0-9c9e-d7e7fc99468e"), "Patient shows improvement in mobility exercises.", new Guid("db968684-8eab-4572-82ca-15439b5730fc") },
                    { new Guid("746d18d2-3aa7-4adb-bc47-4f8fc2f0bcc0"), "Follow-up appointment scheduled for next week.", new Guid("db968684-8eab-4572-82ca-15439b5730fc") },
                    { new Guid("9e83bf4b-5907-4289-87bb-ae5a33a825ec"), "Patient reports reduced pain levels.", new Guid("5abebeb4-40a8-4fa2-ad75-44ce81d133ea") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notes");
        }
    }
}
