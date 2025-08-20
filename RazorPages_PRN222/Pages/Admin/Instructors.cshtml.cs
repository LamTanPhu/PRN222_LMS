using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.Models;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // Remove validation for fields that should be auto-generated or have defaults
            ModelState.Remove("NewInstructor.InstructorId"); // Auto-generated
            ModelState.Remove("NewInstructor.TotalStudents"); // Has default
            ModelState.Remove("NewInstructor.AverageRating"); // Has default  
            ModelState.Remove("NewInstructor.TotalCourses"); // Has default
            ModelState.Remove("NewInstructor.User"); // Navigation property

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                await LoadData();
                return Page();
            }

            try
            {
                // Validate user exists
                var user = await userService.GetByIdAsync(NewInstructor.UserId);
                if (user == null)
                {
                    ModelState.AddModelError("NewInstructor.UserId", "Invalid User ID.");
                    await LoadData();
                    return Page();
                }

                // Check if instructor profile already exists for this user
                var existingInstructors = await instructorService.GetAllAsync();
                if (existingInstructors.Any(i => i.UserId == NewInstructor.UserId))
                {
                    ModelState.AddModelError("NewInstructor.UserId", "An instructor profile already exists for this user.");
                    await LoadData();
                    return Page();
                }

                // Set default values explicitly (in case they're not being set properly)
                NewInstructor.TotalStudents = NewInstructor.TotalStudents ?? 0;
                NewInstructor.AverageRating = NewInstructor.AverageRating ?? 0.00m;
                NewInstructor.TotalCourses = NewInstructor.TotalCourses ?? 0;

                // Don't set InstructorId - let it be auto-generated
                NewInstructor.InstructorId = 0;

                await instructorService.CreateAsync(NewInstructor);

                // Redirect to avoid form resubmission
                return RedirectToPage();
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
            // Remove validation for auto-generated and navigation properties
            ModelState.Remove("EditInstructor.InstructorId");
            ModelState.Remove("EditInstructor.User");

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

                // Check if another instructor already has this UserId (excluding current instructor)
                var existingInstructors = await instructorService.GetAllAsync();
                if (existingInstructors.Any(i => i.UserId == EditInstructor.UserId && i.InstructorId != id))
                {
                    ModelState.AddModelError("EditInstructor.UserId", "An instructor profile already exists for this user.");
                    await LoadData();
                    return Page();
                }

                // Update only the fields that should be updated
                instructor.UserId = EditInstructor.UserId;
                instructor.Biography = EditInstructor.Biography ?? instructor.Biography;
                instructor.Headline = EditInstructor.Headline ?? instructor.Headline;
                instructor.Website = EditInstructor.Website ?? instructor.Website;
                instructor.LinkedInProfile = EditInstructor.LinkedInProfile ?? instructor.LinkedInProfile;
                instructor.TwitterHandle = EditInstructor.TwitterHandle ?? instructor.TwitterHandle;
                instructor.YearsOfExperience = EditInstructor.YearsOfExperience ?? instructor.YearsOfExperience;

                await instructorService.UpdateAsync(instructor);

                // Redirect to avoid form resubmission
                return RedirectToPage();
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

                // Redirect to avoid form resubmission
                return RedirectToPage();
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