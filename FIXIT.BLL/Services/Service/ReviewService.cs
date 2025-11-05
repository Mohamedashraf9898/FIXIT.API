using AutoMapper;
using FIXIT.BLL.DTOs.ReviewDTOs;
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
            var serviceRequest = await _serviceRequestRepository.GetAsync(reviewDto.ServicesRequestId);

            if (serviceRequest == null)
            {
                throw new Exception("This service request does not exist.");
            }
            if (serviceRequest.Status != ServiceRequestStatus.Completed)
            {
                throw new Exception("You can only review completed jobs.");
            }

            var reviewExists = await _reviewRepository.DoesReviewExistForRequestAsync(reviewDto.ServicesRequestId);

            if (reviewExists)
            {
                throw new Exception("This job has already been reviewed.");
            }

            
           

            var newReview = _mapper.Map<Review>(reviewDto, opts => {
                opts.Items["ServiceRequest"] = serviceRequest;
            });


            await _reviewRepository.AddAsync(newReview);
            _reviewRepository.Save(); 

            return _mapper.Map<GetAllReviewsDTO>(newReview);
        }


        public async Task<GetAllReviewsDTO> UpdateReviewAsync(int reviewId, UpdateReviewDTO reviewDto)
        {
            var reviewToUpdate = await _reviewRepository.GetAsync(reviewId);

            if (reviewToUpdate == null)
            {
                throw new Exception("Review not found.");
            }
            _mapper.Map(reviewDto, reviewToUpdate);
            _reviewRepository.Update(reviewToUpdate, reviewId);
            _reviewRepository.Save();
            return _mapper.Map<GetAllReviewsDTO>(reviewToUpdate);
        }

       
        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var reviewToDelete = await _reviewRepository.GetAsync(reviewId);

            if (reviewToDelete == null)
            {
                throw new Exception("Review not found.");
            }
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
            // 1. Get the review using your generic repository's GetAsync
            var review = await _reviewRepository.GetAsync(reviewId);

            // 2. Check if it was found
            if (review == null)
            {
                throw new Exception("Review not found.");
            }

            // 3. Map it to the DTO and return it
            return _mapper.Map<GetAllReviewsDTO>(review);
        }

        // --- ADD THIS "GET FOR CRAFTSMAN" METHOD ---
        public async Task<IEnumerable<GetAllReviewsDTO>> GetReviewsForCraftsmanAsync(int craftsmanId)
        {
            // 1. Get reviews using the new repository method you created
            var reviews = await _reviewRepository.GetReviewsForCraftsmanAsync(craftsmanId);

            // 2. Map the list and return it (it's ok if the list is empty)
            return _mapper.Map<IEnumerable<GetAllReviewsDTO>>(reviews);
        }


    }
}
