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
        Task<bool> DeleteAsync(int? id);
    }
}
