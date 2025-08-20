using Microsoft.EntityFrameworkCore;
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
    public class WishlistService : IWishlistService
    {
        private readonly WishlistRepository wishlistRepository;

        public WishlistService()
        {
            wishlistRepository = new WishlistRepository();
        }

        public async Task<IList<Course>> GetWishlistAsync(int userId)
        {
            return await wishlistRepository.GetByUserAsync(userId);
        }

        public async Task<List<Wishlist>> GetAllAsync()
        {
            return await wishlistRepository.GetAllAsync();
        }

        public async Task<Wishlist> GetByIdAsync(int? id)
        {
            return await wishlistRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var wishlist = await GetByIdAsync(id);
            if (wishlist != null)
            {
                return await wishlistRepository.RemoveAsync(wishlist);
            }
            return false;
        }

        public async Task AddToWishlistAsync(int userId, int courseId)
        {
            var exists = await wishlistRepository.ExistsAsync(userId, courseId);

            if (!exists)
            {
                var wishlist = new Wishlist
                {
                    UserId = userId,
                    CourseId = courseId,
                };
                await wishlistRepository.CreateAsync(wishlist);
            }
        }
    }
}
