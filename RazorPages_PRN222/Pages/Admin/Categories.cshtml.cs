using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using Service.Service;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class CategoriesModel : PageModel
    {
        private readonly ICategoryService categoryService;

        public List<Category> Categories { get; set; }

        public CategoriesModel(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task OnGetAsync()
        {
            Categories = await categoryService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await categoryService.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}