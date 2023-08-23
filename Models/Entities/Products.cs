using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace API_Test1.Models.Entities
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }
        //foreign key
        [ForeignKey("ProductTypes")]
        public int ProductTypeID { get; set; }

        [IgnoreDataMember]
        public ProductTypes? ProductTypes { get; set; }

        public string? NameProduct { get; set; }
        public double? Price { get; set; }
        public string? AvatarImageProduct { get; set; }
        public string? Title { get; set; }
        public int? Discount { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
        public int? NumberOfViews { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        //1-n
        [IgnoreDataMember]
        public IEnumerable<ProductReviews>? ProductReviews { get; set; }

        [IgnoreDataMember]
        public IEnumerable<ProductImages>? ProductImages { get; set; }

        [IgnoreDataMember]
        public IEnumerable<OrderDetails>? OrderDetails { get; set; }
    }
}
