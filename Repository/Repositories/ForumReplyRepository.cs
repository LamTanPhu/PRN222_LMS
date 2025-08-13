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
    public class ForumReplyRepository : GenericRepository<ForumReply>
    {
        public ForumReplyRepository()
        {
        }

        public ForumReplyRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<ForumReply>> GetAllAsync()
        {
            return await _context.ForumReplies
                .Include(fr => fr.Post)
                .Include(fr => fr.User)
                .Include(fr => fr.ParentReply)
                .Include(fr => fr.InverseParentReply)
                .ToListAsync();
        }

        public async Task<ForumReply> GetByIdAsync(int id)
        {
            return await _context.ForumReplies
                .Include(fr => fr.Post)
                .Include(fr => fr.User)
                .Include(fr => fr.ParentReply)
                .Include(fr => fr.InverseParentReply)
                .FirstOrDefaultAsync(fr => fr.ReplyId == id);
        }

        public async Task<int> CreateAsync(ForumReply entity)
        {
            _context.ForumReplies.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(ForumReply entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.ForumReplies.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(ForumReply entity)
        {
            _context.ForumReplies.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
