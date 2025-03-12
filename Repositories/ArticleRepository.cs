using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPFTest.Data;
using WPFTest.Models;

namespace WPFTest.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        public ArticleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Article>> GetArticlesByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Publications)
                .ThenInclude(p => p.Author)
                .Where(a => a.Publications.Any(p => p.AuthorId == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetArticlesByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Publications)
                .ThenInclude(p => p.Author)
                .Where(a => a.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetFilteredArticlesAsync(int? categoryId, string? searchText)
        {
            var query = _dbSet
                .Include(a => a.Category)
                .Include(a => a.Publications)
                .ThenInclude(p => p.Author)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.Trim().ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(search));
            }

            return await query.ToListAsync();
        }
    }
}
