using Microsoft.EntityFrameworkCore;
using Repository.Basic;
using Repository.DBContext;
using Repository.Models;
using Repository.Models.ViewModel;
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
        public async Task<CourseProgressDto> GetCourseProgressAsync(int userId, int courseId)
        {
            // Enrollment record
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
                return null;

            int totalLessons = await _context.Lessons
                .CountAsync(l => l.CourseId == courseId);

            int completedLessons = await _context.Lessons
                .CountAsync(l => l.CourseId == courseId && l.IsCompleted == true);

            
            decimal progressPercentage = totalLessons > 0
                ? Math.Round((decimal)completedLessons / totalLessons * 100, 2)
                : 0;

            // Update enrollment
            enrollment.ProgressPercentage = progressPercentage;
            enrollment.LastAccessedAt = DateTime.UtcNow;
            enrollment.Status = progressPercentage == 100 ? "Completed" : "In Progress";
            if (progressPercentage == 100)
                enrollment.CompletionDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CourseProgressDto
            {
                CourseId = courseId,
                UserId = userId,
                Progress = progressPercentage,
                CompletedLessons = completedLessons,
                TotalLessons = totalLessons,
                Status = enrollment.Status
            };
        }


        public async Task MarkLessonCompletedAsync(int userId, int courseId, int lessonId)
        {
            var progress = await _context.StudentProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId && p.LessonId == lessonId);

            if (progress == null)
            {
                // create record if missing
                progress = new StudentProgress
                {
                    UserId = userId,
                    CourseId = courseId,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow,
                    LastAccessedAt = DateTime.UtcNow
                };
                _context.StudentProgresses.Add(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                progress.LastAccessedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

    }
}

