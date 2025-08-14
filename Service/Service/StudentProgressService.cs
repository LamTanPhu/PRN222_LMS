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
    public class StudentProgressService : IStudentProgressService
    {
        private readonly StudentProgressRepository studentProgressRepository;

        public StudentProgressService()
        {
            studentProgressRepository = new StudentProgressRepository();
        }

        public async Task<List<StudentProgress>> GetAllAsync()
        {
            return await studentProgressRepository.GetAllAsync();
        }

        public async Task<StudentProgress> GetByIdAsync(int? id)
        {
            return await studentProgressRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var studentProgress = await GetByIdAsync(id);
            if (studentProgress != null)
            {
                return await studentProgressRepository.RemoveAsync(studentProgress);
            }
            return false;
        }
    }
}
