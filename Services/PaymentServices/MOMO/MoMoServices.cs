using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net.Http;


namespace API_Test1.Services.PaymentServices.MOMO
{
    public class MoMoServices : IMoMoServices
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MoMoServices(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }

        public async Task<dynamic> CreatePaymentAsync(Orders model)
        {
            var OrderInfo = "Khách hàng: " + model.FullName + ". Nội dung: Thanh toan don hang tai E-commerce";
            var rawData =
                $"partnerCode={_configuration["MomoAPI:PartnerCode"]}&accessKey={_configuration["MomoAPI:AccessKey"]}&requestId={model.OrderID}&amount={model.ActualPrice}&orderId={model.OrderID}&orderInfo={OrderInfo}&returnUrl={_configuration["MomoAPI:ReturnUrl"]}¬ifyUrl={_configuration["MomoAPI:NotifyUrl"]}&extraData=";

            var Signature = ComputeHmacSha256(rawData, _configuration["MomoAPI:SecretKey"]);
            // Create an object representing the request data
            var requestData = new
            {
                accessKey = _configuration["MomoAPI:AccessKey"],
                partnerCode = _configuration["MomoAPI:PartnerCode"],
                requestType = _configuration["MomoAPI:RequestType"],
                notifyUrl = _configuration["MomoAPI:NotifyUrl"],
                returnUrl = _configuration["MomoAPI:ReturnUrl"],
                orderId = model.OrderID,
                amount = model.ActualPrice.ToString(),
                orderInfo = OrderInfo,
                requestId = model.OrderID,
                extraData = "",
                signature = Signature
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_configuration["MomoAPI:MomoApiUrl"], content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
            // Trích xuất payURL từ đối tượng phản hồi
            return responseObject;
        }
    }
}
