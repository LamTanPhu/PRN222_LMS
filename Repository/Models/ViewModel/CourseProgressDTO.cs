using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.ViewModel
{
    public class CourseProgressDto
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public decimal Progress { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public string Status { get; set; }
    }

}
