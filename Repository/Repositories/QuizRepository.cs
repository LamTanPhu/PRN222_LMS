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
    public class QuizRepository : GenericRepository<Quiz>
    {
        public QuizRepository()
        {
        }

        public QuizRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Quiz>> GetAllAsync()
        {
            return await _context.Quizzes
                .Include(q => q.Lesson)
                    .ThenInclude(l => l.Course)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Quiz> GetByIdAsync(int id)
        {
            return await _context.Quizzes
                .Include(q => q.Lesson)
                    .ThenInclude(l => l.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.QuizId == id);
        }

        public async Task<int> CreateAsync(Quiz entity)
        {
            _context.Quizzes.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Quiz entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Quizzes.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Quiz entity)
        {
            _context.Quizzes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
