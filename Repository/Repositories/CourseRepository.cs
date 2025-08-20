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
    public class CourseRepository : GenericRepository<Course>
    {
        public CourseRepository()
        {
        }

        public CourseRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<int> CreateAsync(Course entity)
        {
            _context.Courses.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Course entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Courses.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Course entity)
        {
            _context.Courses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Course>> GetCoursesForAdminAsync()
        {
            return await _context.Courses
                .Select(c => new Course
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Category = new Category
                    {
                        CategoryId = c.Category.CategoryId,
                        Name = c.Category.Name
                    },
                    Instructor = new User
                    {
                        UserId = c.Instructor.UserId,
                        Username = c.Instructor.Username
                    }
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
