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
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository categoryRepository;

        public CategoryService(CategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await categoryRepository.GetAllAsync();
        }

        public async Task<Category> GetByIdAsync(int? id)
        {
            return await categoryRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                return await categoryRepository.RemoveAsync(category);
            }
            return false;
        }
    }
}
