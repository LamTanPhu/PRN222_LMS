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
    public class StudentProgressRepository : GenericRepository<StudentProgress>
    {
        public StudentProgressRepository()
        {
        }

        public StudentProgressRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<StudentProgress>> GetAllAsync()
        {
            return await _context.StudentProgresses
                .Include(sp => sp.Course)
                .Include(sp => sp.Lesson)
                .Include(sp => sp.User)
                .ToListAsync();
        }

        public async Task<StudentProgress> GetByIdAsync(int id)
        {
            return await _context.StudentProgresses
                .Include(sp => sp.Course)
                .Include(sp => sp.Lesson)
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.ProgressId == id);
        }

        public async Task<int> CreateAsync(StudentProgress entity)
        {
            _context.StudentProgresses.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int userId, int courseId, int lessonId, bool isCompleted)
        {
            var progress = await _context.StudentProgresses
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.CourseId == courseId && sp.LessonId == lessonId);

            if (progress == null)
            {
                progress = new StudentProgress
                {
                    UserId = userId,
                    CourseId = courseId,
                    LessonId = lessonId,
                    IsCompleted = isCompleted,
                    CompletedAt = isCompleted ? DateTime.UtcNow : null
                };
                _context.StudentProgresses.Add(progress);
            }
            else
            {
                progress.IsCompleted = isCompleted;
                progress.CompletedAt = isCompleted ? DateTime.UtcNow : null;
                _context.StudentProgresses.Update(progress);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(StudentProgress entity)
        {
            _context.StudentProgresses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<StudentProgress>> GetStudentProgressesByUserAsync(int userId)
        {
            return await _context.StudentProgresses
                .Include(sp => sp.Course)
                .Include(sp => sp.Lesson)
                .Where(sp => sp.UserId == userId)
                .ToListAsync();
        }
    }
}
