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
    public class CourseService : ICourseService
    {
        private readonly CourseRepository courseRepository;

        public CourseService(CourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await courseRepository.GetAllAsync();
        }

        public async Task<Course> GetByIdAsync(int? id)
        {
            return await courseRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var course = await GetByIdAsync(id);
            if (course != null)
            {
                return await courseRepository.RemoveAsync(course);
            }
            return false;
        }
    }
}
