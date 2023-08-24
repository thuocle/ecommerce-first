using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace API_Test1.Models.Entities
{
    public class OrderStatuses
    {
        [Key]
        public int OrderStatusID { get; set; }
        public string? StatusName { get; set; }
        [JsonIgnore]
        public IEnumerable<Orders>? Orders { get; set; }
    }
}
