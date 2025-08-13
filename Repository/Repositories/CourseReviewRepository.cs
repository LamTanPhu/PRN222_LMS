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
    public class CourseReviewRepository : GenericRepository<CourseReview>
    {
        public CourseReviewRepository()
        {
        }

        public CourseReviewRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<CourseReview>> GetAllAsync()
        {
            return await _context.CourseReviews
                .Include(cr => cr.Course)
                .Include(cr => cr.User)
                .ToListAsync();
        }

        public async Task<CourseReview> GetByIdAsync(int id)
        {
            return await _context.CourseReviews
                .Include(cr => cr.Course)
                .Include(cr => cr.User)
                .FirstOrDefaultAsync(cr => cr.ReviewId == id);
        }

        public async Task<int> CreateAsync(CourseReview entity)
        {
            _context.CourseReviews.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(CourseReview entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.CourseReviews.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(CourseReview entity)
        {
            _context.CourseReviews.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
