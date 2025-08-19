using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICouponService
    {
        Task<List<Coupon>> GetAllAsync();
        Task<Coupon> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task<Coupon> GetCouponByCodeAsync(string couponCode);
    }
}
