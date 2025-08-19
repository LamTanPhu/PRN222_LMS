using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IForumReplyService
    {
        Task<List<ForumReply>> GetAllAsync();
        Task<ForumReply> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(ForumReply reply);
        Task UpdateAsync(ForumReply reply);
    }
}
