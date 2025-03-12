using System;
using System.Threading.Tasks;

namespace WPFTest.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IArticleRepository Articles { get; }
        ICategoryRepository Categories { get; }
        Task<int> SaveChangesAsync();
    }
}
