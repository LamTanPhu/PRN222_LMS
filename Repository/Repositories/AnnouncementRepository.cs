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
    public class AnnouncementRepository : GenericRepository<Announcement>
    {
        public AnnouncementRepository()
        {
        }

        public AnnouncementRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Announcement>> GetAllAsync()
        {
            return await _context.Announcements
                .Include(a => a.Author)
                .ToListAsync();
        }

        public async Task<Announcement> GetByIdAsync(int id)
        {
            return await _context.Announcements
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.AnnouncementId == id);
        }

        public async Task<int> CreateAsync(Announcement entity)
        {
            _context.Announcements.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Announcement entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Announcements.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Announcement entity)
        {
            _context.Announcements.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
