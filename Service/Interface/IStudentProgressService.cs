using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IStudentProgressService
    {
        Task<List<StudentProgress>> GetAllAsync();
        Task<StudentProgress> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task UpdateProgressAsync(int userId, int courseId, int lessonId, bool isCompleted);
        Task<List<StudentProgress>> GetStudentProgressesByUserAsync(int userId);

        Task<object> GetCourseProgressAsync(int userId, int courseId);  

    }
}
