using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IWishlistService
    {
        Task AddToWishlistAsync(int userId, int courseId);
        Task<List<Wishlist>> GetAllAsync();
        Task<Wishlist> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task<IList<Course>> GetWishlistAsync(int userId);
    }
}
