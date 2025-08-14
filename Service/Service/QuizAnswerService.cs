using Repository.Models;
using Repository.Repositories;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class QuizAnswerService : IQuizAnswerService
    {
        private readonly QuizAnswerRepository quizAnswerRepository;

        public QuizAnswerService()
        {
            quizAnswerRepository = new QuizAnswerRepository();
        }
        public async Task<List<QuizAnswer>> GetAllAsync()
        {
            return await quizAnswerRepository.GetAllAsync();
        }

        public async Task<QuizAnswer> GetByIdAsync(int? id)
        {
            return await quizAnswerRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var quizAnswer = await GetByIdAsync(id);
            if (quizAnswer != null)
            {
                return await quizAnswerRepository.RemoveAsync(quizAnswer);
            }
            return false;
        }
    }
}
