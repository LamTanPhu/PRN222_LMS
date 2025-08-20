using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Service.Service;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace RazorPages_PRN222.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IUserService userService;
        private readonly ICourseService courseService;
        private readonly IAnnouncementService announcementService;
        private readonly IOrderService orderService;
        private readonly IPaymentService paymentService;
        private readonly IEnrollmentService enrollmentService;
        private readonly ICourseReviewService courseReviewService;

        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalAnnouncements { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PaymentsToday { get; set; }
        public int EnrollmentsToday { get; set; }
        public decimal AverageRating { get; set; }
        public int NewUsersLast7Days { get; set; }

        public string LabelsJson { get; set; }
        public string RevenueSeriesJson { get; set; }
        public string PaymentsSeriesJson { get; set; }
        public string EnrollmentsSeriesJson { get; set; }
        public string NewUsersSeriesJson { get; set; }

        public IndexModel(
            IUserService userService,
            ICourseService courseService,
            IAnnouncementService announcementService,
            IOrderService orderService,
            IPaymentService paymentService,
            IEnrollmentService enrollmentService,
            ICourseReviewService courseReviewService)
        {
            this.userService = userService;
            this.courseService = courseService;
            this.announcementService = announcementService;
            this.orderService = orderService;
            this.paymentService = paymentService;
            this.enrollmentService = enrollmentService;
            this.courseReviewService = courseReviewService;
        }

        public async Task OnGetAsync()
        {
            var users = await userService.GetAllAsync();
            var courses = await courseService.GetAllAsync();
            var announcements = await announcementService.GetAllAsync();
            var orders = await orderService.GetAllAsync();
            var payments = await paymentService.GetAllAsync();
            var enrollments = await enrollmentService.GetAllAsync();
            var reviews = await courseReviewService.GetAllAsync();

            TotalUsers = users.Count;
            TotalCourses = courses.Count;
            TotalAnnouncements = announcements.Count;
            TotalOrders = orders.Count;
            TotalRevenue = orders.Sum(o => (decimal?)o.FinalAmount ?? 0m);

            var today = DateTime.Today;
            PaymentsToday = payments.Count(p => p.PaymentStatus == "Completed" && p.PaymentDate.HasValue && p.PaymentDate.Value.Date == today);
            EnrollmentsToday = enrollments.Count(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value.Date == today);
            AverageRating = reviews.Count > 0 ? (decimal)reviews.Average(r => r.Rating) : 0m;
            var sevenDaysAgo = today.AddDays(-7);
            NewUsersLast7Days = users.Count(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date >= sevenDaysAgo);

            // Build 7-day time series for charts
            var last7Days = Enumerable.Range(0, 7).Select(i => today.AddDays(-6 + i).Date).ToList();
            var labels = last7Days.Select(d => d.ToString("MM-dd")).ToList();

            var revenueSeries = new List<decimal>();
            var paymentsSeries = new List<int>();
            var enrollmentsSeries = new List<int>();
            var newUsersSeries = new List<int>();

            foreach (var d in last7Days)
            {
                var paymentsOnDay = payments.Where(p => p.PaymentStatus == "Completed" && p.PaymentDate.HasValue && p.PaymentDate.Value.Date == d);
                revenueSeries.Add(paymentsOnDay.Sum(p => p.Amount));
                paymentsSeries.Add(paymentsOnDay.Count());

                enrollmentsSeries.Add(enrollments.Count(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value.Date == d));
                newUsersSeries.Add(users.Count(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date == d));
            }

            LabelsJson = JsonSerializer.Serialize(labels);
            RevenueSeriesJson = JsonSerializer.Serialize(revenueSeries);
            PaymentsSeriesJson = JsonSerializer.Serialize(paymentsSeries);
            EnrollmentsSeriesJson = JsonSerializer.Serialize(enrollmentsSeries);
            NewUsersSeriesJson = JsonSerializer.Serialize(newUsersSeries);
        }
    }
}