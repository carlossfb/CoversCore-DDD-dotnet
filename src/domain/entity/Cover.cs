namespace CoversFunctionApp.src.domain.entity
{
    public class Cover
    {
      public Guid Id { get; private set; }
      public string FileName { get; private set; }
      public string? FileUrl { get; private set; }
      public DateTime UploadedAt { get; private set; }
    
      private Cover(string fileName, string? fileUrl)
      {
        Id = Guid.NewGuid();
        FileName = fileName;
        FileUrl = fileUrl;
        UploadedAt = DateTime.UtcNow;
      }

        public static Cover Create(string fileName, string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("FileName cannot be null or empty");

            return new Cover(fileName, fileUrl);
        }
    }
}