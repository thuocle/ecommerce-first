
namespace API_Test1.Models.Entities
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
        [ForeignKey("ApplicationUser")]
        public string? AccountID { get; set; }
        [JsonIgnore]
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
