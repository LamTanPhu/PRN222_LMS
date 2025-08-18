using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllAsync();
        Task<Course> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(Course course);
        Task UpdateAsync(Course course);

    }
}
