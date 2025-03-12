using System.Threading.Tasks;
using WPFTest.Data;

namespace WPFTest.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IUserRepository? _users;
        private IArticleRepository? _articles;
        private ICategoryRepository? _categories;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IArticleRepository Articles => _articles ??= new ArticleRepository(_context);
        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
