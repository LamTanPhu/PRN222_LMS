using Microsoft.EntityFrameworkCore;
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

        public EnrollmentService()
        {
            enrollmentRepository = new EnrollmentRepository();
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

        public async Task CreateAsync(Enrollment enrollment)
        {
            await enrollmentRepository.CreateAsync(enrollment);
        }

        public async Task<List<Enrollment>> GetEnrollmentsByUserAsync(int userId)
        {
            return await enrollmentRepository.GetEnrollmentsByUserAsync(userId);
        }

        public async Task EnrollAsync(int courseId, int userId)
        {
            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId
            };
            await enrollmentRepository.CreateAsync(enrollment);
        }
    }
}
