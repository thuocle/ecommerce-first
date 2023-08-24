namespace API_Test1.Models.ViewModels
{
    public class ProductTypeForm
    {
        [Required]
        public string? NameProductType { get; set; }
        [Required]
        public IFormFile? ImageTypeProduct { get; set; }
    }
}
