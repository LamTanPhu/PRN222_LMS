using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IEnrollmentService
    {
        Task<List<Enrollment>> GetAllAsync();
        Task<Enrollment> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(Enrollment enrollment);
        Task<List<Enrollment>> GetEnrollmentsByUserAsync(int userId);
        Task EnrollAsync(int courseId, int userId);
        Task EnrollAsync(Enrollment enrollment);
        Task<bool> IsEnrolledAsync(int userId, int courseId);
    }
}
