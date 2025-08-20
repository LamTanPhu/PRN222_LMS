using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IQuizAnswerService
    {
        Task<List<QuizAnswer>> GetAllAsync();
        Task<QuizAnswer> GetByIdAsync(int? id);
        Task<QuizAnswer> GetByIdAsync(int id);
        Task<bool> CreateAsync(QuizAnswer answer);
        Task<bool> UpdateAsync(QuizAnswer answer);
        Task<bool> DeleteAsync(int? id);
        Task<bool> DeleteAsync(int id);
    }
}
