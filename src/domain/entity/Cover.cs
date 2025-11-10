namespace CoversFunctionApp.src.domain.entity
{
    public class Cover
    {
      public Guid Id { get; private set; }
      public string FileName { get; private set; }
      public long SizeInBytes { get; private set; }
      public string? StorageUrl { get; private set; }
      public string? StoragePath { get; private set; }  
      public DateTime UploadedAt { get; private set; }
    
      private Cover(string fileName)
      {
        Id = Guid.NewGuid();
        FileName = fileName;
        UploadedAt = DateTime.UtcNow;
      }

        public void SetStorageInfo(string storageUrl, string storagePath)
        {
            if (string.IsNullOrWhiteSpace(storageUrl))
                throw new ArgumentException("StorageUrl cannot be null or empty");

            if (string.IsNullOrWhiteSpace(storagePath))
                throw new ArgumentException("StoragePath cannot be null or empty");

            StoragePath = storagePath;
            StorageUrl = storageUrl;
        }

        public static Cover Create(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("FileName cannot be null or empty");

            return new Cover(fileName);
        }

        public void SetFileSize(long sizeInBytes)
        {
            if (sizeInBytes <= 0)
                throw new ArgumentException("SizeInBytes must be greater than zero", nameof(sizeInBytes));

            SizeInBytes = sizeInBytes;
        }
    }
}