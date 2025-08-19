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
    public class WishlistRepository : GenericRepository<Wishlist>
    {
        public WishlistRepository()
        {
        }

        public WishlistRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<IList<Course>> GetByUserAsync(int userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => w.Course)
                .ToListAsync();
        }
        public async Task<List<Wishlist>> GetAllAsync()
        {
            return await _context.Wishlists
                .Include(w => w.Course)
                .Include(w => w.User)
                .ToListAsync();
        }

        public async Task<Wishlist> GetByIdAsync(int id)
        {
            return await _context.Wishlists
                .Include(w => w.Course)
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.WishlistId == id);
        }

        public async Task<int> CreateAsync(Wishlist entity)
        {
            _context.Wishlists.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Wishlist entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Wishlists.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Wishlist entity)
        {
            _context.Wishlists.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int userId, int courseId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.CourseId == courseId);
        }
    }
}

//