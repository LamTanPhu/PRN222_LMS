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

        public async Task<int> UpdateAsync(StudentProgress entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.StudentProgresses.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(StudentProgress entity)
        {
            _context.StudentProgresses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
