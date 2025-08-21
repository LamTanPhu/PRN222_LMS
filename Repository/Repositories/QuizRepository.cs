using Microsoft.EntityFrameworkCore;
using Repository.Basic;
using Repository.DBContext;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class QuizRepository : GenericRepository<Quiz>
    {
        public QuizRepository()
        {
        }

        public QuizRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Quiz>> GetAllAsync()
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Lesson)
                    .ThenInclude(l => l.Course)
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.QuizAnswers)
                .ToListAsync();
            
            // Debug: Log quiz information
            System.Diagnostics.Debug.WriteLine($"QuizRepository.GetAllAsync - Found {quizzes.Count} quizzes");
            foreach (var quiz in quizzes.Take(3)) // Log first 3 quizzes
            {
                System.Diagnostics.Debug.WriteLine($"  Quiz {quiz.QuizId}: {quiz.Title}, Questions: {quiz.QuizQuestions?.Count ?? 0}");
                if (quiz.QuizQuestions != null)
                {
                    foreach (var question in quiz.QuizQuestions.Take(2)) // Log first 2 questions
                    {
                        System.Diagnostics.Debug.WriteLine($"    Question {question.QuestionId}: {question.QuestionText}, Answers: {question.QuizAnswers?.Count ?? 0}");
                    }
                }
            }
            
            return quizzes;
        }

        public async Task<Quiz> GetByIdAsync(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Lesson)
                    .ThenInclude(l => l.Course)
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.QuizAnswers)
                .FirstOrDefaultAsync(q => q.QuizId == id);
            
            // Debug: Log quiz information
            if (quiz != null)
            {
                System.Diagnostics.Debug.WriteLine($"QuizRepository.GetByIdAsync - Quiz ID: {quiz.QuizId}, Title: {quiz.Title}");
                System.Diagnostics.Debug.WriteLine($"Quiz.QuizQuestions count: {quiz.QuizQuestions?.Count ?? 0}");
                if (quiz.QuizQuestions != null)
                {
                    foreach (var question in quiz.QuizQuestions)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Question {question.QuestionId}: {question.QuestionText}, Answers: {question.QuizAnswers?.Count ?? 0}");
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"QuizRepository.GetByIdAsync - Quiz with ID {id} not found");
            }
            
            return quiz;
        }

        public async Task<int> CreateAsync(Quiz entity)
        {
            _context.Quizzes.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Quiz entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Quizzes.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Quiz entity)
        {
            _context.Quizzes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
