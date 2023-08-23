namespace API_Test1.Models.DTOs
{
    public class OrderStatus
    {
        public const int Placed = 1;             // Đã đặt hàng
        public const int Preparing = 2;          // Đang chuẩn bị hàng
        public const int Shipping = 3;           // Đang vận chuyển
        public const int Delivered = 4;          // Đã nhận hàng = Hoàn Thành
        public const int Completed = 5;          // Hoàn thành
        public const int CancelRequest = 6;      // Yêu cầu hủy đơn
        public const int ResumingDelivery = 7;   // Tiếp tục giao = Đang chuẩn bị hàng
        public const int CancelRejected = 8;     // Từ chối hủy đơn
        public const int Cancelled = 9;          // Đã hủy
        public const int ReturnRequest = 10;     // Yêu cầu trả hàng
        public const int ReturnCancelled = 11;   // Hủy trả hàng = Hoàn thành
        public const int ReturnRejected = 12;    // Từ chối trả hàng
        public const int Returned = 13;          // Đã trả hàng
}
}
