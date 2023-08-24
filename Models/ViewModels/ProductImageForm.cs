
namespace API_Test1.Models.ViewModels
{
    public class ProductImageForm
    {
        public string? Title { get; set; }
        public IFormFile? ImageProduct { get; set; }
        public int ProductID { get; set; }
        public int? Status { get; set; }
    }
}
