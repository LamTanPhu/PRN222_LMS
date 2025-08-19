using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICourseReviewService
    {
        Task<List<CourseReview>> GetAllAsync();
        Task<CourseReview> GetByIdAsync(int? id);
        Task<bool> DeleteAsync(int? id);
        Task CreateAsync(CourseReview review);
        Task UpdateAsync(CourseReview review);
    }
}
