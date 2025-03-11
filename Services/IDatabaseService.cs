using System.Collections.Generic;
using System.Threading.Tasks;
using WPFTest.Models;

namespace WPFTest.Services
{
    public interface IDatabaseService
    {
        Task<UserAccount?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterUserAsync(string username, string email, string password);
        Task<bool> VerifyUserAsync(string token);
        Task<IEnumerable<UserAccount>> GetUsersAsync(UserRole? role = null, string? searchText = null);
        Task<bool> ToggleUserStatusAsync(int userId);
        Task<IEnumerable<Category>> GetCategoriesByTypeAsync(CategoryType type);
        Task<IEnumerable<Article>> GetArticlesAsync(int? categoryId = null, string? searchText = null);
        Task<bool> AddArticleAsync(Article article);
        Task<bool> UpdateArticleAsync(Article article);
        Task<bool> DeleteArticleAsync(int articleId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> AddUserAsync(UserAccount user);
    }
}
