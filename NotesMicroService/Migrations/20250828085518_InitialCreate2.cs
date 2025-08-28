using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NotesMicroService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "Id",
                keyValue: new Guid("0e1a2b97-ed2e-48b0-9c9e-d7e7fc99468e"));

            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "Id",
                keyValue: new Guid("746d18d2-3aa7-4adb-bc47-4f8fc2f0bcc0"));

            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "Id",
                keyValue: new Guid("9e83bf4b-5907-4289-87bb-ae5a33a825ec"));

            migrationBuilder.InsertData(
                table: "Notes",
                columns: new[] { "Id", "Content", "PatientId" },
                values: new object[,]
                {
                    { new Guid("16eae5db-88c9-4eb3-94a3-d4c7825c1ecc"), "Patient shows improvement in mobility exercises.", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("a0df8709-c007-4007-8856-dfc7809ea83f"), "Patient reports reduced pain levels.", new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("d9aa4831-29be-464d-b400-9b8fb9567a24"), "Follow-up appointment scheduled for next week.", new Guid("11111111-1111-1111-1111-111111111111") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "Id",
                keyValue: new Guid("16eae5db-88c9-4eb3-94a3-d4c7825c1ecc"));

            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "Id",
                keyValue: new Guid("a0df8709-c007-4007-8856-dfc7809ea83f"));

            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "Id",
                keyValue: new Guid("d9aa4831-29be-464d-b400-9b8fb9567a24"));

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
    }
}
