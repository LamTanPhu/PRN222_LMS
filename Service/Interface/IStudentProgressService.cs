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
    }
}
