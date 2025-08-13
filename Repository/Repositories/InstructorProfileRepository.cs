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
    public class InstructorProfileRepository : GenericRepository<InstructorProfile>
    {
        public InstructorProfileRepository()
        {
        }

        public InstructorProfileRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<InstructorProfile>> GetAllAsync()
        {
            return await _context.InstructorProfiles
                .Include(ip => ip.User)
                .ToListAsync();
        }

        public async Task<InstructorProfile> GetByIdAsync(int id)
        {
            return await _context.InstructorProfiles
                .Include(ip => ip.User)
                .FirstOrDefaultAsync(ip => ip.InstructorId == id);
        }

        public async Task<int> CreateAsync(InstructorProfile entity)
        {
            _context.InstructorProfiles.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(InstructorProfile entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.InstructorProfiles.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(InstructorProfile entity)
        {
            _context.InstructorProfiles.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
