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
        
        public Task CreateCoverAsync(string coverName, string coverPath)
        {
            throw new NotImplementedException();
        }
    }
}