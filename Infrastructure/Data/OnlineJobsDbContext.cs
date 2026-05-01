using Microsoft.EntityFrameworkCore;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.ValueObjects;

namespace OnlineJobs.Infrastructure.Data
{
    public class OnlineJobsDbContext : DbContext
    {
        public OnlineJobsDbContext(DbContextOptions<OnlineJobsDbContext> options) : base(options)
        {
            // Enable foreign keys for SQLite
            if (Database.IsSqlite())
            {
                Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<CompanyReveal> CompanyReveals { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<CategoryLeaf> CategoryLeaves { get; set; }
        public DbSet<CategoryComposite> CategoryComposites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User inheritance hierarchy (TPH - Table Per Hierarchy)
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<User>("User")
                .HasValue<JobSeeker>("JobSeeker")
                .HasValue<Employer>("Employer");

            modelBuilder.Entity<User>()
                .Property<string>("Discriminator")
                .HasMaxLength(50);

            // Configure JobCategory inheritance (TPH - Table Per Hierarchy)
            modelBuilder.Entity<JobCategory>()
                .HasDiscriminator<string>("CategoryType")
                .HasValue<CategoryLeaf>("CategoryLeaf")
                .HasValue<CategoryComposite>("CategoryComposite");

            // Company Configuration
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Description).HasMaxLength(2000);
                entity.Property(c => c.Website).HasMaxLength(500);
                entity.Property(c => c.Location).HasMaxLength(200);
                entity.Property(c => c.Industry).HasMaxLength(100);

                entity.HasMany(c => c.JobPostings)
                    .WithOne(j => j.Company)
                    .HasForeignKey(j => j.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Employers)
                    .WithOne(e => e.Company)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // JobPosting Configuration
            modelBuilder.Entity<JobPosting>(entity =>
            {
                entity.HasKey(j => j.Id);
                entity.Property(j => j.Title).IsRequired().HasMaxLength(200);
                entity.Property(j => j.Description).IsRequired().HasMaxLength(5000);
                entity.Property(j => j.Requirements).HasMaxLength(3000);
                entity.Property(j => j.Location).HasMaxLength(200);
                entity.Property(j => j.EmploymentType).HasMaxLength(50);
                entity.Property(j => j.Category).HasMaxLength(100);

                entity.HasMany(j => j.Applications)
                    .WithOne(a => a.JobPosting)
                    .HasForeignKey(a => a.JobPostingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // JobApplication Configuration
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.CoverLetter).HasMaxLength(3000);
                entity.Property(a => a.ResumeUrl).HasMaxLength(500);
                entity.Property(a => a.PortfolioLink).HasMaxLength(500);

                entity.HasOne(a => a.JobSeeker)
                    .WithMany(js => js.Applications)
                    .HasForeignKey(a => a.JobSeekerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // JobSeeker Configuration - Store value object collections as JSON
            modelBuilder.Entity<JobSeeker>(entity =>
            {
                entity.Property(js => js.Resume).HasMaxLength(5000);
                entity.Property(js => js.Skills).HasMaxLength(2000);
                entity.Property(js => js.ProfessionalSummary).HasMaxLength(2000);

                // Ignore value object collections - they won't be persisted in this simplified version
                entity.Ignore(js => js.SkillSet);
                entity.Ignore(js => js.EducationHistory);
                entity.Ignore(js => js.WorkHistory);
                entity.Ignore(js => js.Certifications);
            });

            // Employer Configuration
            modelBuilder.Entity<Employer>(entity =>
            {
                entity.Property(e => e.Position).HasMaxLength(100);
            });

            // PaymentTransaction Configuration
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(pt => pt.Id);
                entity.Property(pt => pt.ExternalTransactionId).HasMaxLength(100);
                entity.Property(pt => pt.Currency).HasMaxLength(3);
                entity.HasIndex(pt => pt.ExternalTransactionId);
            });

            // CompanyReveal Configuration
            modelBuilder.Entity<CompanyReveal>(entity =>
            {
                entity.HasKey(cr => cr.Id);
                entity.Property(cr => cr.JobSeekerId).IsRequired();
                entity.Property(cr => cr.JobPostingId).IsRequired();
            });

            // CategoryLeaf Configuration
            modelBuilder.Entity<CategoryLeaf>(entity =>
            {
                entity.HasMany(cl => cl.Jobs)
                    .WithOne()
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // CategoryComposite Configuration
            modelBuilder.Entity<CategoryComposite>(entity =>
            {
                entity.HasMany(cc => cc.Children)
                    .WithOne()
                    .HasForeignKey("ParentId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}