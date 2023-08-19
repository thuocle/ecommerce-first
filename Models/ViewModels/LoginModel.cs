﻿namespace API_Test1.Models.ViewModels
{
    public class LoginModel
    {
        [Required, MinLength(6)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
