using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserProjectJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_MembersId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectsId",
                table: "ProjectMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.RenameTable(
                name: "ProjectMembers",
                newName: "ApplicationUserProject");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_ProjectsId",
                table: "ApplicationUserProject",
                newName: "IX_ApplicationUserProject_ProjectsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserProject",
                table: "ApplicationUserProject",
                columns: new[] { "MembersId", "ProjectsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProject_AspNetUsers_MembersId",
                table: "ApplicationUserProject",
                column: "MembersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProject_Projects_ProjectsId",
                table: "ApplicationUserProject",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProject_AspNetUsers_MembersId",
                table: "ApplicationUserProject");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProject_Projects_ProjectsId",
                table: "ApplicationUserProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserProject",
                table: "ApplicationUserProject");

            migrationBuilder.RenameTable(
                name: "ApplicationUserProject",
                newName: "ProjectMembers");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserProject_ProjectsId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_ProjectsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers",
                columns: new[] { "MembersId", "ProjectsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_MembersId",
                table: "ProjectMembers",
                column: "MembersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectsId",
                table: "ProjectMembers",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
