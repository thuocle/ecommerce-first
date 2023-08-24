using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API_Test1.Models.Entities
{
    public class Payments
    {
        [Key]
        public int PaymentID { get; set; }
        public string? PaymentMethod { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        //1-n
        [JsonIgnore]
        public IEnumerable<Orders>? Orders { get; set; }
    }
}
