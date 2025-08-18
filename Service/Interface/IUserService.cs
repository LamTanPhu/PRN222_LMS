using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task<User> LoginAsync(string email, string password);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);

    }
}