using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // 🔹 EKLENDİ
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 

using Week5.Models;

namespace Week5.Data
{
    // 🔹 IdentityDbContext'ten kalıtım alındı
    public class SchoolDbContext : IdentityDbContext<ApplicationUser>
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }

        public DbSet<Class> Classes { get; set; }

        // 🔹 Identity tabloları için key ayarları (isteğe bağlı)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(r => new { r.UserId, r.RoleId });

            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            // (İsteğe bağlı) Class tablosu özelleştirmeleri buraya eklenebilir
        }
    }
}
