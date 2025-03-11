using Microsoft.Extensions.DependencyInjection;

namespace WPFTest.Data
{
    public interface IDbContextFactory
    {
        ApplicationDbContext CreateDbContext();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ApplicationDbContext CreateDbContext()
        {
            return _serviceProvider.GetRequiredService<ApplicationDbContext>();
        }
    }
}