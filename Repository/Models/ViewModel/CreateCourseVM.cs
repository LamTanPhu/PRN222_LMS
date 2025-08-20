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

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public List<LessonViewModel> Lessons { get; set; } = new List<LessonViewModel>();
    }

    public class LessonViewModel
    {
        [Required(ErrorMessage = "Lesson title is required")]
        public string Title { get; set; }

        public string Content { get; set; }
        public List<QuizViewModel> Quizzes { get; set; } = new List<QuizViewModel>();
    }

    public class QuizViewModel
    {
        [Required(ErrorMessage = "Quiz title is required")]
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
