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
    public class LessonService : ILessonService
    {
        private readonly LessonRepository lessonRepository;

        public LessonService()
        {
            lessonRepository = new LessonRepository();
        }

        public async Task<List<Lesson>> GetAllAsync()
        {
            return await lessonRepository.GetAllAsync();
        }

        public async Task<Lesson> GetByIdAsync(int? id)
        {
            return await lessonRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var lesson = await GetByIdAsync(id);
            if (lesson != null)
            {
                return await lessonRepository.RemoveAsync(lesson);
            }
            return false;
        }

        public async Task<Lesson> CreateAsync(Lesson lesson, int courseId)
        {
            lesson.CourseId = courseId;
            await lessonRepository.CreateAsync(lesson);
            return lesson;
        }
    }
}
