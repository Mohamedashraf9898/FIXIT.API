using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Services.IService.Payment
{
    public interface IPaymentService
    {

        Task<ReadServiceRequestDto?> CreateOrUpdatePaymentIntent(int serviceRequestId);
        Task UpdatePaymentStatus(string requestBody, string paymentStatus);
    }
}
