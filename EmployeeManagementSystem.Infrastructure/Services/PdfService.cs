using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EmployeeManagementSystem.Infrastructure.Services;

public class PdfService : IPdfService
{
    public PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateEmployeeCvPdfAsync(EmployeeDto employee)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .AlignCenter()
                        .Text("HOJA DE VIDA")
                        .FontSize(20)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            // Datos Personales
                            column.Item().Element(c => SectionHeader(c, "DATOS PERSONALES"));
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Nombre: {employee.FirstName} {employee.LastName}").Bold();
                                    col.Item().Text($"Documento: {employee.Document}");
                                    col.Item().Text($"Fecha de Nacimiento: {employee.BirthDate:dd/MM/yyyy}");
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Dirección: {employee.Address}");
                                    col.Item().Text($"Teléfono: {employee.Phone}");
                                    col.Item().Text($"Email: {employee.Email}");
                                });
                            });

                            // Información Laboral
                            column.Item().Element(c => SectionHeader(c, "INFORMACIÓN LABORAL"));
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Cargo: {employee.JobPositionName}").Bold();
                                    col.Item().Text($"Departamento: {employee.DepartmentName}");
                                    col.Item().Text($"Fecha de Ingreso: {employee.HiringDate:dd/MM/yyyy}");
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Salario: ${employee.Salary:N2}");
                                    col.Item().Text($"Estado: {employee.Status}");
                                });
                            });

                            // Educación
                            column.Item().Element(c => SectionHeader(c, "NIVEL EDUCATIVO"));
                            column.Item().Text(employee.EducationLevel);

                            // Perfil Profesional
                            column.Item().Element(c => SectionHeader(c, "PERFIL PROFESIONAL"));
                            column.Item().Text(employee.ProfessionalProfile)
                                .Justify();
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generado por TalentoPlus S.A.S - {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1);
                });
            });

            return document.GeneratePdf();
        });
    }

    private static void SectionHeader(IContainer container, string text)
    {
        container
            .PaddingTop(10)
            .PaddingBottom(5)
            .BorderBottom(1)
            .BorderColor(Colors.Blue.Darken2)
            .Text(text)
            .FontSize(14)
            .Bold()
            .FontColor(Colors.Blue.Darken2);
    }
}

