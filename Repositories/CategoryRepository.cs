using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPFTest.Data;
using WPFTest.Models;

namespace WPFTest.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetCategoriesByTypeAsync(CategoryType type)
        {
            return await _dbSet
                .Where(c => c.Type == type)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
