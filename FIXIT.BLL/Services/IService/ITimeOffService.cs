using FIXIT.BLL.DTOs.SchedulingDTOs;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface ITimeOffService
    {
        // Create
        Task<TimeOffDto> CreateTimeOffAsync(CreateTimeOffDto dto);

        // Read
        Task<TimeOffDto> GetTimeOffByIdAsync(int id);
        Task<List<TimeOffDto>> GetCraftsmanTimeOffsAsync(int craftsmanId);
        Task<List<TimeOffDto>> GetActiveTimeOffsAsync(int craftsmanId);
        Task<List<TimeOffDto>> GetUpcomingTimeOffsAsync(int craftsmanId, int days = 30);

        // Update
        Task<TimeOffDto> UpdateTimeOffAsync(int id, CreateTimeOffDto dto);

        // Delete
        Task<bool> DeleteTimeOffAsync(int id);

        // Check
        Task<bool> HasTimeOffOnDateAsync(int craftsmanId, DateTime date);
        Task<TimeOffDto> GetTimeOffByDateAsync(int craftsmanId, DateTime date);
    }
}
