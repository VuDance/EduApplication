using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduApplication.Migrations
{
    /// <inheritdoc />
    public partial class addclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_Subjects_SubjectId",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_Class_Teachers_TeacherId",
                table: "Class");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Class",
                table: "Class");

            migrationBuilder.RenameTable(
                name: "Class",
                newName: "Classes");

            migrationBuilder.RenameIndex(
                name: "IX_Class_TeacherId",
                table: "Classes",
                newName: "IX_Classes_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_Class_SubjectId",
                table: "Classes",
                newName: "IX_Classes_SubjectId");

            migrationBuilder.AddColumn<int>(
                name: "MaxStudent",
                table: "Classes",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "ClassId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$v2ElYiQsGWYFClZM.5VDf.E/PBIMLLANQohBIvUAJeRC5rQOBz7RC");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Subjects_SubjectId",
                table: "Classes",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Teachers_TeacherId",
                table: "Classes",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Subjects_SubjectId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Teachers_TeacherId",
                table: "Classes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "MaxStudent",
                table: "Classes");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "Class");

            migrationBuilder.RenameIndex(
                name: "IX_Classes_TeacherId",
                table: "Class",
                newName: "IX_Class_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_Classes_SubjectId",
                table: "Class",
                newName: "IX_Class_SubjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Class",
                table: "Class",
                column: "ClassId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$noJTGxstCRcTX2.wtEzWE.txnlmFk6BP/aPeSCdYFy189ebEXQmpC");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Subjects_SubjectId",
                table: "Class",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Teachers_TeacherId",
                table: "Class",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
