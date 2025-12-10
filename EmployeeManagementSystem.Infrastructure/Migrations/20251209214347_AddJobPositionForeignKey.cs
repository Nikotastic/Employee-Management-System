using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPositionForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Esta migración está vacía porque las foreign keys ya fueron creadas en InitialCreate
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No hay nada que revertir
        }
    }
}
