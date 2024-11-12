using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectMembersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProject_AspNetUsers_MembersId",
                table: "ApplicationUserProject");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProject_Projects_ProjectsId",
                table: "ApplicationUserProject");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProjectTask_AspNetUsers_AssignedUsersId",
                table: "ApplicationUserProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProjectTask_Tasks_TasksId",
                table: "ApplicationUserProjectTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserProjectTask",
                table: "ApplicationUserProjectTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserProject",
                table: "ApplicationUserProject");

            migrationBuilder.RenameTable(
                name: "ApplicationUserProjectTask",
                newName: "TaskAssignments");

            migrationBuilder.RenameTable(
                name: "ApplicationUserProject",
                newName: "ProjectMembers");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserProjectTask_TasksId",
                table: "TaskAssignments",
                newName: "IX_TaskAssignments_TasksId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserProject_ProjectsId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_ProjectsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskAssignments",
                table: "TaskAssignments",
                columns: new[] { "AssignedUsersId", "TasksId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAssignments_AspNetUsers_AssignedUsersId",
                table: "TaskAssignments",
                column: "AssignedUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAssignments_Tasks_TasksId",
                table: "TaskAssignments",
                column: "TasksId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_MembersId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectsId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskAssignments_AspNetUsers_AssignedUsersId",
                table: "TaskAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskAssignments_Tasks_TasksId",
                table: "TaskAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskAssignments",
                table: "TaskAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.RenameTable(
                name: "TaskAssignments",
                newName: "ApplicationUserProjectTask");

            migrationBuilder.RenameTable(
                name: "ProjectMembers",
                newName: "ApplicationUserProject");

            migrationBuilder.RenameIndex(
                name: "IX_TaskAssignments_TasksId",
                table: "ApplicationUserProjectTask",
                newName: "IX_ApplicationUserProjectTask_TasksId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_ProjectsId",
                table: "ApplicationUserProject",
                newName: "IX_ApplicationUserProject_ProjectsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserProjectTask",
                table: "ApplicationUserProjectTask",
                columns: new[] { "AssignedUsersId", "TasksId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProjectTask_AspNetUsers_AssignedUsersId",
                table: "ApplicationUserProjectTask",
                column: "AssignedUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProjectTask_Tasks_TasksId",
                table: "ApplicationUserProjectTask",
                column: "TasksId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
