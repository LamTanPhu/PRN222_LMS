using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IPaymentService
    {
        Task<List<Payment>> GetAllAsync();
        Task<Payment> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
    }
}
