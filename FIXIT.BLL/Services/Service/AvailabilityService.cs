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
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly ICraftsManRepo _craftsmanRepository;
        private readonly ITimeOffRepository _timeOffRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IMapper _mapper;

        public AvailabilityService(
            IAvailabilityRepository availabilityRepository,
            ICraftsManRepo craftsmanRepository,
            ITimeOffRepository timeOffRepository,
            IServiceRequestRepository serviceRequestRepository,
            IMapper mapper)
        {
            _availabilityRepository = availabilityRepository;
            _craftsmanRepository = craftsmanRepository;
            _timeOffRepository = timeOffRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _mapper = mapper;
        }

        public async Task<AvailabilityDto> CreateAvailabilityAsync(CreateAvailabilityDto dto)
        {
            // Validate DTO
            if (dto == null)
                throw new ValidationException("Availability data cannot be null");

            // Check if craftsman exists
            var craftsman = await _craftsmanRepository.GetAsync(dto.CraftsManId);
            if (craftsman == null)
                throw new NotFoundException(nameof(CraftsMan), dto.CraftsManId);

            // Validate time format
            if (!TimeSpan.TryParse(dto.StartTime, out var startTime))
                throw new ValidationException("Invalid StartTime format. Use HH:mm");

            if (!TimeSpan.TryParse(dto.EndTime, out var endTime))
                throw new ValidationException("Invalid EndTime format. Use HH:mm");

            // Validate start < end
            if (startTime >= endTime)
                throw new ValidationException("StartTime must be before EndTime");

            // Check if availability already exists for this day
            var existing = await _availabilityRepository.GetByDayOfWeekAsync(dto.CraftsManId, dto.DayOfWeek);
            if (existing != null)
                throw new ValidationException($"Availability already exists for {dto.DayOfWeek}");

            // Create entity
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

            return _mapper.Map<AvailabilityDto>(availability);
        }
        public async Task<List<TimeSlotDto>> GetDailyTimeSlotsAsync(int craftsmanId, DateTime date, int durationMinutes)
        {
            var slots = new List<TimeSlotDto>();

            // √.  ‘Ìﬂ ·Ê «· «—ÌŒ ﬁœÌ„ (›« )
            if (date.Date < DateTime.UtcNow.Date)
                return slots; // „›Ì‘ „Ê«⁄Ìœ ›Ì «·„«÷Ì

            // ».  ‘Ìﬂ «·≈Ã«“«  (TimeOff)
            bool onLeave = await _timeOffRepository.HasTimeOffOnDateAsync(craftsmanId, date);
            if (onLeave)
                return slots; // «·’‰«Ì⁄Ì ›Ì ≈Ã«“…

            // Ã.  ‘Ìﬂ „Ê«⁄Ìœ «·⁄„· (Availability)
            var dayOfWeek = date.DayOfWeek;
            var availability = await _availabilityRepository.GetByDayOfWeekAsync(craftsmanId, dayOfWeek);

            if (availability == null || !availability.IsAvailable)
                return slots; // „‘ ÌÊ„ ⁄„·

            // œ. Â«  «·ÕÃÊ“«  «·„ƒﬂœ… ··ÌÊ„ œÂ (Existing Bookings)
            var bookedRequests = await _serviceRequestRepository.GetByDateAsync(craftsmanId, date);

            // Â‹. «·ŒÊ«—“„Ì…:  ﬁÿÌ⁄ «·Êﬁ  ·‘—«∆Õ (Slicing Time)
            TimeSpan currentStart = availability.StartTime;
            TimeSpan workEnd = availability.EndTime;

            // »‰⁄„· Slot ﬂ· 30 œﬁÌﬁ… ⁄‘«‰ ‰œÌ „—Ê‰… ··⁄„Ì·
            TimeSpan step = TimeSpan.FromMinutes(durationMinutes);

            // «··Ê» »Ì„‘Ì „‰ »œ«Ì… «·ÌÊ„ ·Õœ ‰Â«Ì Â
            while (currentStart.Add(TimeSpan.FromMinutes(durationMinutes)) <= workEnd)
            {
                var slotEnd = currentStart.Add(TimeSpan.FromMinutes(durationMinutes));

                // 1. «· √ﬂœ ≈‰ «·Êﬁ  œÂ ·”Â „Ã«‘ (·Ê «·‰Â«—œ…)
                bool isPastTime = date.Date == DateTime.UtcNow.Date && currentStart <= DateTime.UtcNow.TimeOfDay;

                if (!isPastTime)
                {
                    // 2. «· √ﬂœ „‰ ⁄œ„ ÊÃÊœ  œ«Œ· „⁄ ÕÃ“  «‰Ì (Conflict Check)
                    bool isConflict =
                    bookedRequests.Any(req =>
                    IsOverlapping(currentStart, slotEnd, req.ServiceStartTime.TimeOfDay, req.ServiceEndTime.Value.TimeOfDay));

                    // ·Ê «·Êﬁ  „‰«”» Ê„›Ì‘  œ«Œ·° ÷Ì›Â ··ﬁ«∆„…
                    if (!isConflict)
                    {
                        slots.Add(new TimeSlotDto
                        {
                            // œÂ «··Ì »ÌŸÂ— ··ÌÊ“—: "09:30 AM"
                            Time = DateTime.Today.Add(currentStart).ToString("hh:mm tt"),
                            IsAvailable = true,
                            // œÂ «··Ì »Ì »⁄  ··»«ﬂ ≈‰œ ·„« ÌŒ «—: "2025-11-20T09:30:00"
                            StartTime = date.Date.Add(currentStart)
                        });
                    }
                }

                // «‰ﬁ· ⁄·Ï «·‰’ ”«⁄… «··Ì »⁄œÂ«
                currentStart = currentStart.Add(step);
            }

            return slots;
        }
        private bool IsOverlapping(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            return start1 < end2 && start2 < end1;
        }

        public async Task<AvailabilityDto> GetAvailabilityByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid availability ID");

            var availability = await _availabilityRepository.GetAsync(id);
            if (availability == null)
                throw new NotFoundException(nameof(CraftsManAvailability), id);

            return _mapper.Map<AvailabilityDto>(availability);
        }

        public async Task<List<AvailabilityDto>> GetCraftsmanAvailabilityAsync(int craftsmanId)
        {
            if (craftsmanId <= 0)
                throw new ValidationException("Invalid craftsman ID");

            var craftsman = await _craftsmanRepository.GetAsync(craftsmanId);
            if (craftsman == null)
                throw new NotFoundException(nameof(CraftsMan), craftsmanId);

            var availabilities = await _availabilityRepository.GetByCraftsmanIdAsync(craftsmanId);
            return _mapper.Map<List<AvailabilityDto>>(availabilities);
        }

        public async Task<AvailabilityDto> GetByDayAsync(int craftsmanId, DayOfWeek dayOfWeek)
        {
            var availability = await _availabilityRepository.GetByDayOfWeekAsync(craftsmanId, dayOfWeek);
            if (availability == null)
                throw new NotFoundException(nameof(CraftsManAvailability), $"for {dayOfWeek}");

            return _mapper.Map<AvailabilityDto>(availability);
        }

        public async Task<AvailabilityDto> UpdateAvailabilityAsync(int id, UpdateAvailabilityDto dto)
        {
            if (dto == null)
                throw new ValidationException("Availability data cannot be null");

            var availability = await _availabilityRepository.GetAsync(id);
            if (availability == null)
                throw new NotFoundException(nameof(CraftsManAvailability), id);

            // Parse times
            if (!TimeSpan.TryParse(dto.StartTime, out var startTime))
                throw new ValidationException("Invalid StartTime format. Use HH:mm");

            if (!TimeSpan.TryParse(dto.EndTime, out var endTime))
                throw new ValidationException("Invalid EndTime format. Use HH:mm");

            // Validate
            if (startTime >= endTime)
                throw new ValidationException("StartTime must be before EndTime");

            // Update
            availability.StartTime = startTime;
            availability.EndTime = endTime;
            availability.IsAvailable = dto.IsAvailable;
            availability.UpdatedAt = DateTime.UtcNow;

            var updated = _availabilityRepository.Update(availability, id);
            if (!updated)
                throw new Exception("Failed to update availability");

            _availabilityRepository.Save();

            return _mapper.Map<AvailabilityDto>(availability);
        }

        public async Task<bool> DeleteAvailabilityAsync(int id)
        {
            var availability = await _availabilityRepository.GetAsync(id);
            if (availability == null)
                throw new NotFoundException(nameof(CraftsManAvailability), id);

            _availabilityRepository.Delete(id);
            _availabilityRepository.Save();

            return true;
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
