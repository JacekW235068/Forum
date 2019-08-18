using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Migrations
{
    public partial class ForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumThread_SubForum_ParentForumSubForumID",
                table: "ForumThread");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumThread_AspNetUsers_UserId",
                table: "ForumThread");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_ForumThread_ParentThreadThreadID",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubForum",
                table: "SubForum");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ForumThread",
                table: "ForumThread");

            migrationBuilder.RenameTable(
                name: "SubForum",
                newName: "SubForums");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "ForumThread",
                newName: "Threads");

            migrationBuilder.RenameColumn(
                name: "ParentThreadThreadID",
                table: "Posts",
                newName: "ParentID");

            migrationBuilder.RenameIndex(
                name: "IX_Post_UserId",
                table: "Posts",
                newName: "IX_Posts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_ParentThreadThreadID",
                table: "Posts",
                newName: "IX_Posts_ParentID");

            migrationBuilder.RenameColumn(
                name: "ParentForumSubForumID",
                table: "Threads",
                newName: "ParentID");

            migrationBuilder.RenameIndex(
                name: "IX_ForumThread_UserId",
                table: "Threads",
                newName: "IX_Threads_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ForumThread_ParentForumSubForumID",
                table: "Threads",
                newName: "IX_Threads_ParentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubForums",
                table: "SubForums",
                column: "SubForumID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "PostID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Threads",
                table: "Threads",
                column: "ThreadID");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Threads_ParentID",
                table: "Posts",
                column: "ParentID",
                principalTable: "Threads",
                principalColumn: "ThreadID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_SubForums_ParentID",
                table: "Threads",
                column: "ParentID",
                principalTable: "SubForums",
                principalColumn: "SubForumID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_AspNetUsers_UserId",
                table: "Threads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Threads_ParentID",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_SubForums_ParentID",
                table: "Threads");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_AspNetUsers_UserId",
                table: "Threads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Threads",
                table: "Threads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubForums",
                table: "SubForums");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "Threads",
                newName: "ForumThread");

            migrationBuilder.RenameTable(
                name: "SubForums",
                newName: "SubForum");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                table: "ForumThread",
                newName: "ParentForumSubForumID");

            migrationBuilder.RenameIndex(
                name: "IX_Threads_UserId",
                table: "ForumThread",
                newName: "IX_ForumThread_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Threads_ParentID",
                table: "ForumThread",
                newName: "IX_ForumThread_ParentForumSubForumID");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                table: "Post",
                newName: "ParentThreadThreadID");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId",
                table: "Post",
                newName: "IX_Post_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ParentID",
                table: "Post",
                newName: "IX_Post_ParentThreadThreadID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForumThread",
                table: "ForumThread",
                column: "ThreadID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubForum",
                table: "SubForum",
                column: "SubForumID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "PostID");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThread_SubForum_ParentForumSubForumID",
                table: "ForumThread",
                column: "ParentForumSubForumID",
                principalTable: "SubForum",
                principalColumn: "SubForumID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThread_AspNetUsers_UserId",
                table: "ForumThread",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_ForumThread_ParentThreadThreadID",
                table: "Post",
                column: "ParentThreadThreadID",
                principalTable: "ForumThread",
                principalColumn: "ThreadID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
