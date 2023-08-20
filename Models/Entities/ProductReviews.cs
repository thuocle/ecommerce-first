
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Test1.Models.Entities
{
    public class ProductReviews
    {
        [Key]
        public int ProductReviewID { get; set; }
        //foreign key
        [ForeignKey("Products")]
        public int ProductID { get; set; }
        [JsonIgnore]
        public Products? Products { get; set; }
        [ForeignKey("ApplicationUser")]
        public int Id { get; set; }
        [JsonIgnore]
        public ApplicationUser? ApplicationUser { get; set; }
        public string? ContentRated { get; set; }
        public int PointEvaluation { get; set; }
        public string? ContentSeen { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
