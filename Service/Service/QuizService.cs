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
    public class QuizService : IQuizService
    {
        private readonly QuizRepository quizRepository;

        public QuizService()
        {
            quizRepository = new QuizRepository();
        }

        public async Task<List<Quiz>> GetAllAsync()
        {
            return await quizRepository.GetAllAsync();
        }

        public async Task<Quiz> GetByIdAsync(int? id)
        {
            return await quizRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<Quiz> GetByIdAsync(int id)
        {
            return await quizRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Quiz quiz)
        {
            var result = await quizRepository.CreateAsync(quiz);
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Quiz quiz)
        {
            var result = await quizRepository.UpdateAsync(quiz);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var quiz = await GetByIdAsync(id);
            if (quiz != null)
            {
                return await quizRepository.RemoveAsync(quiz);
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quiz = await GetByIdAsync(id);
            if (quiz != null)
            {
                return await quizRepository.RemoveAsync(quiz);
            }
            return false;
        }
        public async Task CreateAsync(Quiz quiz, int lessonId)
        {
            quiz.LessonId = lessonId;
            await quizRepository.CreateAsync(quiz);
        }
    }
}
