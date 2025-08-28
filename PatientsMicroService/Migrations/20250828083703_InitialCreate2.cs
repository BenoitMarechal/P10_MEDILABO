using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PatientsMicroService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "BirthDate", "FirstName", "Gender", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "1 Brookside St", new DateOnly(1966, 12, 31), "Test", "Female", "TestNone", "100-222-3333" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "2 High St", new DateOnly(1945, 6, 24), "Test", "Male", "TestBorderLine", "200-333-4444" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "3 Club Road", new DateOnly(2004, 6, 18), "Test", "Male", "TestDanger", "300-444-5555" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "4 Valley Dr", new DateOnly(2002, 6, 28), "Test", "Female", "TestEarlyOnSet", "400-555-6666" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
