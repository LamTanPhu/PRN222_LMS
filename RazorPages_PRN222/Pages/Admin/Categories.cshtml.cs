using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class CategoriesModel : PageModel
    {
        private readonly ICategoryService categoryService;

        public List<Category> Categories { get; set; } = new List<Category>();
        [BindProperty]
        public Category NewCategory { get; set; }
        [BindProperty]
        public Category EditCategory { get; set; }

        public CategoriesModel(ICategoryService categoryService)
        {
            this.categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            Categories = await categoryService.GetAllAsync() ?? new List<Category>();
            NewCategory = new Category();
            EditCategory = new Category();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                await categoryService.CreateAsync(NewCategory);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error creating category: " + ex.Message);
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                var category = await categoryService.GetByIdAsync(id);
                if (category != null)
                {
                    category.Name = EditCategory.Name ?? category.Name;
                    await categoryService.UpdateAsync(category);
                }
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error updating category: " + ex.Message);
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await categoryService.DeleteAsync(id);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error deleting category: " + ex.Message);
                await LoadData();
                return Page();
            }
        }
    }
}