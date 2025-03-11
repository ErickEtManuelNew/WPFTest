using Microsoft.EntityFrameworkCore;
using System;
using WPFTest.Models;
using WPFTest.Services;

namespace WPFTest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserAccount> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Publication> Publications { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("UserAccounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasConversion<string>();
                entity.HasIndex(e => e.Username).IsUnique();

                // Seed admin user
                entity.HasData(new UserAccount
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@example.com",
                    Role = UserRole.Administrator,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                });
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Type).IsRequired().HasConversion<string>();

                // Seed article categories
                entity.HasData(
                    new Category { Id = 1, Name = "Electronics", Type = CategoryType.Article },
                    new Category { Id = 2, Name = "Books", Type = CategoryType.Article },
                    new Category { Id = 3, Name = "Clothing", Type = CategoryType.Article }
                );

                // Seed publication categories
                entity.HasData(
                    new Category { Id = 4, Name = "News", Type = CategoryType.Publication },
                    new Category { Id = 5, Name = "Blog", Type = CategoryType.Publication },
                    new Category { Id = 6, Name = "Press Release", Type = CategoryType.Publication }
                );
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("Articles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Stock).IsRequired();
                entity.Property(e => e.CategoryId).IsRequired();

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Articles)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Publication>(entity =>
            {
                entity.ToTable("Publications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired().HasConversion<string>();
                entity.Property(e => e.AuthorId).IsRequired();

                entity.HasOne(e => e.Author)
                      .WithMany(u => u.Publications)
                      .HasForeignKey(e => e.AuthorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Articles)
                      .WithMany(a => a.Publications)
                      .UsingEntity(j => j.ToTable("PublicationArticles"));
            });
        }
    }
}