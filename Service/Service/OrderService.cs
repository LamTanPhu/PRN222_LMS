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
    public class OrderService : IOrderService
    {
        private readonly OrderRepository orderRepository;

        public OrderService()
        {
            orderRepository = new OrderRepository();
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await orderRepository.GetAllAsync();
        }

        public async Task<Order> GetByIdAsync(int? id)
        {
            return await orderRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var order = await GetByIdAsync(id);
            if (order != null)
            {
                return await orderRepository.RemoveAsync(order);
            }
            return false;
        }
    }
}
