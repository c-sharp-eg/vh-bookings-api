using Microsoft.EntityFrameworkCore.Migrations;
using System.Linq;

namespace Bookings.DAL.Migrations
{
    public partial class Seed_ChecklistQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            const int individualRoleId = 5;
            const int representativeRoleId = 6;

            migrationBuilder.InsertData(
                table: "ChecklistQuestion",
                columns: new[] { "Id", "UserRoleId", "Key", "Description" },
                values: new object[,]
                {
                    






                    { 1, individualRoleId, "EQUIPMENT_PHONE","On the day of the hearing, will you have access to a phone?" },
                    { 2, individualRoleId, "EQUIPMENT_INTERNET","On the day of the hearing, will you have access to an internet connection?" },
                    { 3, individualRoleId, "EQUIPMENT_LAPTOP","On the day of the hearing, will you have access to a laptop?," },
                    { 4, individualRoleId, "EQUIPMENT_COMPUTER_CAMERA","On the day of the hearing, will you have access to a desktop computer with a camera?" },
                    { 5, individualRoleId, "EQUIPMENT_NONE","None of the above" },
                    { 6, individualRoleId, "ANY_OTHER_CIRCUMSTANCES","Is there anything the court should be aware of when it decides which type of hearing will be suitable?" },
                    { 7, individualRoleId, "SUITABLE_ROOM_AVAILABLE","Will you have access to a suitable room?" },
                    { 8, individualRoleId, "USER_CONSENT","Your consent for a video hearing" },
                    { 9, individualRoleId, "NEED_INTERPRETER","Will you need an interpreter for your hearing?" },
                    { 10, representativeRoleId, "OTHER_INFORMATION","Is there anything else you would like to draw to the court\'s attention?" },
                    { 11, representativeRoleId, "EQUIPMENT_SAME_DEVICE","Please confirm you are using the same computer you would use for the hearing." },
                    //{ 12, representativeRoleId, "EQUIPMENT_BANDWIDTH","" },
                    //{ 13, representativeRoleId, "EQUIPMENT_DEVICE_TYPE" },
                    //{ 14, representativeRoleId, "EQUIPMENT_BROWSER" },
                    //{ 15, representativeRoleId, "EQUIPMENT_CAM_AND_MIC_PRESENT" },
                    //{ 16, representativeRoleId, "ANY_OTHER_CIRCUMSTANCES" },
                    //{ 17, representativeRoleId, "SUITABLE_ROOM_AVAILABLE" },
                    //{ 18, representativeRoleId, "USER_CONSENT" },
                    //{ 19, representativeRoleId, "NEED_INTERPRETER" },
                    //{ 20, representativeRoleId, "OTHER_PERSON_IN_ROOM" },
                    // { 21, representativeRoleId, "EQUIPMENT_INTERNET" },
                    //{ 22, representativeRoleId, "EQUIPMENT_LAPTOP" },
                    //{ 23, representativeRoleId, "OTHER_PERSON_IN_ROOM" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("ChecklistQuestion", "Id", Enumerable.Range(1, 23).ToArray());
        }
    }
}
