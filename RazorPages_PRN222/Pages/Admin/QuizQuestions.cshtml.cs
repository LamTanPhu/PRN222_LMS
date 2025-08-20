using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class QuizQuestionsModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly IQuizQuestionService _quizQuestionService;
        private readonly IQuizAnswerService _quizAnswerService;

        public QuizQuestionsModel(
            IQuizService quizService, 
            IQuizQuestionService quizQuestionService,
            IQuizAnswerService quizAnswerService)
        {
            _quizService = quizService;
            _quizQuestionService = quizQuestionService;
            _quizAnswerService = quizAnswerService;
        }

        public Repository.Models.Quiz Quiz { get; set; }
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();

        public async Task OnGetAsync(int quizId)
        {
            // Get quiz details
            Quiz = await _quizService.GetByIdAsync(quizId);
            
            if (Quiz != null)
            {
                // Get all questions for this quiz
                var allQuestions = await _quizQuestionService.GetAllAsync();
                Questions = allQuestions.Where(q => q.QuizId == quizId).ToList();
            }
        }

        public async Task<IActionResult> OnPostAddQuestionAsync(int quizId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get form data
            var questionText = Request.Form["QuestionText"];
            var questionType = Request.Form["QuestionType"];
            var points = int.Parse(Request.Form["Points"]);

            // Create new question
            var question = new QuizQuestion
            {
                QuizId = quizId,
                QuestionText = questionText,
                QuestionType = questionType,
                Points = points,
                SortOrder = 1 // Default sort order
            };

            // Save question first to get the QuestionId
            var questionCreated = await _quizQuestionService.CreateAsync(question);
            
            if (questionCreated)
            {
                // Get the saved question to get the QuestionId
                var savedQuestion = await _quizQuestionService.GetByIdAsync(question.QuestionId);
                
                // Handle answers for Multiple Choice and True/False questions
                if (questionType == "Multiple Choice" || questionType == "True/False")
                {
                    var answerTexts = Request.Form["AnswerText[]"].ToArray();
                    var correctAnswerIndex = int.Parse(Request.Form["CorrectAnswer"]);

                    for (int i = 0; i < answerTexts.Length; i++)
                    {
                        var answer = new QuizAnswer
                        {
                            QuestionId = savedQuestion.QuestionId,
                            AnswerText = answerTexts[i],
                            IsCorrect = (i == correctAnswerIndex),
                            SortOrder = i + 1
                        };

                        await _quizAnswerService.CreateAsync(answer);
                    }
                }
            }

            return RedirectToPage(new { quizId = quizId });
        }

        public async Task<IActionResult> OnPostDeleteQuestionAsync(int quizId, int questionId)
        {
            await _quizQuestionService.DeleteAsync(questionId);
            return RedirectToPage(new { quizId = quizId });
        }
    }
}
