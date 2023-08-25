namespace API_Test1.Services.PaymentServices.MOMO.Model
{
    public class MomoExecuteResponseModel
    {
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string OrderInfo { get; set; }
        public int ErrorCode { get; set; }
        public string LocalMessage { get; set; }
    }
}
