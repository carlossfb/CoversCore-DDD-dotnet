namespace CoversFunctionApp.src.domain.ports
{
    public interface ICoverService
    {
        Task CreateCoverAsync(string coverName, string coverPath);
    }
}