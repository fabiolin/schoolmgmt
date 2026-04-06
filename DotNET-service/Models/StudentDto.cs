namespace StudentReportService.Models;

public record StudentDto(
    int Id,
    string? Name,
    string? Email,
    bool SystemAccess,
    string? Phone,
    string? Gender,
    string? Dob,
    string? Class,
    string? Section,
    int? Roll,
    string? FatherName,
    string? FatherPhone,
    string? MotherName,
    string? MotherPhone,
    string? GuardianName,
    string? GuardianPhone,
    string? RelationOfGuardian,
    string? CurrentAddress,
    string? PermanentAddress,
    string? AdmissionDate,
    string? ReporterName
);
