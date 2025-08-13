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
    public class ForumRepository : GenericRepository<Forum>
    {
        public ForumRepository()
        {
        }

        public ForumRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Forum>> GetAllAsync()
        {
            return await _context.Forums
                .Include(f => f.Course)
                .Include(f => f.User)
                .Include(f => f.ForumReplies)
                .ToListAsync();
        }

        public async Task<Forum> GetByIdAsync(int id)
        {
            return await _context.Forums
                .Include(f => f.Course)
                .Include(f => f.User)
                .Include(f => f.ForumReplies)
                .FirstOrDefaultAsync(f => f.PostId == id);
        }

        public async Task<int> CreateAsync(Forum entity)
        {
            _context.Forums.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Forum entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Forums.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Forum entity)
        {
            _context.Forums.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
