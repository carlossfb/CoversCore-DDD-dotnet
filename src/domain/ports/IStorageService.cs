using CoversFunctionApp.src.application.dto;

namespace CoversFunctionApp.src.domain.ports
{
    public interface IStorageService
    {
        Task<RequestStorageDTO> GetStorageInfo();
        Task UploadAsync(string localFilePath);
    }
}