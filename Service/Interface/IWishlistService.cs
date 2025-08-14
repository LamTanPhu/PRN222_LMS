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
        Task<List<Wishlist>> GetAllAsync();
        Task<Wishlist> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
    }
}
