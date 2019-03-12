using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class CreateCheckListQuestionAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecklistQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserRoleId = table.Column<int>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistQuestion_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistAnswer",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Answer = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ChecklistQuestionId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswer_ChecklistQuestion_ChecklistQuestionId",
                        column: x => x.ChecklistQuestionId,
                        principalTable: "ChecklistQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswer_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswer_ChecklistQuestionId",
                table: "ChecklistAnswer",
                column: "ChecklistQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswer_ParticipantId",
                table: "ChecklistAnswer",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswer_ParticipantId_ChecklistQuestionId",
                table: "ChecklistAnswer",
                columns: new[] { "ParticipantId", "ChecklistQuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestion_UserRoleId",
                table: "ChecklistQuestion",
                column: "UserRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistAnswer");

            migrationBuilder.DropTable(
                name: "ChecklistQuestion");
        }
    }
}
