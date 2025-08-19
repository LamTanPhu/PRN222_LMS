using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class InstructorsModel : PageModel
    {
        private readonly IInstructorProfileService instructorService;
        private readonly IUserService userService;

        public List<InstructorProfile> Instructors { get; set; } = new List<InstructorProfile>();
        public SelectList UserOptions { get; set; }
        [BindProperty]
        public InstructorProfile NewInstructor { get; set; }
        [BindProperty]
        public InstructorProfile EditInstructor { get; set; }

        public InstructorsModel(IInstructorProfileService instructorService, IUserService userService)
        {
            this.instructorService = instructorService ?? throw new ArgumentNullException(nameof(instructorService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            Instructors = await instructorService.GetAllAsync() ?? new List<InstructorProfile>();
            var users = await userService.GetAllAsync() ?? new List<User>();
            UserOptions = new SelectList(users, "UserId", "UserId"); // Adjust to display name if available
            NewInstructor = new InstructorProfile();
            EditInstructor = new InstructorProfile();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                await instructorService.CreateAsync(NewInstructor);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error creating instructor: " + ex.Message);
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                var instructor = await instructorService.GetByIdAsync(id);
                if (instructor != null)
                {
                    instructor.InstructorId = EditInstructor.InstructorId != 0 ? EditInstructor.InstructorId : instructor.InstructorId;
                    instructor.UserId = EditInstructor.UserId != 0 ? EditInstructor.UserId : instructor.UserId;
                    instructor.Biography = EditInstructor.Biography ?? instructor.Biography;
                    await instructorService.UpdateAsync(instructor);
                }
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error updating instructor: " + ex.Message);
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await instructorService.DeleteAsync(id);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error deleting instructor: " + ex.Message);
                await LoadData();
                return Page();
            }
        }
    }
}