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
    public class QuizQuestionRepository : GenericRepository<QuizQuestion>
    {
        public QuizQuestionRepository()
        {
        }

        public QuizQuestionRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<QuizQuestion>> GetAllAsync()
        {
            return await _context.QuizQuestions
                .Include(qq => qq.Quiz)
                .Include(qq => qq.QuizAnswers)
                .ToListAsync();
        }

        public async Task<QuizQuestion> GetByIdAsync(int id)
        {
            return await _context.QuizQuestions
                .Include(qq => qq.Quiz)
                .Include(qq => qq.QuizAnswers)
                .FirstOrDefaultAsync(qq => qq.QuestionId == id);
        }

        public async Task<int> CreateAsync(QuizQuestion entity)
        {
            _context.QuizQuestions.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(QuizQuestion entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.QuizQuestions.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(QuizQuestion entity)
        {
            _context.QuizQuestions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<QuizQuestion>> GetByQuizIdAsync(int quizId)
        {
            return await _context.QuizQuestions
                .Include(qq => qq.QuizAnswers)
                .Where(qq => qq.QuizId == quizId)
                .OrderBy(qq => qq.SortOrder)
                .ToListAsync();
        }
    }
}
