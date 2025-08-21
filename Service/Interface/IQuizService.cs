using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IQuizService
    {
        Task<List<Quiz>> GetAllAsync();
        Task<Quiz> GetByIdAsync(int? id);
        Task<Quiz> GetByIdAsync(int id);
        Task<bool> CreateAsync(Quiz quiz);
        Task<bool> UpdateAsync(Quiz quiz);
        Task<bool> DeleteAsync(int? id);
        Task<bool> DeleteAsync(int id);

        Task CreateAsync(Quiz quiz, int lessonId);
        Task<bool> AddQuestionToQuizAsync(int quizId, QuizQuestion question);
    }
}
