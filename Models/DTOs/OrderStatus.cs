namespace API_Test1.Models.DTOs
{
    public enum OrderStatus
    {
        Placed,             // Đã đặt hàng
        Preparing,          // Đang chuẩn bị hàng
        Shipping,           // Đang vận chuyển
        Delivered,          // Đã nhận hàng
        Completed,          // Hoàn thành
        CancelRequest,      // Yêu cầu hủy đơn
        CancelRejected,     // Từ chối hủy đơn
        Cancelled,          // Đã hủy
        ReturnRequest,      // Yêu cầu trả hàng
        ReturnRejected,     // Từ chối trả hàng
        Returned,           // Đã trả hàng
        ResumingDelivery,   // Tiếp tục giao
        ReturnCancelled,    // Hủy trả hàng
        ContinueShipping,   // Tiếp tục vận chuyển
    }
}
