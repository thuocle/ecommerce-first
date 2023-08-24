namespace API_Test1.Models.ViewModels
{
    public class FilterProductForm
    {
        public int? minPrice { get; set; }
        public int? maxPrice { get; set; }
        public int? categoryID { get; set; }
        public bool? hotProduct { get; set; }
        public bool? desc { get; set; }
        public bool? asc { get; set; }

    }
}
