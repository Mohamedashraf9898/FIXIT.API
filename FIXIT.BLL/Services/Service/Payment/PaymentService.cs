using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService.Payment;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Stripe;

namespace FIXIT.BLL.Services.Service.Payment
{
    public class PaymentService(IServiceRequestRepository serviceRequestService, IMapper mapper, FixItDbContext dbContext
        , IConfiguration configuration,ILogger<ServicesRequest> logger) : IPaymentService
    {

        private readonly FixItDbContext _dbContext = dbContext;

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
            #region gehad

            decimal totalAmount = serviceRequest.TotalAmount;
            decimal platformCommission = totalAmount * 0.25m;
            decimal craftsmanAmount = totalAmount - platformCommission;

            // 2️⃣ جلب محفظة الحرفي
            if (serviceRequest.CraftsManId.HasValue)
            {
                var craftsman = await _dbContext.CraftsMan
                    .Include(c => c.Wallet)
                    .FirstOrDefaultAsync(c => c.Id == serviceRequest.CraftsManId.Value);

                if (craftsman != null)
                {
                    if (craftsman.Wallet == null)
                    {
                        craftsman.Wallet = new Wallet()
                        {
                            CraftsManId = craftsman.Id,
                            Balance = 0
                        };
                    }// we made wallet to have money in

                    // add monet to this wallet
                    craftsman.Wallet.Balance += craftsmanAmount;

                    //how to record changes to this wallet => WalletTransaction=> to save every change
                    var transaction = new WalletTransaction
                    {
                        CraftsManId = craftsman.Id,
                        ServicesRequestId = serviceRequest.ServicesRequestId,
                        Amount = craftsmanAmount,
                        TransactionDate = DateTime.UtcNow,
                        Type = TransactionType.Income

                    };

                    await _dbContext.WalletTransactions.AddAsync(transaction);
                    //we link transaction with that service requesttt like history of transactionss in service request
                    serviceRequest.WalletTransaction = transaction;

                    //udate db
                    _dbContext.CraftsMan.Update(craftsman);
                    await _dbContext.SaveChangesAsync();
                }

            }

            #endregion

            serviceRequest.Status= isPayed ? ServiceRequestStatus.InProgress : ServiceRequestStatus.WaitingForClientPayment;
            serviceRequestService.Update(serviceRequest, serviceRequest.ServicesRequestId);
            serviceRequestService.Save();
            
            return serviceRequest;


        }

     

    }

}