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
        Task<Course> CreateAsync(Course course, int id);
        Task UpdateAsync(Course course);
        Task<List<Course>> GetCoursesForAdminAsync();

    }
}
