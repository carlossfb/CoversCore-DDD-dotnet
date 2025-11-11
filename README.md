# CoversFunctionApp

Azure Functions application for managing album covers with Azure Blob Storage integration.

## 🚀 Project Setup

This project was created using Azure Functions Core Tools:

```bash
func init --worker-runtime dotnet-isolated
func new --name CreateCover --template "Http trigger" 
func new --name GetStorageInfo --template "Http trigger"
```

## 🏗️ Architecture

The project follows **Hexagonal Architecture** (Ports and Adapters) with clean separation of concerns:

```
src/
├── application/
│   ├── dto/           # Data Transfer Objects
│   └── service/       # Application Services
├── domain/
│   ├── entity/        # Domain Entities
│   ├── exception/     # Domain Exceptions
│   └── ports/         # Interfaces (Ports)
├── function/          # Azure Functions (Adapters)
└── infraestructure/   # External Adapters
    └── util/          # Utility Classes
```

### Layers Description

- **Domain**: Core business logic and entities
- **Application**: Use cases and application services
- **Infrastructure**: External integrations (Azure Blob Storage)
- **Function**: HTTP endpoints and Azure Functions

## 📋 Features

### 1. GetStorageInfo
- **Endpoint**: `GET /api/storage-info`
- **Purpose**: Generates Azure Blob Storage SAS (Shared Access Signature) tokens
- **Use Case**: Allows client applications to upload files directly to Azure Storage
- **Response**: Returns a pre-signed URL with upload permissions

### 2. CreateCover
- **Endpoint**: `POST /api/create-cover`
- **Purpose**: Creates a new album cover and uploads the image file
- **Input**: Form-data with `coverName` (text) and `file` (image)
- **Process**: 
  1. Uploads image to Azure Blob Storage
  2. Creates Cover entity with metadata
  3. Returns cover information with storage URL

## 🔧 Configuration

Set the following environment variables in `local.settings.json`:

```json
{
  "Values": {
    "AzureWebJobsStorage": "your_azure_storage_connection_string",
    "BlobContainerName": "your_container_name"
  }
}
```

## 🚀 Running the Application

1. **Install dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Start the function app**:
   ```bash
   func start
   ```

## 📡 API Usage

### Get Storage Info
```bash
GET http://localhost:7071/api/storage-info
```

**Response**:
```json
{
  "storageUrl": "https://account.blob.core.windows.net/container?sv=2022-11-02&ss=b&srt=c&sp=cw&se=2025-11-10T18:51:55Z&st=2025-11-10T16:51:55Z&spr=https&sig=..."
}
```

### Create Cover
```bash
POST http://localhost:7071/api/create-cover
Content-Type: multipart/form-data

Form fields:
- coverName: "My Album Cover"
- file: [image file]
```

**Response**:
```json
{
  "message": "Cover created and upload completed successfully!",
  "cover": {
    "id": "guid",
    "fileName": "My Album Cover",
    "storageUrl": "https://account.blob.core.windows.net/container/image.jpg"
  },
  "fileName": "image.jpg"
}
```

## 🛠️ Technology Stack

- **.NET 8** - Runtime
- **Azure Functions** - Serverless compute
- **Azure Blob Storage** - File storage
- **C#** - Programming language
- **Dependency Injection** - IoC container

## 📁 Key Components

- **Cover**: Domain entity representing an album cover
- **IStorageService**: Port for storage operations
- **AzureBlobStorageImpl**: Azure Blob Storage adapter
- **ICoverService**: Port for cover management
- **CoverServiceImpl**: Cover business logic implementation

## 🔐 Security

- SAS tokens are generated with limited permissions (Create, Write)
- Tokens have 2-hour expiration time
- File uploads are validated and processed securely