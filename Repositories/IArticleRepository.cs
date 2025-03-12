using System.Collections.Generic;
using System.Threading.Tasks;
using WPFTest.Models;

namespace WPFTest.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<IEnumerable<Article>> GetArticlesByUserIdAsync(int userId);
        Task<IEnumerable<Article>> GetArticlesByCategoryAsync(int categoryId);
        Task<IEnumerable<Article>> GetFilteredArticlesAsync(int? categoryId, string? searchText);
    }
}
