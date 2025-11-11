using CoversFunctionApp.src.application.dto;

namespace CoversFunctionApp.src.domain.ports
{
    public interface ICoverService
    {
        Task<ResponseCoverDTO> CreateCoverAsync(string coverName, string fileUrl);
    }
}