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
        Task<bool> CreateAsync(StudentQuizAttempt attempt);
        Task RecordAttemptAsync(int userId, int quizId, int score);
        Task<List<StudentQuizAttempt>> GetAttemptsByUserAsync(int userId);
    }
}
