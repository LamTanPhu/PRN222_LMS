using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class AnnouncementModel : PageModel
    {
        private readonly IAnnouncementService announcementService;
        private readonly IUserService userService;

        public List<Announcement> Announcements { get; set; } = new List<Announcement>();
        public SelectList UserOptions { get; set; }
        [BindProperty]
        public Announcement NewAnnouncement { get; set; }
        [BindProperty]
        public Announcement EditAnnouncement { get; set; }

        public AnnouncementModel(IAnnouncementService announcementService, IUserService userService)
        {
            this.announcementService = announcementService ?? throw new ArgumentNullException(nameof(announcementService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            Announcements = await announcementService.GetAllAsync() ?? new List<Announcement>();
            var users = await userService.GetAllAsync() ?? new List<User>();
            UserOptions = new SelectList(users, "UserId", "UserId"); // Adjust to display name if available
            NewAnnouncement = new Announcement();
            EditAnnouncement = new Announcement();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }
            try
            {
                var user = await userService.GetByIdAsync(NewAnnouncement.AuthorId);
                if (user == null)
                {
                    ModelState.AddModelError("NewAnnouncement.AuthorId", "Invalid Author ID.");
                    await LoadData();
                    return Page();
                }
                await announcementService.CreateAsync(NewAnnouncement);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error creating announcement: " + ex.Message);
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                var announcement = await announcementService.GetByIdAsync(id);
                if (announcement != null)
                {
                    announcement.Title = EditAnnouncement.Title ?? announcement.Title;
                    announcement.Content = EditAnnouncement.Content ?? announcement.Content;
                    await announcementService.UpdateAsync(announcement);
                }
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error updating announcement: " + ex.Message);
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await announcementService.DeleteAsync(id);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error deleting announcement: " + ex.Message);
                await LoadData();
                return Page();
            }
        }
    }
}