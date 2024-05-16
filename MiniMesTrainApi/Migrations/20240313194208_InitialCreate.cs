using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniMesTrainApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MiniMes");

            migrationBuilder.CreateTable(
                name: "Machines",
                schema: "MiniMes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                schema: "MiniMes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "MiniMes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "MiniMes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orders_Machines_MachineId",
                        column: x => x.MachineId,
                        principalSchema: "MiniMes",
                        principalTable: "Machines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "MiniMes",
                        principalTable: "Products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Processes",
                schema: "MiniMes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Processes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "MiniMes",
                        principalTable: "Orders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessParameters",
                schema: "MiniMes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProcessParameters_Parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalSchema: "MiniMes",
                        principalTable: "Parameters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessParameters_Processes_ProcessId",
                        column: x => x.ProcessId,
                        principalSchema: "MiniMes",
                        principalTable: "Processes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MachineId",
                schema: "MiniMes",
                table: "Orders",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                schema: "MiniMes",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessParameters_ParameterId",
                schema: "MiniMes",
                table: "ProcessParameters",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessParameters_ProcessId",
                schema: "MiniMes",
                table: "ProcessParameters",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_OrderId",
                schema: "MiniMes",
                table: "Processes",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessParameters",
                schema: "MiniMes");

            migrationBuilder.DropTable(
                name: "Parameters",
                schema: "MiniMes");

            migrationBuilder.DropTable(
                name: "Processes",
                schema: "MiniMes");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "MiniMes");

            migrationBuilder.DropTable(
                name: "Machines",
                schema: "MiniMes");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "MiniMes");
        }
    }
}
