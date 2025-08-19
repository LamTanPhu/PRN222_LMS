using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IForumService
    {
        Task<List<Forum>> GetAllAsync();
        Task<Forum> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(Forum forum);
        Task UpdateAsync(Forum forum);
    }
}
