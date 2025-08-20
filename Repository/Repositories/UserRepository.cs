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
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository()
        {
        }

        public UserRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Announcements)
                .Include(u => u.Certificates)
                .Include(u => u.Coupons)
                .Include(u => u.CourseReviews)
                .Include(u => u.Courses)
                .Include(u => u.Enrollments)
                .Include(u => u.ForumReplies)
                .Include(u => u.Forums)
                .Include(u => u.InstructorProfile)
                .Include(u => u.Orders)
                .Include(u => u.StudentProgresses)
                .Include(u => u.StudentQuizAttempts)
                .Include(u => u.Wishlists)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Announcements)
                .Include(u => u.Certificates)
                .Include(u => u.Coupons)
                .Include(u => u.CourseReviews)
                .Include(u => u.Courses)
                .Include(u => u.Enrollments)
                .Include(u => u.ForumReplies)
                .Include(u => u.Forums)
                .Include(u => u.InstructorProfile)
                .Include(u => u.Orders)
                .Include(u => u.StudentProgresses)
                .Include(u => u.StudentQuizAttempts)
                .Include(u => u.Wishlists)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<int> CreateAsync(User entity)
        {
            _context.Users.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(User entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Users.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(User entity)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
        }
        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role.RoleName == role)
                .ToListAsync() ?? new List<User>();
        }
    }
}
