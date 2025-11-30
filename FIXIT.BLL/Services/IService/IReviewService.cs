using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.ReviewDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FIXIT.BLL.Services.IService
{
    public interface IReviewService 
    {
        Task<IEnumerable<GetAllReviewsDTO>> GetAllReviewsAsync();
        // Task<GetAllReviewsDTO> UpdateReviewAsync(int reviewId, UpdateReviewDTO reviewDto);
        Task<GetAllReviewsDTO> UpdateReviewAsync(int reviewId, UpdateReviewDTO reviewDto);
            Task<bool> DeleteReviewAsync(int reviewId);
        Task<GetAllReviewsDTO> CreateReviewAsync(CreateReviewDTO reviewDto);
            Task<GetAllReviewsDTO> GetReviewByIdAsync(int reviewId);
        Task<IEnumerable<GetAllReviewsDTO>> GetReviewsForCraftsmanAsync(int craftsmanId);
    }
}

