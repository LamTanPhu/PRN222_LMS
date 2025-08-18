using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IInstructorProfileService
    {
        Task<List<InstructorProfile>> GetAllAsync();
        Task<InstructorProfile> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(InstructorProfile profile);
        Task UpdateAsync(InstructorProfile profile);
    }
}
