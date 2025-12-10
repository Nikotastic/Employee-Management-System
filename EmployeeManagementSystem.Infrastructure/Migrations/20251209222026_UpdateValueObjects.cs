using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Verifica si la tabla employees existe antes de intentar eliminar columnas
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'employees') THEN
                        IF EXISTS (SELECT FROM information_schema.columns WHERE table_name = 'employees' AND column_name = 'email') THEN
                            ALTER TABLE employees DROP COLUMN email;
                        END IF;
                        IF EXISTS (SELECT FROM information_schema.columns WHERE table_name = 'employees' AND column_name = 'phone') THEN
                            ALTER TABLE employees DROP COLUMN phone;
                        END IF;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "employees",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
