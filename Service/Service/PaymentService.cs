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
    public class PaymentService : IPaymentService
    {
        private readonly PaymentRepository paymentRepository;

        public PaymentService()
        {
            paymentRepository = new PaymentRepository();
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            return await paymentRepository.GetAllAsync();
        }

        public async Task<Payment> GetByIdAsync(int? id)
        {
            return await paymentRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var payment = await GetByIdAsync(id);
            if (payment != null)
            {
                return await paymentRepository.RemoveAsync(payment);
            }
            return false;
        }

        public async Task CreateAsync(Payment payment)
        {
            await paymentRepository.CreateAsync(payment);
        }

        public async Task UpdateAsync(Payment payment)
        {
            await paymentRepository.UpdateAsync(payment);
        }
    }
}
