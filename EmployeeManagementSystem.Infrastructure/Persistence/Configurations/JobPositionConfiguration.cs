using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagementSystem.Infrastructure.Persistence.Configurations;

public class JobPositionConfiguration : IEntityTypeConfiguration<JobPosition>
{
    public void Configure(EntityTypeBuilder<JobPosition> builder)
    {
        builder.ToTable("job_positions");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200).HasColumnName("title");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
    }
}