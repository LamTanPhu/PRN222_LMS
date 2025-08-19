using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.Models;
using Service.Interface;
using System;
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
            UserOptions = new SelectList(users, "UserId", "FullName"); // Assuming User has FullName
            NewInstructor = new InstructorProfile();
            EditInstructor = new InstructorProfile();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                await LoadData();
                return Page();
            }
            try
            {
                var user = await userService.GetByIdAsync(NewInstructor.UserId);
                if (user == null)
                {
                    ModelState.AddModelError("NewInstructor.UserId", "Invalid User ID.");
                    await LoadData();
                    return Page();
                }
                await instructorService.CreateAsync(NewInstructor);
                await LoadData(); // Refresh data
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create error: {ex}");
                ModelState.AddModelError(string.Empty, $"Error creating instructor: {ex.Message}");
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                await LoadData();
                return Page();
            }
            try
            {
                var instructor = await instructorService.GetByIdAsync(id);
                if (instructor == null)
                {
                    ModelState.AddModelError(string.Empty, "Instructor not found.");
                    await LoadData();
                    return Page();
                }
                var user = await userService.GetByIdAsync(EditInstructor.UserId);
                if (user == null)
                {
                    ModelState.AddModelError("EditInstructor.UserId", "Invalid User ID.");
                    await LoadData();
                    return Page();
                }
                instructor.UserId = EditInstructor.UserId;
                instructor.Biography = EditInstructor.Biography ?? instructor.Biography;
                await instructorService.UpdateAsync(instructor);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update error: {ex}");
                ModelState.AddModelError(string.Empty, $"Error updating instructor: {ex.Message}");
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
                Console.WriteLine($"Delete error: {ex}");
                ModelState.AddModelError(string.Empty, $"Error deleting instructor: {ex.Message}");
                await LoadData();
                return Page();
            }
        }
    }
}