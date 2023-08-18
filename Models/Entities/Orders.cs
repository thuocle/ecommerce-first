using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Test1.Models.Entities

{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }

        //foreign key
        [ForeignKey("Payments")]
        public int PaymentID { get; set; }
        [JsonIgnore]
        public Payments? Payments { get; set; }
        [ForeignKey("ApplicationUser")]
        public int Id { get; set; }
        [JsonIgnore]
        public ApplicationUser? ApplicationUser { get; set; }
        public double OriginalPrice { get; set; }
        public double ActualPrice { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? Address { get; set; }
        //foreign key
        [ForeignKey("OrderStatuses")]
        public int OrderStatusID { get; set; }
        [JsonIgnore]
        public OrderStatuses? OrderStatuses { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        //1-n
        [JsonIgnore]
        public IEnumerable<OrderDetails>? OrderDetails { get; set; }

    }
}
