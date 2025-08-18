using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAnnouncementService
    {
        Task<List<Announcement>> GetAllAsync();
        Task<Announcement> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(Announcement announcement);
        Task UpdateAsync(Announcement announcement);

    }
}
