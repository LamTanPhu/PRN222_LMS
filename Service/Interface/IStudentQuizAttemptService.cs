using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IStudentQuizAttemptService
    {
        Task<List<StudentQuizAttempt>> GetAllAsync();
        Task<StudentQuizAttempt> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
    }
}
