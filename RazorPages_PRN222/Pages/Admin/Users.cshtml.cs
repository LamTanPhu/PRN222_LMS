using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly IUserService userService;
        private readonly IRoleService roleService;

        public List<User> Users { get; set; } = new List<User>();
        public SelectList RoleOptions { get; set; }
        [BindProperty]
        public User NewUser { get; set; }
        [BindProperty]
        public User EditUser { get; set; } // For editing

        public UsersModel(IUserService userService, IRoleService roleService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            Users = await userService.GetAllAsync() ?? new List<User>(); // Ensure no limit
            var roles = await roleService.GetAllAsync() ?? new List<Role>();
            RoleOptions = new SelectList(roles, "RoleId", "RoleName");
            NewUser = new User();
            EditUser = new User(); // Initialize for potential pre-population
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                await userService.CreateAsync(NewUser);
                await LoadData(); // Refresh with all records
                return Page(); // Stay on page to show updated list
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error creating user: " + ex.Message);
                await LoadData(); // Refresh even on error
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                var user = await userService.GetByIdAsync(id);
                if (user != null)
                {
                    // Use EditUser values directly for binding
                    user.Username = EditUser.Username ?? user.Username;
                    user.Email = EditUser.Email ?? user.Email;
                    user.PasswordHash = EditUser.PasswordHash ?? user.PasswordHash;
                    user.FullName = EditUser.FullName ?? user.FullName;
                    user.RoleId = EditUser.RoleId != 0 ? EditUser.RoleId : user.RoleId; // Avoid unset role
                    await userService.UpdateAsync(user);
                }
                await LoadData(); // Refresh with all records
                return Page(); // Stay on page to show updated list
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error updating user: " + ex.Message);
                await LoadData(); // Refresh even on error
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await userService.DeleteAsync(id);
                await LoadData(); // Refresh with all records
                return Page(); // Stay on page to show updated list
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error deleting user: " + ex.Message);
                await LoadData(); // Refresh even on error
                return Page();
            }
        }
    }
}