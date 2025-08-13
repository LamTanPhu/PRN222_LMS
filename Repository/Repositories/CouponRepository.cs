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
    public class CouponRepository : GenericRepository<Coupon>
    {
        public CouponRepository()
        {
        }

        public CouponRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _context.Coupons
                .Include(c => c.CreatedByNavigation)
                .ToListAsync();
        }

        public async Task<Coupon> GetByIdAsync(int id)
        {
            return await _context.Coupons
                .Include(c => c.CreatedByNavigation)
                .FirstOrDefaultAsync(c => c.CouponId == id);
        }

        public async Task<int> CreateAsync(Coupon entity)
        {
            _context.Coupons.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Coupon entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Coupons.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Coupon entity)
        {
            _context.Coupons.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
