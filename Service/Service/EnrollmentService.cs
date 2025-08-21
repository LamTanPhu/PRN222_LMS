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

        public async Task<bool> CreateAsync(Enrollment enrollment)
        {
            var result = await enrollmentRepository.CreateAsync(enrollment);
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Enrollment enrollment)
        {
            var result = await enrollmentRepository.UpdateAsync(enrollment);
            return result > 0;
        }

        public async Task<List<Enrollment>> GetEnrollmentsByUserAsync(int userId)
        {
            return await enrollmentRepository.GetEnrollmentsByUserAsync(userId);
        }

        public async Task<Enrollment> GetByUserAndCourseAsync(int userId, int courseId)
        {
            return await enrollmentRepository.GetByUserAndCourseAsync(userId, courseId);
        }

        public async Task EnrollAsync(Enrollment enrollment)
        {
            enrollment.EnrollmentDate = DateTime.Now;
            enrollment.Status = "Active";
            enrollment.PaymentStatus = "Paid";
            enrollment.ProgressPercentage = 0;

            await enrollmentRepository.AddEnrollmentAsync(enrollment);
        }

        public async Task<bool> IsEnrolledAsync(int userId, int courseId)
        {
            var e = await enrollmentRepository.GetByUserAndCourseAsync(userId, courseId);
            return e != null;
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
