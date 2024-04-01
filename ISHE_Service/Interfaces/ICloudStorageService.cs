namespace ISHE_Service.Interfaces
{
    public interface ICloudStorageService
    {
        Task<string> Upload(Guid id, string contentType, Stream stream);
        Task<string> Delete(Guid id);
        Task<string> UploadContract(string id, string contentType, Stream stream);
        Task<string> DeleteContract(string id);
    }
}
