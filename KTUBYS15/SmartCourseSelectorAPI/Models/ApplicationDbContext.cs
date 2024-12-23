using Microsoft.EntityFrameworkCore;
using StudentIMS.Models;

namespace SmartCourseSelectorWeb.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourseSelection> StudentCourseSelections { get; set; }
        public DbSet<Transcript> Transcripts { get; set; }
        public DbSet<CourseQuota> CourseQuotas { get; set; }
        public DbSet<UnapprovedSelections> UnapprovedSelections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UnapprovedSelections>()
           .ToTable("UnapprovedSelections");

            modelBuilder.Entity<StudentCourseSelection>(entity =>
            {

                entity.HasKey(e => e.SelectionID);
                entity.HasOne(e => e.Student)
                     .WithMany(s => s.StudentCourseSelections)
                     .HasForeignKey(e => e.StudentID);

                entity.HasOne(e => e.Course)
                     .WithMany(c => c.StudentCourseSelections)
                     .HasForeignKey(e => e.CourseID);

                modelBuilder.Entity<Student>()
                    .HasOne(s => s.Advisor)
                    .WithMany(a => a.Students)
                    .HasForeignKey(s => s.AdvisorID)
                    .OnDelete(DeleteBehavior.Restrict);
                modelBuilder.Entity<Advisor>()
                    .HasMany(a => a.Students)
                    .WithOne(s => s.Advisor)
                    .HasForeignKey(s => s.AdvisorID)
                    .OnDelete(DeleteBehavior.Restrict);
                modelBuilder.Entity<Student>()
                    .HasMany(s => s.StudentCourseSelections)
                    .WithOne(scs => scs.Student)
                    .HasForeignKey(scs => scs.StudentID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<CourseQuota>()
                    .HasOne(cq => cq.Course)
                    .WithOne()
                    .HasForeignKey<CourseQuota>(cq => cq.CourseId);
            });
        }
    }
}