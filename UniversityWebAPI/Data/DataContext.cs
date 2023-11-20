using Microsoft.EntityFrameworkCore;
using UniversityWebAPI.Models;

namespace UniversityWebAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<StudentUniversity> StudentUniversities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentUniversity>()
                .HasKey(su => new { su.StudentId, su.UniversityId });
            modelBuilder.Entity<StudentUniversity>()
                .HasOne(s => s.Student)
                .WithMany(su => su.StudentUniversities)
                .HasForeignKey(s => s.StudentId);
            modelBuilder.Entity<StudentUniversity>()
                .HasOne(u => u.University)
                .WithMany(su => su.StudentUniversities)
                .HasForeignKey(u => u.UniversityId);
        }
    }
}
