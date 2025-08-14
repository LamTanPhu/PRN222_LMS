using Repository.Models;
using Repository.Repositories;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ForumReplyService : IForumReplyService
    {
        private readonly ForumReplyRepository forumReplyRepository;

        public ForumReplyService(ForumReplyRepository forumReplyRepository)
        {
            this.forumReplyRepository = forumReplyRepository;
        }

        public async Task<List<ForumReply>> GetAllAsync()
        {
            return await forumReplyRepository.GetAllAsync();
        }

        public async Task<ForumReply> GetByIdAsync(int? id)
        {
            return await forumReplyRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var forumReply = await GetByIdAsync(id);
            if (forumReply != null)
            {
                return await forumReplyRepository.RemoveAsync(forumReply);
            }
            return false;
        }
    }
}
