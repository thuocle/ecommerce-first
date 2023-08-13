using System.ComponentModel.DataAnnotations;

namespace API_Test1
{
    public class Users
    {
        [Key]
        public string? UserID { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; }
    }
}
