using Repository.Basic;
using Repository.Models;
using Repository.Repositories;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly AnnouncementRepository announcementRepository;

        public AnnouncementService()
        {
            announcementRepository = new AnnouncementRepository();
        }

        public async Task<List<Announcement>> GetAllAsync()
        {
            return await announcementRepository.GetAllAsync();
        }

        public async Task<Announcement> GetByIdAsync(int? id)
        {
            return await announcementRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var announcement = await GetByIdAsync(id);
            if (announcement != null)
            {
                return await announcementRepository.RemoveAsync(announcement);
            }
            return false;
        }

        public async Task CreateAsync(Announcement announcement)
        {
            await announcementRepository.CreateAsync(announcement);
        }

        public async Task UpdateAsync(Announcement announcement)
        {
            await announcementRepository.UpdateAsync(announcement);
        }
    }
}