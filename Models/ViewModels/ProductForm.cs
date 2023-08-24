namespace API_Test1.Models.ViewModels
{
    public class ProductForm
    {

        [Required]
        public int ProductTypeID { get; set; }
        [Required]
        public string? NameProduct { get; set; }
        [Required]
        public double? Price { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public int? Discount { get; set; }
        [Required]
        public int? Quantity { get; set; }
        public int? Status { get; set; }
        [Required]
        public IFormFile? AvtarProduct { get; set; }
    }
}
