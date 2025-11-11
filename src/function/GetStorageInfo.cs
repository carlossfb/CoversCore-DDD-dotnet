using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoversFunctionApp.src.domain.ports;

namespace CoversFunctionApp.src.function;

public class GetStorageInfo
{
    private readonly ILogger<GetStorageInfo> _logger;
    private readonly IStorageService _storageService;

    public GetStorageInfo(ILogger<GetStorageInfo> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [Function("GetStorageInfo")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "storage-info")] HttpRequest req)
    {
        _logger.LogInformation("GetStorageInfo HTTP trigger function processed a request.");
        var storageInfo = await _storageService.GetStorageInfo();

       return new OkObjectResult(storageInfo);
    }
}
