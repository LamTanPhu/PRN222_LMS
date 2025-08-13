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
    public class QuizAnswerRepository : GenericRepository<QuizAnswer>
    {
        public QuizAnswerRepository()
        {
        }

        public QuizAnswerRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<QuizAnswer>> GetAllAsync()
        {
            return await _context.QuizAnswers
                .Include(qa => qa.Question)
                .ToListAsync();
        }

        public async Task<QuizAnswer> GetByIdAsync(int id)
        {
            return await _context.QuizAnswers
                .Include(qa => qa.Question)
                .FirstOrDefaultAsync(qa => qa.AnswerId == id);
        }

        public async Task<int> CreateAsync(QuizAnswer entity)
        {
            _context.QuizAnswers.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(QuizAnswer entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.QuizAnswers.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(QuizAnswer entity)
        {
            _context.QuizAnswers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
