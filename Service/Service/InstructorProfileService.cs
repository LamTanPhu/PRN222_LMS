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
    public class InstructorProfileService : IInstructorProfileService
    {
        private readonly InstructorProfileRepository instructorProfileRepository;

        public InstructorProfileService(InstructorProfileRepository instructorProfileRepository)
        {
            this.instructorProfileRepository = instructorProfileRepository;
        }

        public async Task<List<InstructorProfile>> GetAllAsync()
        {
            return await instructorProfileRepository.GetAllAsync();
        }

        public async Task<InstructorProfile> GetByIdAsync(int? id)
        {
            return await instructorProfileRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var instructorProfile = await GetByIdAsync(id);
            if (instructorProfile != null)
            {
                return await instructorProfileRepository.RemoveAsync(instructorProfile);
            }
            return false;
        }
    }
}
