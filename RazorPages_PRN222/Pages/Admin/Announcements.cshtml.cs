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
    public class AnnouncementModel : PageModel
    {
        private readonly IAnnouncementService announcementService;
        private readonly IUserService userService;

        public List<Announcement> Announcements { get; set; } = new List<Announcement>();
        public SelectList UserOptions { get; set; }
        public SelectList AnnouncementTypeOptions { get; set; }
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
            UserOptions = new SelectList(users, "UserId", "FullName");

            // FIXED: Updated to match database constraint values exactly
            AnnouncementTypeOptions = new SelectList(new[]
            {
                new { Value = "News", Text = "News" },
                new { Value = "Event", Text = "Event" },
                new { Value = "Update", Text = "Update" },
                new { Value = "Promotion", Text = "Promotion" }
            }, "Value", "Text");

            NewAnnouncement = new Announcement();
            EditAnnouncement = new Announcement();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            // ADDED: Validate AnnouncementType before ModelState check
            if (string.IsNullOrEmpty(NewAnnouncement?.AnnouncementType))
            {
                ModelState.AddModelError("NewAnnouncement.AnnouncementType", "Please select an announcement type.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
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
                Console.WriteLine($"Create error: {ex}");
                ModelState.AddModelError(string.Empty, $"Error creating announcement: {ex.Message}");
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            // ADDED: Validate AnnouncementType before ModelState check
            if (string.IsNullOrEmpty(EditAnnouncement?.AnnouncementType))
            {
                ModelState.AddModelError("EditAnnouncement.AnnouncementType", "Please select an announcement type.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                await LoadData();
                return Page();
            }
            try
            {
                var announcement = await announcementService.GetByIdAsync(id);
                if (announcement == null)
                {
                    ModelState.AddModelError(string.Empty, "Announcement not found.");
                    await LoadData();
                    return Page();
                }
                var user = await userService.GetByIdAsync(EditAnnouncement.AuthorId);
                if (user == null)
                {
                    ModelState.AddModelError("EditAnnouncement.AuthorId", "Invalid Author ID.");
                    await LoadData();
                    return Page();
                }
                announcement.Title = EditAnnouncement.Title ?? announcement.Title;
                announcement.Content = EditAnnouncement.Content ?? announcement.Content;
                announcement.AnnouncementType = EditAnnouncement.AnnouncementType ?? announcement.AnnouncementType;
                announcement.AuthorId = EditAnnouncement.AuthorId;
                announcement.PublishDate = EditAnnouncement.PublishDate ?? announcement.PublishDate;
                await announcementService.UpdateAsync(announcement);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update error: {ex}");
                ModelState.AddModelError(string.Empty, $"Error updating announcement: {ex.Message}");
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
                Console.WriteLine($"Delete error: {ex}");
                ModelState.AddModelError(string.Empty, $"Error deleting announcement: {ex.Message}");
                await LoadData();
                return Page();
            }
        }
    }
}