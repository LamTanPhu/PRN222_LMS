using Microsoft.EntityFrameworkCore;
using Repository.Basic;
using Repository.DBContext;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>
    {
        public EnrollmentRepository()
        {
        }

        public EnrollmentRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .ToListAsync();
        }

        public async Task<Enrollment> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);
        }

        public async Task<int> CreateAsync(Enrollment entity)
        {
            _context.Enrollments.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Enrollment entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Enrollments.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Enrollment entity)
        {
            _context.Enrollments.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Enrollment>> GetEnrollmentsByUserAsync(int userId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }
    }
}
