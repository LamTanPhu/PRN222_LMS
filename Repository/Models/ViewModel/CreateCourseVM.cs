using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.ViewModel
{
    public class CourseCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Subtitle { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Instructor is required")]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        public string DifficultyLevel { get; set; }

        public string Language { get; set; }
    }
}
