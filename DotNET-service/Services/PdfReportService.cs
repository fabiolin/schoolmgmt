using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudentReportService.Models;

namespace StudentReportService.Services;

public interface IPdfReportService
{
    byte[] GenerateStudentReport(StudentDto student);
}

public sealed class PdfReportService : IPdfReportService
{
    public byte[] GenerateStudentReport(StudentDto student)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                // ── Header ────────────────────────────────────────────────
                page.Header().Column(header =>
                {
                    header.Item()
                        .Text("Student Report")
                        .Bold().FontSize(22).FontColor("#1565c0");

                    header.Item()
                        .PaddingTop(2)
                        .Text($"Generated on {DateTime.UtcNow:dd MMM yyyy HH:mm} UTC")
                        .FontSize(9).FontColor("#78909c");

                    header.Item()
                        .PaddingTop(6)
                        .Height(2)
                        .Background("#1565c0");
                });

                // ── Content ───────────────────────────────────────────────
                page.Content()
                    .PaddingTop(16)
                    .Column(col =>
                    {
                        col.Spacing(4);

                        AddSection(col, "Personal Information",
                        [
                            ("Full Name",      student.Name),
                            ("Email",          student.Email),
                            ("Phone",          student.Phone),
                            ("Gender",         student.Gender),
                            ("Date of Birth",  student.Dob),
                        ]);

                        AddSection(col, "Academic Details",
                        [
                            ("Class",              student.Class),
                            ("Section",            student.Section),
                            ("Roll Number",        student.Roll?.ToString()),
                            ("Admission Date",     student.AdmissionDate),
                            ("Reporting Teacher",  student.ReporterName),
                            ("System Access",      student.SystemAccess ? "Active" : "Inactive"),
                        ]);

                        AddSection(col, "Parent / Guardian Information",
                        [
                            ("Father's Name",         student.FatherName),
                            ("Father's Phone",        student.FatherPhone),
                            ("Mother's Name",         student.MotherName),
                            ("Mother's Phone",        student.MotherPhone),
                            ("Guardian's Name",       student.GuardianName),
                            ("Guardian's Phone",      student.GuardianPhone),
                            ("Relation of Guardian",  student.RelationOfGuardian),
                        ]);

                        AddSection(col, "Address",
                        [
                            ("Current Address",    student.CurrentAddress),
                            ("Permanent Address",  student.PermanentAddress),
                        ]);
                    });

                // ── Footer ────────────────────────────────────────────────
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ").FontSize(9).FontColor("#90a4ae");
                        text.CurrentPageNumber().FontSize(9).FontColor("#90a4ae");
                        text.Span(" of ").FontSize(9).FontColor("#90a4ae");
                        text.TotalPages().FontSize(9).FontColor("#90a4ae");
                    });
            });
        }).GeneratePdf();
    }

    private static void AddSection(
        ColumnDescriptor col,
        string title,
        (string Label, string? Value)[] fields)
    {
        col.Item().PaddingTop(14).Text(title)
            .SemiBold().FontSize(13).FontColor("#1976d2");

        col.Item().PaddingBottom(6).Height(1).Background("#bbdefb");

        foreach (var (label, value) in fields)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;

            col.Item().Row(row =>
            {
                row.ConstantItem(170)
                    .Text(label + ":")
                    .FontColor("#546e7a");

                row.RelativeItem()
                    .Text(value);
            });
        }
    }
}
