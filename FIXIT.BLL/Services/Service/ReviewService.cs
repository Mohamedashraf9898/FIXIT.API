using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ReviewDTOs;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IGenericRepository<ServicesRequest> _serviceRequestRepository;
        private readonly IMapper _mapper;

        
        public ReviewService(
            IReviewRepository reviewRepository,
            IGenericRepository<ServicesRequest> serviceRequestRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _mapper = mapper;
        }
		public async Task<GetAllReviewsDTO> CreateReviewAsync(CreateReviewDTO reviewDto)
		{
			if (reviewDto == null)
				throw new ValidationException("Review data cannot be null.");

			var serviceRequest = await _serviceRequestRepository.GetAsync(reviewDto.ServicesRequestId);
			if (serviceRequest == null)
				throw new NotFoundException(nameof(ServicesRequest), reviewDto.ServicesRequestId);

			if (serviceRequest.Status != ServiceRequestStatus.Completed)
				throw new ValidationException("You can only review completed jobs.");

			var reviewExists = await _reviewRepository.DoesReviewExistForRequestAsync(reviewDto.ServicesRequestId);
			if (reviewExists)
				throw new ValidationException("This job has already been reviewed.");
			var review = new CreateReviewDTO
			{
				Comment = reviewDto.Comment,
				RatingValue = reviewDto.RatingValue,
				ServicesRequestId=reviewDto.ServicesRequestId,
				ClientId=serviceRequest.ClientId,
				CraftsManId = serviceRequest.CraftsManId
			};
			var newReview = _mapper.Map<Review>(review);

			await _reviewRepository.AddAsync(newReview);
			_reviewRepository.Save();

			return _mapper.Map<GetAllReviewsDTO>(newReview);
		}

		public async Task<GetAllReviewsDTO> UpdateReviewAsync(int reviewId, UpdateReviewDTO reviewDto)
		{
			var reviewToUpdate = await _reviewRepository.GetAsync(reviewId);
			if (reviewToUpdate == null)
				throw new NotFoundException(nameof(Review), reviewId);

			_mapper.Map(reviewDto, reviewToUpdate);
			_reviewRepository.Update(reviewToUpdate, reviewId);
			_reviewRepository.Save();

			return _mapper.Map<GetAllReviewsDTO>(reviewToUpdate);
		}

		public async Task<bool> DeleteReviewAsync(int reviewId)
		{
			var reviewToDelete = await _reviewRepository.GetAsync(reviewId);
			if (reviewToDelete == null)
				throw new NotFoundException(nameof(Review), reviewId);

			_reviewRepository.Delete(reviewId);
			_reviewRepository.Save();
			return true;
		}

		public async Task<IEnumerable<GetAllReviewsDTO>> GetAllReviewsAsync()
		{
			var allReviews = await _reviewRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<GetAllReviewsDTO>>(allReviews);
		}

		public async Task<GetAllReviewsDTO> GetReviewByIdAsync(int reviewId)
		{
			var review = await _reviewRepository.GetAsync(reviewId);
			if (review == null)
				throw new NotFoundException(nameof(Review), reviewId);

			return _mapper.Map<GetAllReviewsDTO>(review);
		}

		public async Task<IEnumerable<GetAllReviewsDTO>> GetReviewsForCraftsmanAsync(int craftsmanId)
		{
			var reviews = await _reviewRepository.GetReviewsForCraftsmanAsync(craftsmanId);
			return _mapper.Map<IEnumerable<GetAllReviewsDTO>>(reviews);
		}
	}


}

