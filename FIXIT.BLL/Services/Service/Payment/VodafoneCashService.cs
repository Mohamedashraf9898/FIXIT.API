using FIXIT.BLL.Services.IService.Payment;
using FIXIT.DAL.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service.Payment
{
    public class VodafoneCashService : IVodafoneCashService
    {
        private readonly HttpClient _httpClient;
        private readonly VodafoneCashSettings _settings;

        public VodafoneCashService(HttpClient httpClient, IOptions<VodafoneCashSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<bool> SendMoneyAsync(string phoneNumber, decimal amount)
        {
            var requestBody = new
            {
                merchantId = _settings.MerchantId,
                phoneNumber,
                amount,
                pin = _settings.PinCode
            };

            var response = await _httpClient.PostAsJsonAsync($"{_settings.ApiBaseUrl}/transfer", requestBody);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> TransferAsync(string phoneNumber, decimal amount)
        {
            //need to connect to VOdafone

            await Task.Delay(1000); // Simulate API call delay
            return true; // Temporary always success
        }
    }

}
