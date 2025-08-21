using Repository.Models;
using Repository.Repositories;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class QuizService : IQuizService
    {
        private readonly QuizRepository quizRepository;

        public QuizService()
        {
            quizRepository = new QuizRepository();
        }

        public async Task<List<Quiz>> GetAllAsync()
        {
            var quizzes = await quizRepository.GetAllAsync();
            
            // Debug: Log quiz information
            System.Diagnostics.Debug.WriteLine($"QuizService.GetAllAsync - Found {quizzes.Count} quizzes");
            foreach (var quiz in quizzes.Take(5)) // Log first 5 quizzes
            {
                System.Diagnostics.Debug.WriteLine($"  Quiz {quiz.QuizId}: {quiz.Title}, Questions: {quiz.QuizQuestions?.Count ?? 0}");
                if (quiz.QuizQuestions != null)
                {
                    foreach (var question in quiz.QuizQuestions.Take(3)) // Log first 3 questions
                    {
                        System.Diagnostics.Debug.WriteLine($"    Question {question.QuestionId}: {question.QuestionText}, Answers: {question.QuizAnswers?.Count ?? 0}");
                    }
                }
            }
            
            return quizzes;
        }

        public async Task<Quiz> GetByIdAsync(int? id)
        {
            return await quizRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<Quiz> GetByIdAsync(int id)
        {
            var quiz = await quizRepository.GetByIdAsync(id);
            
            // Debug: Log quiz information
            if (quiz != null)
            {
                System.Diagnostics.Debug.WriteLine($"QuizService.GetByIdAsync - Quiz ID: {quiz.QuizId}, Title: {quiz.Title}");
                System.Diagnostics.Debug.WriteLine($"Quiz.QuizQuestions count: {quiz.QuizQuestions?.Count ?? 0}");
                if (quiz.QuizQuestions != null)
                {
                    foreach (var question in quiz.QuizQuestions)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Question: {question.QuestionText}, Answers: {question.QuizAnswers?.Count ?? 0}");
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"QuizService.GetByIdAsync - Quiz with ID {id} not found");
            }
            
            return quiz;
        }

        public async Task<bool> CreateAsync(Quiz quiz)
        {
            var result = await quizRepository.CreateAsync(quiz);
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Quiz quiz)
        {
            var result = await quizRepository.UpdateAsync(quiz);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var quiz = await GetByIdAsync(id);
            if (quiz != null)
            {
                return await quizRepository.RemoveAsync(quiz);
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quiz = await GetByIdAsync(id);
            if (quiz != null)
            {
                return await quizRepository.RemoveAsync(quiz);
            }
            return false;
        }
        public async Task CreateAsync(Quiz quiz, int lessonId)
        {
            quiz.LessonId = lessonId;
            await quizRepository.CreateAsync(quiz);
        }

        public async Task<bool> AddQuestionToQuizAsync(int quizId, QuizQuestion question)
        {
            try
            {
                var context = new Repository.DBContext.CourseraStyleLMSContext();
                
                // Set the quiz ID for the question
                question.QuizId = quizId;
                
                // Add the question
                context.QuizQuestions.Add(question);
                
                // Add the answers
                if (question.QuizAnswers != null)
                {
                    foreach (var answer in question.QuizAnswers)
                    {
                        answer.QuestionId = question.QuestionId;
                        context.QuizAnswers.Add(answer);
                    }
                }
                
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
