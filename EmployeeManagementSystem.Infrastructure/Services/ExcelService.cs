using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Enums;
using OfficeOpenXml;
using System.Globalization;
using System.Text;

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
        
        // Column Mapping in Spanish (normalized keys)
        var columnMap = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
        {
            var headerValue = worksheet.Cells[1, col].Value?.ToString();
            var key = NormalizeKey(headerValue);
            if (!string.IsNullOrEmpty(key))
            {
                columnMap[key] = col;
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
                    ProfessionalProfile = GetCellValue(worksheet, row, columnMap, "perfil profesional", "perfil", "descripcion", "Perfil Profesional", "PerfilProfesional", "perfilprofesional"),
                    Status = GetStatusValue(worksheet, row, columnMap, "estado", "Estado"),
                    EducationLevel = GetEducationLevelValue(worksheet, row, columnMap, "nivel educativo", "educacion", "educación", "Nivel Educativo", "NivelEducativo", "niveleducativo"),
                    JobPositionId = 0, 
                    DepartmentId = 0,
                    JobPositionName = GetCellValue(worksheet, row, columnMap, "cargo", "posicion", "puesto", "Cargo"),
                    DepartmentName = GetCellValue(worksheet, row, columnMap, "departamento", "area", "área", "Departamento")
                };

                // Debug: Log the professional profile value
                Console.WriteLine($"Row {row} - Document: {employee.Document}, ProfessionalProfile: '{employee.ProfessionalProfile}'");

                // Fallback for missing Document (Generate from Email or Random)
                if (string.IsNullOrEmpty(employee.Document))
                {
                    if (!string.IsNullOrEmpty(employee.Email))
                    {
                         // Generate a numeric string from email hash to simulate a document ID
                         employee.Document = Math.Abs(employee.Email.GetHashCode()).ToString();
                    }
                    else
                    {
                         // Random 8 digit number
                         employee.Document = new Random().Next(10000000, 99999999).ToString();
                    }
                }

                // Fallback for missing Department
                if (string.IsNullOrEmpty(employee.DepartmentName))
                {
                    employee.DepartmentName = "General";
                }
                
                employees.Add(employee);
            }
            catch (Exception ex)
            {
                // Log error or handle accordingly
                 Console.WriteLine($"Error importing row {row}: {ex.Message} - {ex.StackTrace}");
                continue;
            }
        }
        
        return employees;
        });
    }
    
    private string NormalizeKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var lower = value.Trim().ToLowerInvariant();
        var normalized = lower.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString();
    }

    private string GetCellValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            var key = NormalizeKey(name);
            if (columnMap.TryGetValue(key, out var col))
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
            var key = NormalizeKey(name);
            if (columnMap.TryGetValue(key, out var col))
            {
                var cellValue = worksheet.Cells[row, col].Value;
                
                if (cellValue is DateTime dateTime)
                    return dateTime;
                
                // Handle Excel numeric dates (double)
                if (cellValue is double doubleDate)
                {
                    try 
                    {
                        return DateTime.FromOADate(doubleDate);
                    }
                    catch {}
                }

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
            var key = NormalizeKey(name);
            if (columnMap.TryGetValue(key, out var col))
            {
                var cellValue = worksheet.Cells[row, col].Value;
                
                if (cellValue is double doubleValue)
                    return (decimal)doubleValue;
                
                if (cellValue is decimal decimalValue)
                    return decimalValue;
                
                if (cellValue != null && decimal.TryParse(cellValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedDecimal))
                    return parsedDecimal;

                // try parse with current culture as fallback
                if (cellValue != null && decimal.TryParse(cellValue.ToString(), out var parsedDecimal2))
                    return parsedDecimal2;
            }
        }
        return 0;
    }
    
    private int GetStatusValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        var statusText = NormalizeKey(GetCellValue(worksheet, row, columnMap, possibleNames));
        
        return statusText switch
        {
            "activo" => (int)EmployeeStatus.Active,
            "inactivo" => (int)EmployeeStatus.Inactive,
            "vacaciones" => (int)EmployeeStatus.Vacation,
            _ => (int)EmployeeStatus.Active
        };
    }
    
    private int GetEducationLevelValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        var educationText = NormalizeKey(GetCellValue(worksheet, row, columnMap, possibleNames));
        
        return educationText switch
        {
            "bachiller" or "secundaria" => (int)EducationLevel.HighSchool,
            "tecnico" or "técnico" => (int)EducationLevel.Technical,
            "tecnologo" => (int)EducationLevel.Technologist,
            "pregrado" or "universitario" or "profesional" => (int)EducationLevel.Professional,
            "especializacion" or "especialidad" => (int)EducationLevel.Specialization,
            "posgrado" or "postgrado" or "maestria" or "magister" => (int)EducationLevel.Master,
            "doctorado" or "phd" => (int)EducationLevel.Doctorate,
            _ => (int)EducationLevel.HighSchool
        };
    }
}
