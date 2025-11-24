using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService.Payment;
using FIXIT.DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace FIXIT.BLL.Services.Service.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IServiceRequestRepository _serviceRequestService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IServiceRequestRepository serviceRequestService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<PaymentService> logger)
        {
            _serviceRequestService = serviceRequestService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ReadServiceRequestDto?> CreateOrUpdatePaymentIntent(int serviceRequestId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
            var serviceRequest = await _serviceRequestService.GetAsync(serviceRequestId);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found", serviceRequestId);

            if (!serviceRequest.TotalAmount.HasValue || serviceRequest.TotalAmount <= 0)
            {
                serviceRequest.TotalAmount = 200;
            }

            var amount = (long)(serviceRequest.TotalAmount.Value * 100);
            PaymentIntentService intentService = new PaymentIntentService();
            PaymentIntent? paymentIntent = null;

            if (string.IsNullOrEmpty(serviceRequest.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await intentService.CreateAsync(options);
                serviceRequest.PaymentIntentId = paymentIntent.Id;
                serviceRequest.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = amount
                };
                await intentService.UpdateAsync(serviceRequest.PaymentIntentId, options);
            }

            _serviceRequestService.Update(serviceRequest, serviceRequestId);
            _serviceRequestService.Save();

            return _mapper.Map<ReadServiceRequestDto>(serviceRequest);
        }

        public async Task UpdatePaymentStatus(string requestBody, string stripeSignature)
        {
            try
            {
                var webhookSecret = _configuration.GetSection("StripeSettings:Webhook:Secret").Value;

                if (string.IsNullOrEmpty(webhookSecret))
                {
                    _logger.LogError("❌ Webhook secret is NULL!");
                    return;
                }

                var stripeEvent = EventUtility.ConstructEvent(
                    requestBody,
                    stripeSignature,
                    webhookSecret
                );

                _logger.LogInformation("✅ Event: {Type}", stripeEvent.Type);

                if (stripeEvent.Type != EventTypes.PaymentIntentSucceeded &&
                    stripeEvent.Type != EventTypes.PaymentIntentPaymentFailed)
                {
                    return;
                }

                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                if (paymentIntent == null)
                {
                    return;
                }

                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var serviceRequest = await UpdatePayment(paymentIntent.Id, true);
                    _logger.LogInformation("✅ Payment succeeded for ServiceRequest: {Id}", serviceRequest.ServicesRequestId);
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
                {
                    var serviceRequest = await UpdatePayment(paymentIntent.Id, false);
                    _logger.LogInformation("⚠️ Payment failed for ServiceRequest: {Id}", serviceRequest.ServicesRequestId);
                }
            }
            catch (Stripe.StripeException)
            {
                // Silently ignore signature validation errors (duplicate webhooks)
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error");
                throw;
            }
        }

        private async Task<ServicesRequest> UpdatePayment(string paymentIntentId, bool isPayed)
        {
            var serviceRequest = await _serviceRequestService.GetByIntentId(paymentIntentId);

            if (serviceRequest == null)
            {
                throw new NotFoundException("Service request not found", paymentIntentId);
            }

            var expectedStatus = isPayed
                ? ServiceRequestStatus.InProgress
                : ServiceRequestStatus.WaitingForClientPayment;

            if (serviceRequest.Status == expectedStatus)
            {
                return serviceRequest;
            }

            serviceRequest.Status = expectedStatus;

            _serviceRequestService.Update(serviceRequest, serviceRequest.ServicesRequestId);
            _serviceRequestService.Save();

            return serviceRequest;
        }
    }
}