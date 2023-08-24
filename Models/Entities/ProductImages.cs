using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Test1.Models.Entities
{
    public class ProductImages
    {
        [Key]
        public int ProductImageID { get; set; }
        public string? Title { get; set; }
        public string? ImageProduct { get; set; }
        //foreign key
        [ForeignKey("Products")]
        public int ProductID { get; set; }
        [JsonIgnore]
        public Products? Products { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
