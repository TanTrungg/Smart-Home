using Microsoft.AspNetCore.Http;

namespace ISHE_Data.Models.Requests.Put
{
    public class UpdateManufacturerModel
    {
        public string? Name { get; set; }
        public string? Origin { get; set; }
        public string? Description { get; set; } 
        public IFormFile? Image { get; set; }
    }
}
