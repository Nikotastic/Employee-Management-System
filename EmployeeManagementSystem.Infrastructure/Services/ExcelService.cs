using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Enums;
using OfficeOpenXml;

namespace EmployeeManagementSystem.Infrastructure.Services;

// Service for handling Excel file operations
public class ExcelService : IExcelService
{
    public Task<List<ImportEmployeeDto>> ImportEmployeesFromExcelAsync(Stream fileStream)
    {
        return Task.Run(() =>
        {
            var employees = new List<ImportEmployeeDto>();
            
            // Configure license for EPPlus 5.2.5
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];
        
        if (worksheet.Dimension == null)
            return employees;
        
        var rowCount = worksheet.Dimension.Rows;
        
        // Column Mapping in Spanish
        var columnMap = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
        {
            var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim().ToLower();
            if (!string.IsNullOrEmpty(headerValue))
            {
                columnMap[headerValue] = col;
            }
        }
        
        // Read data from row 2 (after header)
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var employee = new ImportEmployeeDto
                {
                    Document = GetCellValue(worksheet, row, columnMap, "documento", "cedula", "Documento"),
                    FirstName = GetCellValue(worksheet, row, columnMap, "nombre", "nombres", "primer nombre", "Nombres"),
                    LastName = GetCellValue(worksheet, row, columnMap, "apellido", "apellidos", "Apellidos"),
                    BirthDate = GetDateValue(worksheet, row, columnMap, "fecha de nacimiento", "fecha nacimiento", "nacimiento", "FechaNacimiento"),
                    Address = GetCellValue(worksheet, row, columnMap, "direccion", "dirección", "Direccion"),
                    Email = GetCellValue(worksheet, row, columnMap, "correo", "email", "correo electronico", "Email"),
                    Phone = GetCellValue(worksheet, row, columnMap, "telefono", "teléfono", "celular", "Telefono"),
                    Salary = GetDecimalValue(worksheet, row, columnMap, "salario", "sueldo", "Salario" ),
                    HiringDate = GetDateValue(worksheet, row, columnMap, "fecha de contratacion", "fecha contratación", "fecha ingreso", "fecha de ingreso", "FechaIngreso"),
                    ProfessionalProfile = GetCellValue(worksheet, row, columnMap, "perfil profesional", "perfil", "descripcion", "Perfil Profesional"),
                    Status = GetStatusValue(worksheet, row, columnMap, "estado", "Estado"),
                    EducationLevel = GetEducationLevelValue(worksheet, row, columnMap, "nivel educativo", "educacion", "educación", "Nivel Educativo", "NivelEducativo", "niveleducativo"),
                    JobPositionId = 0, 
                    DepartmentId = 0,
                    JobPositionName = GetCellValue(worksheet, row, columnMap, "cargo", "posicion", "puesto", "Cargo"),
                    DepartmentName = GetCellValue(worksheet, row, columnMap, "departamento", "area", "área", "Departamento")
                };
                
                employees.Add(employee);
            }
            catch (Exception)
            {
                // Log error or handle accordingly
                continue;
            }
        }
        
        return employees;
        });
    }
    
    private string GetCellValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            if (columnMap.TryGetValue(name, out var col))
            {
                var value = worksheet.Cells[row, col].Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
        }
        return string.Empty;
    }
    
    private DateTime GetDateValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            if (columnMap.TryGetValue(name, out var col))
            {
                var cellValue = worksheet.Cells[row, col].Value;
                
                if (cellValue is DateTime dateTime)
                    return dateTime;
                
                if (cellValue != null && DateTime.TryParse(cellValue.ToString(), out var parsedDate))
                    return parsedDate;
            }
        }
        return DateTime.Now;
    }
    
    private decimal GetDecimalValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            if (columnMap.TryGetValue(name, out var col))
            {
                var cellValue = worksheet.Cells[row, col].Value;
                
                if (cellValue is double doubleValue)
                    return (decimal)doubleValue;
                
                if (cellValue is decimal decimalValue)
                    return decimalValue;
                
                if (cellValue != null && decimal.TryParse(cellValue.ToString(), out var parsedDecimal))
                    return parsedDecimal;
            }
        }
        return 0;
    }
    
    private int GetStatusValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        var statusText = GetCellValue(worksheet, row, columnMap, possibleNames).ToLower();
        
        return statusText switch
        {
            "Activo" => (int)EmployeeStatus.Active,
            "Inactivo" => (int)EmployeeStatus.Inactive,
            "Vacaciones" => (int)EmployeeStatus.Vacation,
            _ => (int)EmployeeStatus.Active
        };
    }
    
    private int GetEducationLevelValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        var educationText = GetCellValue(worksheet, row, columnMap, possibleNames).ToLower();
        
        return educationText switch
        {
            "Bachiller" or "Secundaria" => (int)EducationLevel.HighSchool,
            "Tecnico" or "Técnico" => (int)EducationLevel.Technical,
            "Tecnólogo" or "Tecnologo" => (int)EducationLevel.Technologist,
            "Pregrado" or "Universitario" or "Profesional" => (int)EducationLevel.Professional,
            "Especialización" or "Especializacion" or "Especialidad" => (int)EducationLevel.Specialization,
            "Posgrado" or "Postgrado" or "Maestria" or "Maestría" or "Magister" => (int)EducationLevel.Master,
            "Doctorado" or "phd" => (int)EducationLevel.Doctorate,
            _ => (int)EducationLevel.HighSchool
        };
    }
}

