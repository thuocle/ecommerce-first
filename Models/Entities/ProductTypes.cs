using Newtonsoft.Json;

namespace API_Test1.Models.Entities
{
    public class ProductTypes
    {
        [Key]
        public int ProductTypeID { get; set; }
        public string? NameProductType { get; set; }
        public string? ImageTypeProduct { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        //1-n
        [JsonIgnore]
        public IEnumerable<Products>? Products { get; set; }
    }
}
