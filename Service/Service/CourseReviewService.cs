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
    public class CourseReviewService : ICourseReviewService
    {
        private readonly CourseReviewRepository courseReviewRepository;

        public CourseReviewService()
        {
            courseReviewRepository = new CourseReviewRepository();
        }

        public async Task<List<CourseReview>> GetAllAsync()
        {
            return await courseReviewRepository.GetAllAsync();
        }

        public async Task<CourseReview> GetByIdAsync(int? id)
        {
            return await courseReviewRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var review = await GetByIdAsync(id);
            if (review != null)
            {
                return await courseReviewRepository.RemoveAsync(review);
            }
            return false;
        }

        public async Task CreateAsync(CourseReview review)
        {
            await courseReviewRepository.CreateAsync(review);
        }

        public async Task UpdateAsync(CourseReview review)
        {
            await courseReviewRepository.UpdateAsync(review);
        }
    }
}
