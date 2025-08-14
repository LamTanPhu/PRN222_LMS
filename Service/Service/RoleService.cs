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
    public class RoleService : IRoleService
    {
        private readonly RoleRepository roleRepository;
        public RoleService()
        {
            roleRepository = new RoleRepository();
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await roleRepository.GetAllAsync();
        }

        public async Task<Role> GetByIdAsync(int? id)
        {
            return await roleRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var role = await GetByIdAsync(id);
            if (role != null)
            {
                return await roleRepository.RemoveAsync(role);
            }
            return false;
        }
    }
}
