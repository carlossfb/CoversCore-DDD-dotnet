using CoversFunctionApp.src.application.service;
using CoversFunctionApp.src.domain.ports;
using CoversFunctionApp.src.infraestructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddSingleton<ICoverService, CoverServiceImpl>()
    .AddSingleton<IStorageService, AzureBlobStorageImpl>();

builder.Build().Run();
