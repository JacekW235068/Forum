using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Migrations
{
    public partial class BasicModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubForum",
                columns: table => new
                {
                    SubForumID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubForum", x => x.SubForumID);
                });

            migrationBuilder.CreateTable(
                name: "ForumThread",
                columns: table => new
                {
                    ThreadID = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Text = table.Column<string>(maxLength: 500, nullable: false),
                    PostTime = table.Column<DateTime>(nullable: false),
                    LastPostTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ParentForumSubForumID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumThread", x => x.ThreadID);
                    table.ForeignKey(
                        name: "FK_ForumThread_SubForum_ParentForumSubForumID",
                        column: x => x.ParentForumSubForumID,
                        principalTable: "SubForum",
                        principalColumn: "SubForumID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumThread_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull); //Is this even legal
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostID = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(maxLength: 500, nullable: false),
                    PostTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ParentThreadThreadID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostID);
                    table.ForeignKey(
                        name: "FK_Post_ForumThread_ParentThreadThreadID",
                        column: x => x.ParentThreadThreadID,
                        principalTable: "ForumThread",
                        principalColumn: "ThreadID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForumThread_ParentForumSubForumID",
                table: "ForumThread",
                column: "ParentForumSubForumID");

            migrationBuilder.CreateIndex(
                name: "IX_ForumThread_UserId",
                table: "ForumThread",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ParentThreadThreadID",
                table: "Post",
                column: "ParentThreadThreadID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserId",
                table: "Post",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "ForumThread");

            migrationBuilder.DropTable(
                name: "SubForum");
        }
    }
}
