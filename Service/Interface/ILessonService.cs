using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ILessonService
    {
        Task<List<Lesson>> GetAllAsync();
        Task<Lesson> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(Lesson lesson, int courseId);
    }
}
