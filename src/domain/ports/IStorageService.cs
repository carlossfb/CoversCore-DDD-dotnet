using CoversFunctionApp.src.application.dto;

namespace CoversFunctionApp.src.domain.ports
{
    public interface IStorageService
    {
        Task<ResponseStorageDTO> GetStorageInfo();
        Task UploadAsync(string fileName, Stream fileStream);
    }
}