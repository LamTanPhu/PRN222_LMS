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
    public class CouponService : ICouponService
    {
        private readonly CouponRepository couponRepository;

        public CouponService()
        {
            couponRepository = new CouponRepository();
        }

        public async Task<List<Coupon>> GetAllAsync()
        {
            return await couponRepository.GetAllAsync();
        }

        public async Task<Coupon> GetByIdAsync(int? id)
        {
            return await couponRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var coupon = await GetByIdAsync(id);
            if (coupon != null)
            {
                return await couponRepository.RemoveAsync(coupon);
            }
            return false;
        }

        public async Task<Coupon> GetCouponByCodeAsync(string couponCode)
        {
            var allCoupons = await GetAllAsync();
            return allCoupons.FirstOrDefault(c => c.CouponCode == couponCode);
        }
    }
}
