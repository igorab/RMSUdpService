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
    private const string AppSettings = "AppSettings:";
    private const int DelayMillisec = 1000;
    
    private static string? _BaseUrl ;       
    private static string? _MulticastAddress;
    private static int _MulticastPort;
    private static int _Port;
    private static string? _RobotAddress;
    
    private static readonly HttpClient _client = new HttpClient();
    
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        
        // Устанавливаем параметры для SSDP
        _BaseUrl = configuration[AppSettings + "BaseUrl"];
        _MulticastAddress = configuration[AppSettings + "MulticastAddress"];
        _MulticastPort = configuration.GetValue<int>(AppSettings + "MulticastPort");
        _Port = configuration.GetValue<int>(AppSettings + "Port");
        _RobotAddress = configuration.GetValue<string>(AppSettings + "RobotAddress");
        Messages.RobotAddress = _RobotAddress;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ServerType serverTypes = ServerType.SSDP | ServerType.RTC ;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(DelayMillisec, stoppingToken);

            // Создаем новый поток для запуска сервера
            Thread serverThread = Thread.CurrentThread; 

            Srv.MulticastAddress = _MulticastAddress; 
            Srv.MulticastPort    = _MulticastPort;
            Srv.BaseUrl          = _BaseUrl;
            Srv.Logger           = _logger;

            if (serverTypes.HasFlag( ServerType.SSDP))
            {
                serverThread = new Thread(new ThreadStart(Srv.StartSSDPServer));
                serverThread.Start();
            }

            if (serverTypes.HasFlag(ServerType.RTC))
            {
                SrvRtc.Logger = _logger;

                var commandParams = new CommandParameters
                {
                    RobotAddress = "",
                    Client = _client,
                    BaseUrl = _BaseUrl,
                    Port    = _Port
                };

                serverThread = new Thread(new ParameterizedThreadStart(SrvRtc.StartRTCServer));
                serverThread.Start(commandParams);
            }

            if (serverTypes.HasFlag( ServerType.Notify))
            {
                serverThread = new Thread(new ThreadStart(Srv.StartSSDPNotification));
                serverThread.Start();
            }

            if (serverTypes.HasFlag(ServerType.ControlComand))
            {
                serverThread = new Thread(new ParameterizedThreadStart(Srv.SendControlCommand));
                serverThread.Start(_RobotAddress);
            }

            if (serverTypes.HasFlag(ServerType.RMS))
            {
                serverThread = new Thread(new ThreadStart(Srv.StartRMSServer));
                serverThread.Start();
            }
                                                   
            // Ожидаем завершения работы сервера
            serverThread.Join();
        }
    }
}
