using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmailServer.Data.Migrations
{
    public partial class M1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(maxLength: 256, nullable: true, defaultValueSql: "NEWID()"),
                    Active = table.Column<bool>(nullable: false),
                    ScheduledTime = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    ToName = table.Column<string>(maxLength: 256, nullable: false),
                    ToEmail = table.Column<string>(maxLength: 256, nullable: false),
                    FromName = table.Column<string>(maxLength: 256, nullable: false),
                    FromEmail = table.Column<string>(maxLength: 256, nullable: false),
                    DefaultBcc = table.Column<string>(maxLength: 256, nullable: true),
                    Subject = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsHTML = table.Column<bool>(nullable: false),
                    CompanyInfo = table.Column<string>(nullable: true),
                    CompanyId = table.Column<string>(nullable: false),
                    JobId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emails_JobId",
                table: "Emails",
                column: "JobId");
        }

        // Roll back is going to drop all the table in the DB! BEWARE!!!
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emails");
        }
    }
}
