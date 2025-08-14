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
            var studentQuizAttempt = await GetByIdAsync(id);
            if (studentQuizAttempt != null)
            {
                return await studentQuizAttemptRepository.RemoveAsync(studentQuizAttempt);
            }
            return false;
        }
    }
}
