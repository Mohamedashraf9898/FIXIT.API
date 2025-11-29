using AutoMapper;
using FIXIT.BLL.DTOs.SchedulingDTOs;
using FIXIT.BLL.Exceptions;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Important for EF Core methods

namespace FIXIT.BLL.Services.Service
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly ICraftsManRepo _craftsmanRepository;
        private readonly ITimeOffRepository _timeOffRepository;
        private readonly IMapper _mapper;


        private readonly ITimeSlotService _timeSlotService;
        private readonly ITimeSlotRepository _timeSlotRepository;

        public AvailabilityService(
            IAvailabilityRepository availabilityRepository,
            ICraftsManRepo craftsmanRepository,
            ITimeOffRepository timeOffRepository,
            IMapper mapper,
            ITimeSlotService timeSlotService,       
            ITimeSlotRepository timeSlotRepository) 
        {
            _availabilityRepository = availabilityRepository;
            _craftsmanRepository = craftsmanRepository;
            _timeOffRepository = timeOffRepository;
            _mapper = mapper;
            _timeSlotService = timeSlotService;
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<AvailabilityDto> CreateAvailabilityAsync(CreateAvailabilityDto dto)
        {
            if (dto == null) throw new ValidationException("Availability data cannot be null");

            // Validations
            var craftsman = await _craftsmanRepository.GetAsync(dto.CraftsManId);
            if (craftsman == null) throw new NotFoundException(nameof(CraftsMan), dto.CraftsManId);

            if (!TimeSpan.TryParse(dto.StartTime, out var startTime))
                throw new ValidationException("Invalid StartTime format. Use HH:mm");

            if (!TimeSpan.TryParse(dto.EndTime, out var endTime))
                throw new ValidationException("Invalid EndTime format. Use HH:mm");

            if (startTime >= endTime)
                throw new ValidationException("StartTime must be before EndTime");

            var existing = await _availabilityRepository.GetByDayOfWeekAsync(dto.CraftsManId, dto.DayOfWeek);
            if (existing != null)
                throw new ValidationException($"Availability already exists for {dto.DayOfWeek}");

            var availability = new CraftsManAvailability
            {
                CraftsManId = dto.CraftsManId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = dto.IsAvailable,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _availabilityRepository.AddAsync(availability);
            _availabilityRepository.Save();

           
            await _timeSlotService.GenerateSlotsForCraftsmanAsync(dto.CraftsManId, 30);

            return _mapper.Map<AvailabilityDto>(availability);
        }

        public async Task<AvailabilityDto> UpdateAvailabilityAsync(int id, UpdateAvailabilityDto dto)
        {
            if (dto == null) throw new ValidationException("Data cannot be null");

            var availability = await _availabilityRepository.GetAsync(id);
            if (availability == null) throw new NotFoundException(nameof(CraftsManAvailability), id);

            if (!TimeSpan.TryParse(dto.StartTime, out var startTime))
                throw new ValidationException("Invalid StartTime");
            if (!TimeSpan.TryParse(dto.EndTime, out var endTime))
                throw new ValidationException("Invalid EndTime");

            // Update Configuration
            availability.StartTime = startTime;
            availability.EndTime = endTime;
            availability.IsAvailable = dto.IsAvailable;
            availability.UpdatedAt = DateTime.UtcNow;

            _availabilityRepository.Update(availability, id);
            _availabilityRepository.Save();

          
            var futureSlots = await _timeSlotRepository.GetAvailableSlotsAsync(availability.CraftsManId, DateTime.Today);

            var slotsToDelete = futureSlots.Where(s => s.Date >= DateTime.Today && s.Date.DayOfWeek == availability.DayOfWeek).ToList();

            if (slotsToDelete.Any())
            {
                foreach (var slot in slotsToDelete)
                {
                    _timeSlotRepository.Delete(slot.Id);
                }
                _timeSlotRepository.Save();
            }


            await _timeSlotService.GenerateSlotsForCraftsmanAsync(availability.CraftsManId, 30);

            return _mapper.Map<AvailabilityDto>(availability);
        }

        public async Task<List<TimeSlotDto>> GetDailyTimeSlotsAsync(int craftsmanId, DateTime date, int durationMinutes)
        {
            var slots = await _timeSlotRepository.GetAvailableSlotsAsync(craftsmanId, date);

            return slots.Select(s => new TimeSlotDto
            {
                Id = s.Id, // „Â„ ⁄‘«‰ «·ÕÃ“
                Time = DateTime.Today.Add(s.StartTime).ToString("hh:mm tt"),
                IsAvailable = true,
                StartTime = s.Date.Add(s.StartTime)
            }).ToList();
        }

        public async Task<bool> DeleteAvailabilityAsync(int id)
        {
            var availability = await _availabilityRepository.GetAsync(id);
            if (availability == null) throw new NotFoundException(nameof(CraftsManAvailability), id);
            _availabilityRepository.Delete(id);
            _availabilityRepository.Save();
            return true;
        }

        public async Task<AvailabilityDto> GetAvailabilityByIdAsync(int id)
        {
            var availability = await _availabilityRepository.GetAsync(id);
            if (availability == null) throw new NotFoundException(nameof(CraftsManAvailability), id);
            return _mapper.Map<AvailabilityDto>(availability);
        }

        public async Task<List<AvailabilityDto>> GetCraftsmanAvailabilityAsync(int craftsmanId)
        {
            var availabilities = await _availabilityRepository.GetByCraftsmanIdAsync(craftsmanId);
            return _mapper.Map<List<AvailabilityDto>>(availabilities);
        }

        public async Task<AvailabilityDto> GetByDayAsync(int craftsmanId, DayOfWeek dayOfWeek)
        {
            var availability = await _availabilityRepository.GetByDayOfWeekAsync(craftsmanId, dayOfWeek);
            if (availability == null) throw new NotFoundException(nameof(CraftsManAvailability), $"for {dayOfWeek}");
            return _mapper.Map<AvailabilityDto>(availability);
        }

        public async Task<bool> IsAvailableOnDayAsync(int craftsmanId, DayOfWeek dayOfWeek)
        {
            return await _availabilityRepository.IsAvailableOnDayAsync(craftsmanId, dayOfWeek);
        }

        public async Task<bool> IsAvailableAtTimeAsync(int craftsmanId, DayOfWeek dayOfWeek, TimeSpan time)
        {
            return await _availabilityRepository.IsAvailableAtTimeAsync(craftsmanId, dayOfWeek, time);
        }
    }
}