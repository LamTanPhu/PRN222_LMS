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
    public class StudentQuizAttemptService : IStudentQuizAttemptService
    {
        private readonly StudentQuizAttemptRepository studentQuizAttemptRepository;

        public StudentQuizAttemptService()
        {
            studentQuizAttemptRepository = new StudentQuizAttemptRepository();
        }

        public async Task<List<StudentQuizAttempt>> GetAllAsync()
        {
            return await studentQuizAttemptRepository.GetAllAsync();
        }

        public async Task<StudentQuizAttempt> GetByIdAsync(int? id)
        {
            return await studentQuizAttemptRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var attempt = await GetByIdAsync(id);
            if (attempt != null)
            {
                return await studentQuizAttemptRepository.RemoveAsync(attempt);
            }
            return false;
        }

        public async Task<bool> CreateAsync(StudentQuizAttempt attempt)
        {
            var result = await studentQuizAttemptRepository.CreateAsync(attempt);
            return result > 0;
        }

        public async Task RecordAttemptAsync(int userId, int quizId, int score)
        {
            var attempt = new StudentQuizAttempt
            {
                UserId = userId,
                QuizId = quizId,
                Score = score
            };
            await studentQuizAttemptRepository.CreateAsync(attempt);
        }

        public async Task<List<StudentQuizAttempt>> GetAttemptsByUserAsync(int userId)
        {
            return await studentQuizAttemptRepository.GetAttemptsByUserAsync(userId);
        }
    }
}
