using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(Category category);
        Task UpdateAsync(Category category);

    }
}
