using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Domain.ValueObjects;

public class EducationInfo
{
    public EducationLevel Level { get; }

    public EducationInfo(EducationLevel level)
    {
        Level = level;
    }
}
