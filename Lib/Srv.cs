using Microsoft.Extensions.Primitives;
using RMSUDPAgent.Services;
using RMSUdpService.Model;
using RMSUdpService.RTC;
using RMSUdpService.SSDP;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RMSUdpService.Lib
{
    public static class Srv
    {
        /// <summary>
        /// multicast port
        /// </summary>
        public static int MulticastPort { private get; set; }
        /// <summary>
        /// multicast address
        /// </summary>
        public static string? MulticastAddress { private get; set; }
        /// <summary>
        /// base url
        /// </summary>
        public static string? BaseUrl { private get; set; }
        public static ILogger<Worker> Logger { get; internal set; }

        private static System.Timers.Timer? _timer;

        /// <summary>
        /// При включении каждого робота его SSDP-клиент отправляет multicast запрос «M-SEARCH», 
        /// на который отвечает служба SSDP на RMS и передаёт IP-адрес сервера устройству, от которого пришёл запрос. 
        /// Если сервер запускается после робота, то он инициирует поиск сообщением «NOTIFY», 
        /// при приёме которого роботы должны повторить поиск.
        /// </summary>
        public static void StartSSDPServer()
        {
            IPAddress multicastAddress = IPAddress.Parse(MulticastAddress??"");

            // Создаем UDP-сервер
            using (UdpClient server = new UdpClient(MulticastPort))
            {
                Logger.LogInformation("Server started.");
                //Console.WriteLine("Server started.");

                // Присоединяемся к мультикаст-группе
                server.JoinMulticastGroup(multicastAddress);

                Console.WriteLine("Sending Notify requests...");

                // Отправляем Notify
                Messages.SendNotifyMessage(multicastAddress, server, MulticastPort);

                Logger.LogInformation("Listening for SSDP requests...");
                //Console.WriteLine("Listening for SSDP requests...");

                while (true)
                {
                    // Получаем SSDP-запрос
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] requestBytes = server.Receive(ref remoteEndPoint);
                    string request = Encoding.UTF8.GetString(requestBytes);

                    // Парсим запрос
                    SSDPRequest parsedRequest = SSDPServer.ParseSSDPRequest(request);

                    // Логируем детали запроса
                    SSDPServer.LogRequestDetails(parsedRequest, remoteEndPoint);

                    // Ловим «M-SEARCH»
                    bool isOk = SSDPServer.ValidateRequest(parsedRequest);

                    if (isOk)
                    {
                        SSDPServer.GenerateResponse(parsedRequest);

                        // Обрабатываем SSDP-запрос
                        string response = Messages.ProcessSSDPRequest(request, remoteEndPoint.Address);

                        // Отправляем ответ
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        server.Send(responseBytes, responseBytes.Length, remoteEndPoint);                        
                    }
                }
            }
        }

        public static void SendControlCommand(object paramRobotAdr)
        {            
            string? robotAdr = paramRobotAdr as string;

            IPAddress robotAddress = IPAddress.Parse(robotAdr);

            int commandType = 0;

            UDPClient udpClient = new UDPClient(robotAddress);

            if (commandType == 0)
            {
                udpClient.SendEcho();

                RTCServer srv = new RTCServer(udpClient.GetClient);
                srv.ReceiveData(IPAddress.Any);

            }
            else if (commandType == 1)
            {
                udpClient.SendControlCommand(10, 10);
            }
            else if (commandType == 2)
            {
                StateReport stateReport = new StateReport() { };

                udpClient.SendStateReport(stateReport);
            }
        }

        public async static void StartRMSServer()
        {
            await RMSClient.RunAsync();
        }

        
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Lib.Messages.SendNotify(MulticastAddress, MulticastPort);
        }

        /// <summary>
        /// отправлять notify через интервал времени
        /// </summary>
        public static void StartSSDPNotification()
        {
            _timer = new System.Timers.Timer(30000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true; // Повторять событие
            _timer.Enabled = true; // Запускаем таймер


            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadLine();

            // Останавливаем таймер перед выходом
            _timer.Stop();
            _timer.Dispose();

        }

    }
}
