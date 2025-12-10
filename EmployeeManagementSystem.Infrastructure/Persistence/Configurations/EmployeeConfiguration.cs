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
                
            builder.Property(e => e.Address)
                .HasColumnName("address")
                .HasMaxLength(500);
                
            builder.Property(e => e.BirthDate)
                .HasColumnName("hire_date");
                
            builder.Property(e => e.ProfessionalProfile)
                .HasColumnName("job_title")
                .HasMaxLength(200);
                
            builder.Property(e => e.DepartmentId)
                .HasColumnName("department_id");

            // Owned types (Value Objects)
            builder.OwnsOne(e => e.FullName, fn =>
            {
                fn.Property(p => p.FirstName).HasColumnName("first_name").IsRequired().HasMaxLength(100);
                fn.Property(p => p.LastName).HasColumnName("last_name").IsRequired().HasMaxLength(100);
            });

            builder.OwnsOne(e => e.ContactInfo, ci =>
            {
                ci.Property(p => p.Phone).HasColumnName("phone_number").HasMaxLength(50);
                ci.Property(p => p.Email).HasColumnName("email_address").HasMaxLength(150);
            });

            builder.OwnsOne(e => e.JobInfo, ji =>
            {
                ji.Property(p => p.JobPositionId).HasColumnName("job_position_id");
                ji.Property(p => p.Salary).HasColumnName("salary").HasColumnType("numeric(18,2)");
                ji.Property(p => p.HiringDate).HasColumnName("created_at");
                ji.Property(p => p.Status).HasColumnName("status").HasConversion<string>();
            });

            builder.OwnsOne(e => e.EducationInfo, ei =>
            {
                ei.Property(p => p.Level).HasColumnName("education_level").HasConversion<string>();
            });
            
            // Mapear columnas shadow properties (columnas en BD que no están en el modelo)
            builder.Property<string>("city").HasColumnName("city").HasMaxLength(100);
            builder.Property<string>("department_name").HasColumnName("department_name").HasMaxLength(200);
            builder.Property<string>("institution").HasColumnName("institution").HasMaxLength(200);
            builder.Property<int?>("graduation_year").HasColumnName("graduation_year");
            builder.Property<DateTime?>("updated_at").HasColumnName("updated_at");

            // Relationships
            builder.HasOne(e => e.Department)
                   .WithMany()
                   .HasForeignKey(e => e.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => e.Document).IsUnique();
            builder.HasIndex(e => e.DepartmentId);
    }
}