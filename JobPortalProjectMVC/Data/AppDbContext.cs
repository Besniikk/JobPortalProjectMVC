using JobPortalProjectMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobPortalProjectMVC.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        {
        }
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<JobComment> JobComments { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure JobComment relationships
            modelBuilder.Entity<JobComment>()
                .HasKey(jc => jc.CommentId); // Primary key

            modelBuilder.Entity<JobComment>()
                .HasOne(jc => jc.JobPost)
                .WithMany(jp => jp.JobComments) // A JobPost can have many comments
                .HasForeignKey(jc => jc.JobPostId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete

            modelBuilder.Entity<JobComment>()
                .HasOne(jc => jc.User)
                .WithMany() // A User can comment on multiple JobPosts
                .HasForeignKey(jc => jc.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete


            modelBuilder.Entity<JobApplication>()
        .HasKey(ja => ja.ApplicationId);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.JobPost)
                .WithMany(jp => jp.JobApplications) // A JobPost can have multiple applications
                .HasForeignKey(ja => ja.JobPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.User)
                .WithMany() // A User can submit multiple applications
                .HasForeignKey(ja => ja.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }

}