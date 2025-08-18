using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using Service.Service;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class InstructorsModel : PageModel
    {
        private readonly IInstructorProfileService instructorProfileService;

        public List<InstructorProfile> Instructors { get; set; }

        public InstructorsModel(IInstructorProfileService instructorProfileService)
        {
            this.instructorProfileService = instructorProfileService;
        }

        public async Task OnGetAsync()
        {
            Instructors = await instructorProfileService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await instructorProfileService.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}