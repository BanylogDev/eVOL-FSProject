using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.RepositoriesInteraces
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByName(string email);
        Task<User?> GetUserByEmail(string email);
        void RemoveUser(User user);
    }
}
