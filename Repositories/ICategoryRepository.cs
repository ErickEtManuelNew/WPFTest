using System.Collections.Generic;
using System.Threading.Tasks;
using WPFTest.Models;

namespace WPFTest.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesByTypeAsync(CategoryType type);
    }
}
