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
    public class TimeOffService : ITimeOffService
    {
        private readonly ITimeOffRepository _timeOffRepository;
        private readonly ICraftsManRepo _craftsmanRepository;
        private readonly IMapper _mapper;

        public TimeOffService(
            ITimeOffRepository timeOffRepository,
            ICraftsManRepo craftsmanRepository,
            IMapper mapper)
        {
            _timeOffRepository = timeOffRepository;
            _craftsmanRepository = craftsmanRepository;
            _mapper = mapper;
        }

        public async Task<TimeOffDto> CreateTimeOffAsync(CreateTimeOffDto dto)
        {
            // Validate DTO
            if (dto == null)
                throw new ValidationException("Time off data cannot be null");

            // Check craftsman exists
            var craftsman = await _craftsmanRepository.GetAsync(dto.CraftsManId);
            if (craftsman == null)
                throw new NotFoundException(nameof(CraftsMan), dto.CraftsManId);

            // Validate dates
            if (dto.StartDate >= dto.EndDate)
                throw new ValidationException("StartDate must be before EndDate");

            if (dto.StartDate < DateTime.UtcNow)
                throw new ValidationException("Cannot create time off in the past");

            // Create entity
            var timeOff = new CraftsManTimeOff
            {
                CraftsManId = dto.CraftsManId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Type = (DAL.Models.TimeOffType)dto.Type,
                Reason = dto.Reason,
                IsApproved = true, // Auto-approved for now
                CreatedAt = DateTime.UtcNow
            };

            await _timeOffRepository.AddAsync(timeOff);
            _timeOffRepository.Save();

            return _mapper.Map<TimeOffDto>(timeOff);
        }

        public async Task<TimeOffDto> GetTimeOffByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid time off ID");

            var timeOff = await _timeOffRepository.GetAsync(id);
            if (timeOff == null)
                throw new NotFoundException(nameof(CraftsManTimeOff), id);

            return _mapper.Map<TimeOffDto>(timeOff);
        }

        public async Task<List<TimeOffDto>> GetCraftsmanTimeOffsAsync(int craftsmanId)
        {
            if (craftsmanId <= 0)
                throw new ValidationException("Invalid craftsman ID");

            var craftsman = await _craftsmanRepository.GetAsync(craftsmanId);
            if (craftsman == null)
                throw new NotFoundException(nameof(CraftsMan), craftsmanId);

            var timeOffs = await _timeOffRepository.GetByCraftsmanIdAsync(craftsmanId);
            return _mapper.Map<List<TimeOffDto>>(timeOffs);
        }

        public async Task<List<TimeOffDto>> GetActiveTimeOffsAsync(int craftsmanId)
        {
            if (craftsmanId <= 0)
                throw new ValidationException("Invalid craftsman ID");

            var timeOffs = await _timeOffRepository.GetActiveDaysAsync(craftsmanId);
            return _mapper.Map<List<TimeOffDto>>(timeOffs);
        }

        public async Task<List<TimeOffDto>> GetUpcomingTimeOffsAsync(int craftsmanId, int days = 30)
        {
            if (craftsmanId <= 0)
                throw new ValidationException("Invalid craftsman ID");

            var timeOffs = await _timeOffRepository.GetUpcomingAsync(craftsmanId, days);
            return _mapper.Map<List<TimeOffDto>>(timeOffs);
        }

        public async Task<TimeOffDto> UpdateTimeOffAsync(int id, CreateTimeOffDto dto)
        {
            if (dto == null)
                throw new ValidationException("Time off data cannot be null");

            var timeOff = await _timeOffRepository.GetAsync(id);
            if (timeOff == null)
                throw new NotFoundException(nameof(CraftsManTimeOff), id);

            // Validate dates
            if (dto.StartDate >= dto.EndDate)
                throw new ValidationException("StartDate must be before EndDate");

            // Update
            timeOff.StartDate = dto.StartDate;
            timeOff.EndDate = dto.EndDate;
            timeOff.Type = (DAL.Models.TimeOffType)dto.Type;
            timeOff.Reason = dto.Reason;

            var updated = _timeOffRepository.Update(timeOff, id);
            if (!updated)
                throw new Exception("Failed to update time off");

            _timeOffRepository.Save();

            return _mapper.Map<TimeOffDto>(timeOff);
        }

        public async Task<bool> DeleteTimeOffAsync(int id)
        {
            var timeOff = await _timeOffRepository.GetAsync(id);
            if (timeOff == null)
                throw new NotFoundException(nameof(CraftsManTimeOff), id);

            _timeOffRepository.Delete(id);
            _timeOffRepository.Save();

            return true;
        }

        public async Task<bool> HasTimeOffOnDateAsync(int craftsmanId, DateTime date)
        {
            return await _timeOffRepository.HasTimeOffOnDateAsync(craftsmanId, date);
        }

        public async Task<TimeOffDto> GetTimeOffByDateAsync(int craftsmanId, DateTime date)
        {
            var timeOff = await _timeOffRepository.GetByDateAsync(craftsmanId, date);
            if (timeOff == null)
                throw new NotFoundException(nameof(CraftsManTimeOff), $"for date {date:yyyy-MM-dd}");

            return _mapper.Map<TimeOffDto>(timeOff);
        }
    }
}
