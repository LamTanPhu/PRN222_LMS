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
    public class ForumService : IForumService
    {
        private readonly ForumRepository forumRepository;

        public ForumService()
        {
            forumRepository = new ForumRepository();
        }

        public async Task<List<Forum>> GetAllAsync()
        {
            return await forumRepository.GetAllAsync();
        }

        public async Task<Forum> GetByIdAsync(int? id)
        {
            return await forumRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var forum = await GetByIdAsync(id);
            if (forum != null)
            {
                return await forumRepository.RemoveAsync(forum);
            }
            return false;
        }

        public async Task CreateAsync(Forum forum)
        {
            await forumRepository.CreateAsync(forum);
        }

        public async Task UpdateAsync(Forum forum)
        {
            await forumRepository.UpdateAsync(forum);
        }
    }
}
