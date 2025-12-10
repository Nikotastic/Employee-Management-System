using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Domain.ValueObjects;

public class EducationInfo
{
    public EducationLevel Level { get; private set; }

    private EducationInfo()
    {
        Level = EducationLevel.HighSchool;
    }

    public EducationInfo(EducationLevel level)
    {
        Level = level;
    }
}
