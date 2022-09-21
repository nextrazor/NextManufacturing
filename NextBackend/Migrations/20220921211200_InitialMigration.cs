using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextBackend.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarStates",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarStates", x => x.guid);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.guid);
                });

            migrationBuilder.CreateTable(
                name: "CalendarTemplates",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    default_state_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_duration = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarTemplates", x => x.guid);
                    table.ForeignKey(
                        name: "FK_CalendarTemplates_CalendarStates_default_state_guid",
                        column: x => x.default_state_guid,
                        principalTable: "CalendarStates",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarTemplateSpans",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    calendar_template_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    from_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    to_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    state_guid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarTemplateSpans", x => x.guid);
                    table.CheckConstraint("CK_CalendarTemplates_FromTo", "from_time < to_time");
                    table.ForeignKey(
                        name: "FK_CalendarTemplateSpans_CalendarStates_state_guid",
                        column: x => x.state_guid,
                        principalTable: "CalendarStates",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarTemplateSpans_CalendarTemplates_calendar_template_g~",
                        column: x => x.calendar_template_guid,
                        principalTable: "CalendarTemplates",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceCalendars",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    calendar_template_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    from_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCalendars", x => x.guid);
                    table.ForeignKey(
                        name: "FK_ResourceCalendars_CalendarTemplates_calendar_template_guid",
                        column: x => x.calendar_template_guid,
                        principalTable: "CalendarTemplates",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceCalendars_Resources_resource_guid",
                        column: x => x.resource_guid,
                        principalTable: "Resources",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CalendarStates",
                columns: new[] { "guid", "name" },
                values: new object[,]
                {
                    { new Guid("17c94115-07cc-4326-9ab4-66d4155cfe1a"), "Работает" },
                    { new Guid("68ed1833-de39-435f-9e7b-e87325a35ad5"), "Перерыв" }
                });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "guid", "name" },
                values: new object[,]
                {
                    { new Guid("7bb90a66-f18b-4acc-ae91-49d535e0d189"), "Станок токарный (инв. 120)" },
                    { new Guid("9e1d6ec8-5875-4dd7-b0c3-8134006d25a1"), "Станок токарный (инв. 122)" },
                    { new Guid("bbfcf611-5234-4f61-9502-991e340006bb"), "Станок фрезерный (инв. 212)" }
                });

            migrationBuilder.InsertData(
                table: "CalendarTemplates",
                columns: new[] { "guid", "default_state_guid", "name", "period_duration", "reference_date" },
                values: new object[] { new Guid("b462c5b6-cecb-41f7-922d-a62a565a85ea"), new Guid("17c94115-07cc-4326-9ab4-66d4155cfe1a"), "Круглосуточный", new TimeSpan(1, 0, 0, 0, 0), new DateTimeOffset(new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "ResourceCalendars",
                columns: new[] { "guid", "calendar_template_guid", "from_time", "resource_guid" },
                values: new object[,]
                {
                    { new Guid("800037d5-cd8a-4d19-958e-3732b1864b91"), new Guid("b462c5b6-cecb-41f7-922d-a62a565a85ea"), new DateTimeOffset(new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), new Guid("bbfcf611-5234-4f61-9502-991e340006bb") },
                    { new Guid("8b92dc58-6f6a-42a8-a4ff-6fdec803d94c"), new Guid("b462c5b6-cecb-41f7-922d-a62a565a85ea"), new DateTimeOffset(new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), new Guid("7bb90a66-f18b-4acc-ae91-49d535e0d189") },
                    { new Guid("f4fb391b-0519-4e38-98b5-c8ce6829e893"), new Guid("b462c5b6-cecb-41f7-922d-a62a565a85ea"), new DateTimeOffset(new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), new Guid("9e1d6ec8-5875-4dd7-b0c3-8134006d25a1") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarStates_name",
                table: "CalendarStates",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTemplates_default_state_guid",
                table: "CalendarTemplates",
                column: "default_state_guid");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTemplates_name",
                table: "CalendarTemplates",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTemplateSpans_calendar_template_guid",
                table: "CalendarTemplateSpans",
                column: "calendar_template_guid");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTemplateSpans_state_guid",
                table: "CalendarTemplateSpans",
                column: "state_guid");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendars_calendar_template_guid",
                table: "ResourceCalendars",
                column: "calendar_template_guid");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendars_resource_guid_from_time",
                table: "ResourceCalendars",
                columns: new[] { "resource_guid", "from_time" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_name",
                table: "Resources",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarTemplateSpans");

            migrationBuilder.DropTable(
                name: "ResourceCalendars");

            migrationBuilder.DropTable(
                name: "CalendarTemplates");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "CalendarStates");
        }
    }
}
