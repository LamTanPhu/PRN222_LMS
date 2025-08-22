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
    public class LessonRepository : GenericRepository<Lesson>
    {
        public LessonRepository()
        {
        }

        public LessonRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Lesson>> GetAllAsync()
        {
            return await _context.Lessons
                .Include(l => l.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(l => l.Course)
                    .ThenInclude(c => c.Enrollments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Lesson> GetByIdAsync(int id)
        {
            return await _context.Lessons
                .Include(l => l.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(l => l.Course)
                    .ThenInclude(c => c.Enrollments)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LessonId == id);
        }

        public async Task<int> CreateAsync(Lesson entity)
        {
            _context.Lessons.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Lesson entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Lessons.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Lesson entity)
        {
            _context.Lessons.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Lesson>> GetLessonsForAdminAsync()
        {
            return await _context.Lessons
                .Select(l => new Lesson
                {
                    LessonId = l.LessonId,
                    CourseId = l.CourseId,
                    Title = l.Title,
                    Description = l.Description,
                    LessonType = l.LessonType,
                    Duration = l.Duration,
                    SortOrder = l.SortOrder,
                    IsPreview = l.IsPreview,
                    CreatedAt = l.CreatedAt,
                    Course = new Course
                    {
                        CourseId = l.Course.CourseId,
                        Title = l.Course.Title
                    }
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
