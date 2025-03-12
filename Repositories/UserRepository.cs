using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WPFTest.Data;
using WPFTest.Models;

namespace WPFTest.Repositories
{
    public class UserRepository : Repository<UserAccount>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserAccount?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserAccount?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<UserAccount?> AuthenticateAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null) return null;

            bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return validPassword ? user : null;
        }

        public async Task<bool> VerifyUserAsync(string token)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null) return false;

            user.IsActive = true;
            user.VerificationToken = null;
            user.VerifiedAt = DateTime.Now;
            
            Update(user);
            return true;
        }
    }
}
