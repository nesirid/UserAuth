using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AuthService.Core.Entities;

namespace AuthService.DAL.Configurations
{
    public class LoginLogConfiguration : IEntityTypeConfiguration<LoginLog>
    {
        public void Configure(EntityTypeBuilder<LoginLog> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.LoginDate)
                .IsRequired();

            modelBuilder.Property(e => e.IsSucceed)
                .IsRequired();

            modelBuilder.Property(e => e.IP)
                .HasMaxLength(45);

            modelBuilder.HasOne(e => e.User)
                .WithMany(u => u.LoginLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
