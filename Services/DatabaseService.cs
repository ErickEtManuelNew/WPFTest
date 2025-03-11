using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFTest.Data;
using WPFTest.Models;

namespace WPFTest.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public DatabaseService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;

            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize database: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public async Task<UserAccount?> AuthenticateAsync(string username, string password)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Authentication failed: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string password)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                if (await context.Users.AnyAsync(u => u.Username == username || u.Email == email))
                {
                    return false;
                }

                var user = new UserAccount
                {
                    Username = username,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = UserRole.Viewer,
                    IsActive = false,
                    VerificationToken = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> VerifyUserAsync(string token)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var user = await context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = true;
                user.VerificationToken = null;
                user.VerifiedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Verification failed: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<IEnumerable<UserAccount>> GetUsersAsync(UserRole? role = null, string? searchText = null)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var query = context.Users.AsQueryable();

                if (role.HasValue)
                {
                    query = query.Where(u => u.Role == role.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var search = searchText.Trim().ToLower();
                    query = query.Where(u => u.Username.Contains(search, StringComparison.CurrentCultureIgnoreCase) || 
                                           u.Email.Contains(search, StringComparison.CurrentCultureIgnoreCase));
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve users: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<UserAccount>();
            }
        }

        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var user = await context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsActive = !user.IsActive;
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to toggle user status: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesByTypeAsync(CategoryType type)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                return await context.Categories
                    .Where(c => c.Type == type)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve categories: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Category>();
            }
        }

        public async Task<IEnumerable<Article>> GetArticlesAsync(int? categoryId = null, string? searchText = null)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var query = context.Articles
                    .Include(a => a.Category)
                    .AsQueryable();

                if (categoryId.HasValue)
                {
                    query = query.Where(a => a.CategoryId == categoryId.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var search = searchText.Trim().ToLower();
                    query = query.Where(a => a.Title.ToLower().Contains(search) || 
                                           a.Description.ToLower().Contains(search));
                }

                return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve articles: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Article>();
            }
        }

        public async Task<bool> AddArticleAsync(Article article)
        {
            if (article == null)
            {
                return false;
            }

            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                article.CreatedAt = DateTime.UtcNow;
                context.Articles.Add(article);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add article: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> UpdateArticleAsync(Article article)
        {
            if (article == null)
            {
                return false;
            }

            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var existingArticle = await context.Articles.FindAsync(article.Id);
                if (existingArticle == null)
                {
                    return false;
                }

                existingArticle.Title = article.Title;
                existingArticle.Description = article.Description;
                existingArticle.CategoryId = article.CategoryId;
                existingArticle.Price = article.Price;
                existingArticle.Stock = article.Stock;

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update article: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> DeleteArticleAsync(int articleId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var article = await context.Articles.FindAsync(articleId);
                if (article == null)
                {
                    return false;
                }

                context.Articles.Remove(article);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete article: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                return await context.Users.AnyAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check username: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                return await context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check email: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> AddUserAsync(UserAccount user)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                if (await context.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(user.VerificationToken))
                {
                    user.VerificationToken = Guid.NewGuid().ToString();
                }
                
                user.CreatedAt = DateTime.UtcNow;
                context.Users.Add(user);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add user: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}