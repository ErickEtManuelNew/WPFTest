using System.Threading.Tasks;
using WPFTest.Models;

namespace WPFTest.Repositories
{
    public interface IUserRepository : IRepository<UserAccount>
    {
        Task<UserAccount?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<UserAccount?> GetByUsernameAsync(string username);
        Task<UserAccount?> AuthenticateAsync(string username, string password);
        Task<bool> VerifyUserAsync(string token);
    }
}
