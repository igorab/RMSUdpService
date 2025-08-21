using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RMSUdpService.Lib;

namespace RMSUdpService;

public class Worker : BackgroundService
{    
    private static string? _baseUrl ;       
    private static string? _multicastAddress;
    private static int _multicastPort;
    private static int _port;

    private static ServerType serverType;

    private static readonly HttpClient _client = new HttpClient();
    
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;

        serverType = ServerType.SSDP;

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

            // Создаем новый поток для запуска сервера
            Thread serverThread;

            Srv.MulticastAddress = _multicastAddress; 
            Srv.MulticastPort    = _multicastPort;
            Srv.BaseUrl          = _baseUrl;
            Srv.Logger           = _logger;

            if (serverType == ServerType.SSDP)
            {
                serverThread = new Thread(new ThreadStart(Srv.StartSSDPServer));
                serverThread.Start();
            }
            else if (serverType == ServerType.RTC)
            {
                SrvRtc.Logger = _logger;

                var commandParams = new CommandParameters
                {
                    RobotAddress = "",
                    Client = _client,
                    BaseUrl = _baseUrl                    
                };

                serverThread = new Thread(new ParameterizedThreadStart(SrvRtc.StartRTCServer));
                serverThread.Start(commandParams);
            }
            else if (serverType == ServerType.Notify)
            {
                serverThread = new Thread(new ThreadStart(Srv.StartSSDPNotification));
                serverThread.Start();
            }
            else if (serverType == ServerType.ControlComand)
            {
                serverThread = new Thread(new ParameterizedThreadStart(Srv.SendControlCommand));
                serverThread.Start("172.16.10.9");
            }
            else if (serverType == ServerType.RMS)
            {
                serverThread = new Thread(new ThreadStart(Srv.StartRMSServer));
                serverThread.Start();
            }
            else
            {
                serverThread = Thread.CurrentThread;
            }

            // Ожидаем завершения работы сервера
            serverThread.Join();
        }
    }
}
