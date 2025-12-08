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
    public class TimeSlotService : ITimeSlotService
    {
        private readonly IAvailabilityRepository _availabilityRepo;
        private readonly ITimeSlotRepository _timeSlotRepo;

        public TimeSlotService(
            IAvailabilityRepository availabilityRepo,
            ITimeSlotRepository timeSlotRepo)
        {
            _availabilityRepo = availabilityRepo;
            _timeSlotRepo = timeSlotRepo;
        }

        public async Task GenerateSlotsForCraftsmanAsync(int craftsmanId, int daysAhead = 30)
        {
            var config = await _availabilityRepo.GetByCraftsmanIdAsync(craftsmanId);
            if (config == null || !config.Any())
                throw new Exception("No availability configuration found for this craftsman.");

            DateTime startDate = DateTime.UtcNow.Date;
            var slotsToAdd = new List<TimeSlot>();

            for (int i = 0; i < daysAhead; i++)
            {
                DateTime targetDate = startDate.AddDays(i);
                DayOfWeek dayName = targetDate.DayOfWeek;

                var dayConfig = config.FirstOrDefault(c => c.DayOfWeek == dayName && c.IsAvailable);
                if (dayConfig == null) continue;

                bool alreadyGenerated = await _timeSlotRepo.ExistsAsync(craftsmanId, targetDate);
                if (alreadyGenerated) continue;

                TimeSpan current = dayConfig.StartTime;
                TimeSpan end = dayConfig.EndTime;

                while (current.Add(TimeSpan.FromHours(1)) <= end)
                {
                    slotsToAdd.Add(new TimeSlot
                    {
                        CraftsManId = craftsmanId,
                        Date = targetDate,
                        StartTime = current,
                        EndTime = current.Add(TimeSpan.FromHours(1)),
                        Status = SlotStatus.Available,
                        CreatedAt = DateTime.UtcNow
                    });

                    current = current.Add(TimeSpan.FromHours(1));
                }
            }

            if (slotsToAdd.Any())
            {
                await _timeSlotRepo.AddRangeAsync(slotsToAdd);
                _timeSlotRepo.Save();
            }
        }

        public async Task<List<TimeSlot>> GetAvailableSlotsAsync(int craftsmanId, DateTime date)
        {
            return await _timeSlotRepo.GetAvailableSlotsAsync(craftsmanId, date);
        }
        public async Task<List<TimeSlot>> GetCraftsmanScheduleAsync(int craftsmanId, DateTime date)
        {
            return await _timeSlotRepo.GetAllSlotsByDateAsync(craftsmanId, date);
        }

        public async Task<bool> ToggleSlotStatusAsync(int slotId, int craftsmanId)
        {
            var slot = await _timeSlotRepo.GetAsync(slotId);

            if (slot == null)
                throw new KeyNotFoundException("Slot not found");

            if (slot.CraftsManId != craftsmanId)
                throw new UnauthorizedAccessException("Not authorized to modify this slot.");

            switch (slot.Status)
            {
                case SlotStatus.Available:
                    slot.Status = SlotStatus.Disabled;
                    break;

                case SlotStatus.Disabled:
                    slot.Status = SlotStatus.Available;
                    break;

                case SlotStatus.Booked:
                    throw new InvalidOperationException("Cannot disable a booked slot. Reject the request first.");

                default:
                    throw new InvalidOperationException("Cannot modify slot status.");
            }

            _timeSlotRepo.Update(slot, slot.Id);
            _timeSlotRepo.Save();

            return true;
        }
    }
}

