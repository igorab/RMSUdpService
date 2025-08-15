using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace RMSUdpService;

public class Worker : BackgroundService
{    
    private static string? _baseUrl ;       
    private static string? _multicastAddress;
    private static int _multicastPort;
    private static int _port;

    private static ServerType serverType;

    private static readonly HttpClient client = new HttpClient();
    private static System.Timers.Timer _timer;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;

        serverType = ServerType.None;

        // Устанавливаем параметры для SSDP
        _baseUrl = configuration["AppSettings:BaseUrl"]?? "https://localhost:7038/api/RMS/";
        _multicastAddress = configuration["AppSettings:MulticastAddress"]?? "239.255.255.250";
        _multicastPort = configuration.GetValue<int>("AppSettings:MulticastPort");
        _port = configuration.GetValue<int>("AppSettings:Port");

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
