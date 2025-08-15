using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RMSUdpService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

// Настройка логирования
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();

    logging.AddConsole();

    logging.AddDebug();
    //файловый логгер пакет Serilog
    logging.AddFile("Logs/app.log");
});


var host = builder.Build();

host.Run();
