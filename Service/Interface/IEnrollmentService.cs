using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IEnrollmentService
    {
        Task<List<Enrollment>> GetAllAsync();
        Task<Enrollment> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
    }
}
