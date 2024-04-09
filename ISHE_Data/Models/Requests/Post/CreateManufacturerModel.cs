using Microsoft.AspNetCore.Http;

namespace ISHE_Data.Models.Requests.Post
{
    public class CreateManufacturerModel
    {
        public string Name { get; set; } = null!;
        public string? Origin { get; set; } 
        public string? Description { get; set; }
        public IFormFile Image { get; set; } = null!;

    }
}
