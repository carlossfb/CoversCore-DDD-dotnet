using CoversFunctionApp.src.application.dto;
using CoversFunctionApp.src.domain.entity;
using CoversFunctionApp.src.domain.ports;

namespace CoversFunctionApp.src.application.service
{
    public class CoverServiceImpl : ICoverService
    {
        private readonly IStorageService _storageService;

        public CoverServiceImpl(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public Task<ResponseCoverDTO> CreateCoverAsync(string coverName, string fileUrl)
        {
            // Cria a entidade de domínio Cover
            var cover = Cover.Create(coverName, fileUrl);
            
            // Converte para DTO de resposta
            var response = new ResponseCoverDTO(
                cover.Id,
                cover.FileName,
                cover.FileUrl ?? string.Empty
            );
            
            return Task.FromResult(response);
        }

        public Task<ResponseStorageDTO> GetStorageInfo()
        {
            return _storageService.GetStorageInfo();
        }

        public async Task UploadAsync(string fileName, Stream fileStream)
        {
            await _storageService.UploadAsync(fileName, fileStream);
        }
    }
}