using Repository.Models;
using Repository.Repositories;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service.Service
{
    public class QuizQuestionService : IQuizQuestionService
    {
        private readonly QuizQuestionRepository quizQuestionRepository;

        public QuizQuestionService()
        {
            quizQuestionRepository = new QuizQuestionRepository();
        }

        public async Task<List<QuizQuestion>> GetAllAsync()
        {
            return await quizQuestionRepository.GetAllAsync();
        }

        public async Task<QuizQuestion> GetByIdAsync(int? id)
        {
            return await quizQuestionRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<List<QuizQuestion>> GetByQuizIdAsync(int quizId)
        {
            var context = new Repository.DBContext.CourseraStyleLMSContext();
            return await context.QuizQuestions
                .Where(q => q.QuizId == quizId)
                .Include(q => q.QuizAnswers)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var quizQuestion = await GetByIdAsync(id);
            if (quizQuestion != null)
            {
                return await quizQuestionRepository.RemoveAsync(quizQuestion);
            }
            return false;
        }
    }
}
