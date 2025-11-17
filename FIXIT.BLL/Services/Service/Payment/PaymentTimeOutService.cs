using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service.Payment
{
    public class PaymentTimeOutService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentTimeOutService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // شغّل كل 5 دقائق
            _timer = new Timer(async _ => await CheckPendingPaymentsAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        private async Task CheckPendingPaymentsAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceRequestRepository = scope.ServiceProvider.GetRequiredService<IServiceRequestRepository>();
                var offerRepository = scope.ServiceProvider.GetRequiredService<IOfferRepository>();

                // جلب كل Requests اللي محتاجة فحص
                var pendingRequests = (await serviceRequestRepository.GetAllAsync())
                    .Where(r => r.Status == ServiceRequestStatus.WaitingForClientPayment
                                && r.WaitingForClientPaymentAt.HasValue
                                && r.WaitingForClientPaymentAt.Value.AddHours(5) <= DateTime.UtcNow)
                    .ToList();

                foreach (var request in pendingRequests)
                {
                    // جلب Offer المرتبط مباشرة بالـ Request
                    var offer = await offerRepository.GetAsync(request.ServicesRequestId);

                    if (offer != null)
                    {
                        offer.Status = OfferStatus.Cancelled;
                        offerRepository.Update(offer, offer.Id);
                    }

                    // تحديث حالة الـ Request
                    request.Status = ServiceRequestStatus.CancelledDueToNonPayment;
                    request.IsCancelled = true;
                    serviceRequestRepository.Update(request, request.ServicesRequestId);
                }

                // حفظ التغييرات async
                offerRepository.Save();
                serviceRequestRepository.Save();
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
