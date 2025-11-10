using System;
using System.Collections.Generic;
using System.Linq;
using CoversFunctionApp.src.application.dto;
using CoversFunctionApp.src.domain.ports;

namespace CoversFunctionApp.src.infraestructure
{
    public class AzureBlobStorageImpl : IStorageService
    {
        public Task<RequestStorageDTO> GetStorageInfo()
        {
            throw new NotImplementedException();
        }

        public Task UploadAsync(string localFilePath)
        {
            throw new NotImplementedException();
        }
    }
}