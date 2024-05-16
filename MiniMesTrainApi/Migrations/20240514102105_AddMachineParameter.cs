using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniMesTrainApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMachineParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Minimes");

            migrationBuilder.CreateTable(
                name: "MachineParameters",
                schema: "Minimes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    ParameterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MachineParameters_Machines_MachineId",
                        column: x => x.MachineId,
                        principalSchema: "MiniMes",
                        principalTable: "Machines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MachineParameters_Parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalSchema: "MiniMes",
                        principalTable: "Parameters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachineParameters_MachineId",
                schema: "Minimes",
                table: "MachineParameters",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineParameters_ParameterId",
                schema: "Minimes",
                table: "MachineParameters",
                column: "ParameterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineParameters",
                schema: "Minimes");
        }
    }
}
