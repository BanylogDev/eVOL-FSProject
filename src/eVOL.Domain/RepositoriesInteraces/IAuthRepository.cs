using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.RepositoriesInteraces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByName(string name);
        Task<User?> GetUserByEmail(string email);
        Task<User?> Register(User newUser);
        Task SaveChangesAsync();
    }
}
