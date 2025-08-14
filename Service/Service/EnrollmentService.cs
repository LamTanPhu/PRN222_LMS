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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly EnrollmentRepository enrollmentRepository;

        public EnrollmentService(EnrollmentRepository enrollmentRepository)
        {
            this.enrollmentRepository = enrollmentRepository;
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await enrollmentRepository.GetAllAsync();
        }

        public async Task<Enrollment> GetByIdAsync(int? id)
        {
            return await enrollmentRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var enrollment = await GetByIdAsync(id);
            if (enrollment != null)
            {
                return await enrollmentRepository.RemoveAsync(enrollment);
            }
            return false;
        }
    }
}
