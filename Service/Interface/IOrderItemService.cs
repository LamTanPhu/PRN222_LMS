using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IOrderItemService
    {
        Task<List<OrderItem>> GetAllAsync();
        Task<OrderItem> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(OrderItem orderItem);
        Task UpdateAsync(OrderItem orderItem);
    }
}
