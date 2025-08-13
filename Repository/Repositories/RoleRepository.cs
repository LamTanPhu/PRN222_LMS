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
    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository()
        {
        }

        public RoleRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Include(r => r.Users)
                .ToListAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.RoleId == id);
        }

        public async Task<int> CreateAsync(Role entity)
        {
            _context.Roles.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Role entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Roles.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Role entity)
        {
            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
