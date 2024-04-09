namespace ISHE_Data.Models.Views
{
    public class ManufacturerViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        
        public string? Origin { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
