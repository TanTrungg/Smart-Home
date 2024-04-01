namespace ISHE_Data.Models.Views
{
    public class FeedbackDevicePackageViewModel
    {
        public Guid Id { get; set; }
        //public Guid CustomerId { get; set; }
        //public Guid DevicePackageId { get; set; }
        public int Rating { get; set; }
        public string? Content { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual PartialCustomerViewModel Customer { get; set; } = null!;
        //public virtual DevicePackage DevicePackage { get; set; } = null!;
    }

    
}
