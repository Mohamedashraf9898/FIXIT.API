using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService.Payment;
using FIXIT.DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace FIXIT.BLL.Services.Service.Payment
{
    public class PaymentService(IServiceRequestRepository serviceRequestService, IMapper mapper, IConfiguration configuration,ILogger<ServicesRequest> logger) : IPaymentService
    {
        public async Task<ReadServiceRequestDto?> CreateOrUpdatePaymentIntent(int serviceRequestId)
        {
            StripeConfiguration.ApiKey = configuration["StripeSettings:PublishableKey"];
            var serviceRequest = await serviceRequestService.GetAsync(serviceRequestId);
            if (serviceRequest == null)
            {
                throw new NotFoundException("Service request not found", serviceRequestId);
            }

            if (serviceRequest.TotalAmount > 0)
            {

            }
            PaymentIntent? paymentIntent = null;
            PaymentIntentService intentService = new PaymentIntentService();
            //create payment intent
            if (string.IsNullOrEmpty(serviceRequest.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {


                    Amount = (long)serviceRequest.TotalAmount * 100,
                    Currency = "USD",
                    PaymentMethodTypes = new List<string>()
                    {
                        "card"
                    }

                };
                paymentIntent = await intentService.CreateAsync(options);
                serviceRequest.PaymentIntentId = paymentIntent.Id;
                serviceRequest.ClientSecret = paymentIntent.ClientSecret;

            }
            else // update payment intent
            {
                var optoins = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)serviceRequest.TotalAmount * 100,
                };

                await intentService.UpdateAsync(serviceRequest.PaymentIntentId, optoins);
            }

            serviceRequestService.Update(serviceRequest, serviceRequestId);
            serviceRequestService.Save();
            var dto = mapper.Map<ReadServiceRequestDto>(serviceRequest);
            return dto;
        }

        public async Task UpdatePaymentStatus(string requestBody, string paymentStatus)
        {



            //var stripeEvent = EventUtility.ParseEvent(json);
            //var signatureHeader = Request.Headers["Stripe-Signature"];

            var stripeEvent = EventUtility.ConstructEvent(requestBody, paymentStatus, configuration.GetSection("StripeSettings:Webhook:Secret").Value);
            //signatureHeader, endpointSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            ServicesRequest? serviceRequest ;

            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {

                serviceRequest = await UpdatePayment(paymentIntent.Id,true);
                logger.LogInformation($"Payment succeeded for ServiceRequestId: {serviceRequest.ServicesRequestId}");

            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                serviceRequest = await UpdatePayment(paymentIntent.Id,false);
                logger.LogInformation($"Payment failed for ServiceRequestId: {serviceRequest.ServicesRequestId}");  
            }


        }


        private async Task<ServicesRequest> UpdatePayment(string paymentIntentId,bool isPayed)
        {

            var serviceRequest = await serviceRequestService.GetByIntentId(paymentIntentId);
            if (serviceRequest == null)
            {
                throw new NotFoundException("Service request not found", paymentIntentId);
            }
            serviceRequest.Status= isPayed ? ServiceRequestStatus.InProgress : ServiceRequestStatus.WaitingForClientPayment;
            serviceRequestService.Update(serviceRequest, serviceRequest.ServicesRequestId);
            serviceRequestService.Save();
            return serviceRequest;


        }

     

    }

}