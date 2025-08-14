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
    public class OrderItemService : IOrderItemService
    {
        private readonly OrderItemRepository orderItemRepository;

        public OrderItemService()
        {
            orderItemRepository = new OrderItemRepository();
        }

        public async Task<List<OrderItem>> GetAllAsync()
        {
            return await orderItemRepository.GetAllAsync();
        }

        public async Task<OrderItem> GetByIdAsync(int? id)
        {
            return await orderItemRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var orderItem = await GetByIdAsync(id);
            if (orderItem != null)
            {
                return await orderItemRepository.RemoveAsync(orderItem);
            }
            return false;
        }
    }
}
