﻿namespace API_Test1.Models.ViewModels
{
    public class OrderInfo
    {
        [Required]
        public string? FullName { get; set; } = string.Empty;
        [Required]
        public string? Email { get; set; } = string.Empty;
        [Required]
        public string? Phone { get; set; } = string.Empty;
        [Required]
        public string? Address { get; set; } = string.Empty;
        [Required]
        public int? PaymentID { get; set; }
    }
}
