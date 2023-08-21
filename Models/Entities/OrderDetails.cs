using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Test1.Models.Entities
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailID { get; set; }
        //foreign key
        [ForeignKey("Orders")]
        public string? OrderID { get; set; }
        [JsonIgnore]
        public Orders? Orders { get; set; }
        [ForeignKey("Products")]
        public int ProductID { get; set; }
        [JsonIgnore]
        public Products? Products { get; set; }

        public double PriceTotal { get; set; }
        public int Quantity { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
