using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagementSystem.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
            builder.ToTable("employees");

            builder.HasKey(e => e.Document);
            builder.Property(e => e.Document)
                .HasColumnName("document")
                .IsRequired()
                .HasMaxLength(50);

            // Owned types (Value Objects)
            builder.OwnsOne(e => e.FullName, fn =>
            {
                fn.Property(p => p.FirstName).HasColumnName("first_name").IsRequired().HasMaxLength(100);
                fn.Property(p => p.LastName).HasColumnName("last_name").IsRequired().HasMaxLength(100);
            });

            builder.OwnsOne(e => e.ContactInfo, ci =>
            {
                ci.Property(p => p.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50);
                ci.Property(p => p.Email).HasColumnName("email_address").HasMaxLength(150);
                ci.Property(p => p.Address).HasColumnName("address").HasMaxLength(500);
                ci.Property(p => p.City).HasColumnName("city").HasMaxLength(100);
            });

            builder.OwnsOne(e => e.JobInfo, ji =>
            {
                ji.Property(p => p.JobTitle).HasColumnName("job_title").HasMaxLength(200);
                ji.Property(p => p.Salary).HasColumnName("salary").HasColumnType("numeric(18,2)");
                ji.Property(p => p.HiringDate).HasColumnName("hire_date");
                ji.Property(p => p.DepartmentName).HasColumnName("department_name").HasMaxLength(200);
            });

            builder.OwnsOne(e => e.EducationInfo, ei =>
            {
                ei.Property(p => p.Level).HasColumnName("education_level").HasMaxLength(100);
                ei.Property(p => p.Institution).HasColumnName("institution").HasMaxLength(200);
                ei.Property(p => p.GraduationYear).HasColumnName("graduation_year");
            });

            builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            builder.Property(e => e.DepartmentId).HasColumnName("department_id");
            builder.Property(e => e.JobPositionId).HasColumnName("job_position_id");
            builder.Property(e => e.CreatedAt).HasColumnName("created_at");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            // Relationships
            builder.HasOne(e => e.Department)
                   .WithMany()
                   .HasForeignKey(e => e.DepartmentId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.JobPosition)
                   .WithMany()
                   .HasForeignKey(e => e.JobPositionId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(e => e.Document).IsUnique();
            builder.HasIndex(e => e.DepartmentId);
            builder.HasIndex(e => e.JobPositionId);
    }
}