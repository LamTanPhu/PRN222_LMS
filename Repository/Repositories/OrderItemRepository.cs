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
    public class OrderItemRepository : GenericRepository<OrderItem>
    {
        public OrderItemRepository()
        {
        }

        public OrderItemRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<OrderItem>> GetAllAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Course)
                .ToListAsync();
        }

        public async Task<OrderItem> GetByIdAsync(int id)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Course)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id);
        }

        public async Task<int> CreateAsync(OrderItem entity)
        {
            _context.OrderItems.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(OrderItem entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.OrderItems.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(OrderItem entity)
        {
            _context.OrderItems.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
