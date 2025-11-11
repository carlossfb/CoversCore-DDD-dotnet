using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using CoversFunctionApp.src.domain.ports;

namespace CoversFunctionApp.src.function;

public class CreateCover
{
    private readonly ILogger<CreateCover> _logger;
    private readonly ICoverService _coverService;
    private readonly IStorageService _storageService;

    public CreateCover(ILogger<CreateCover> logger, ICoverService coverService, IStorageService storageService)
    {
        _logger = logger;
        _coverService = coverService;
        _storageService = storageService;
    }

    [Function("CreateCover")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create-cover")] HttpRequestData req)
    {
        try
        {
            // Verifica se é multipart/form-data
            var contentType = req.Headers.GetValues("Content-Type").FirstOrDefault();
            if (string.IsNullOrEmpty(contentType) || !contentType.Contains("multipart/form-data"))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Content-Type deve ser multipart/form-data");
                return badResponse;
            }

            // Extrai o boundary do Content-Type
            var boundary = GetBoundary(contentType);
            var reader = new MultipartReader(boundary, req.Body);

            string? coverName = null;
            MultipartSection? fileSection = null;

            // Lê cada seção do multipart
            MultipartSection? section;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDisposition = section.GetContentDispositionHeader();
                if (contentDisposition != null)
                {
                    if (contentDisposition.IsFormDisposition())
                    {
                        var name = contentDisposition.Name.Value?.Trim('"');
                        if (name == "coverName")
                        {
                            using var streamReader = new StreamReader(section.Body);
                            coverName = await streamReader.ReadToEndAsync();
                        }
                    }
                    else if (contentDisposition.IsFileDisposition() && fileSection == null)
                    {
                        fileSection = section;
                        // Não quebra aqui - continua lendo outras seções
                    }
                }
            }

            // Validações
            if (string.IsNullOrWhiteSpace(coverName))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("O campo 'coverName' é obrigatório.");
                return badResponse;
            }

            if (fileSection == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Nenhum arquivo foi enviado.");
                return badResponse;
            }

            // Obtém o nome do arquivo
            var fileContentDisposition = fileSection.GetContentDispositionHeader();
            var fileName = fileContentDisposition?.FileName.HasValue == true ? fileContentDisposition.FileName.Value?.Trim('"') : coverName;
            
            // Garante que fileName não seja null
            fileName = fileName ?? "uploaded-file";

            // 1. Faz upload da imagem primeiro
            await _storageService.UploadAsync(fileName, fileSection.Body);
            
            // 2. Obtém a URL do storage para criar o cover
            var storageInfo = await _storageService.GetStorageInfo();
            var fileUrl = $"{storageInfo.StorageUrl.Split('?')[0]}/{fileName}";

            // 3. Cria o cover com a URL do arquivo
            var coverResponse = await _coverService.CreateCoverAsync(coverName, fileUrl);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteStringAsync(JsonSerializer.Serialize(new
            {
                message = "Cover created and upload completed successfully!",
                cover = coverResponse,
            }));
            return ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            var error = req.CreateResponse(HttpStatusCode.InternalServerError);
            await error.WriteStringAsync($"Internal error: {ex.Message}");
            return error;
        }
    }

    private static string GetBoundary(string contentType)
    {
        var elements = contentType.Split(' ', ';');
        var element = elements.FirstOrDefault(entry => entry.Trim().StartsWith("boundary="));
        var boundary = element?.Substring("boundary=".Length).Trim();
        return boundary?.Trim('"') ?? throw new InvalidOperationException("Boundary não encontrado");
    }
}
