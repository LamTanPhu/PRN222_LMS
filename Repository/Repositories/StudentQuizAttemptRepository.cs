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
    public class StudentQuizAttemptRepository : GenericRepository<StudentQuizAttempt>
    {
        public StudentQuizAttemptRepository()
        {
        }

        public StudentQuizAttemptRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<StudentQuizAttempt>> GetAllAsync()
        {
            return await _context.StudentQuizAttempts
                .Include(sqa => sqa.Quiz)
                .Include(sqa => sqa.User)
                .ToListAsync();
        }

        public async Task<StudentQuizAttempt> GetByIdAsync(int id)
        {
            return await _context.StudentQuizAttempts
                .Include(sqa => sqa.Quiz)
                .Include(sqa => sqa.User)
                .FirstOrDefaultAsync(sqa => sqa.AttemptId == id);
        }

        public async Task<int> CreateAsync(StudentQuizAttempt entity)
        {
            _context.StudentQuizAttempts.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(StudentQuizAttempt entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.StudentQuizAttempts.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(StudentQuizAttempt entity)
        {
            _context.StudentQuizAttempts.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
