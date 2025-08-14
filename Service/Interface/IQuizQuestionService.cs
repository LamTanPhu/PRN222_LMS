using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IQuizQuestionService
    {
        Task<List<QuizQuestion>> GetAllAsync();
        Task<QuizQuestion> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
    }
}
