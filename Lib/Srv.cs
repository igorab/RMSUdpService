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
        public static int _MulticastPort { get; set; }

        public static string? _MulticastAddress {get; set;}
        public static HttpClient client { get; private set; }
        public static string baseUrl { get; private set; }

        private static System.Timers.Timer _timer;


        /// <summary>
        /// При включении каждого робота его SSDP-клиент отправляет multicast запрос «M-SEARCH», 
        /// на который отвечает служба SSDP на RMS и передаёт IP-адрес сервера устройству, от которого пришёл запрос. 
        /// Если сервер запускается после робота, то он инициирует поиск сообщением «NOTIFY», 
        /// при приёме которого роботы должны повторить поиск.
        /// </summary>
        public static void StartSSDPServer()
        {
            IPAddress multicastAddress = IPAddress.Parse(_MulticastAddress);

            // Создаем UDP-сервер
            using (UdpClient server = new UdpClient(_MulticastPort))
            {
                Console.WriteLine("Server started.");

                // Присоединяемся к мультикаст-группе
                server.JoinMulticastGroup(multicastAddress);

                Console.WriteLine("Sending Notify requests...");

                // Отправляем Notify
                Messages.SendNotifyMessage(multicastAddress, server, _MulticastPort);

                Console.WriteLine("Listening for SSDP requests...");

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

                        //UDPClient uDPClient = new UDPClient(remoteEndPoint.ToString());
                        //uDPClient.SendEcho();
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

        internal static void StartRMSServer()
        {
            RMSClient.RunAsync();
        }

        public static void StartRTCServer(object parameters)
        {
            RTCServer rtcServer = new RTCServer();

            rtcServer.client = client;
            rtcServer.baseUrl = baseUrl;

            rtcServer.Start();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Lib.Messages.SendNotify(_MulticastAddress, _MulticastPort);
        }

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
