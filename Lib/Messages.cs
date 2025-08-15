using RMSUdpService.Model;
using RMSUdpService.RTC;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RMSUdpService.Lib;

public static class Messages
{
    public static void SendNotify(string _MulticastAddress, int _MulticastPort)
    {
        IPAddress multicastAddress = IPAddress.Parse(_MulticastAddress);

        // Создаем UDP-сервер
        using (UdpClient server = new UdpClient(_MulticastPort))
        {
            Console.WriteLine("Server started.");

            // Присоединяемся к мультикаст-группе
            server.JoinMulticastGroup(multicastAddress);

            Console.WriteLine("Sending Notify requests...");
            Messages.SendNotifyMessage(multicastAddress, server, _MulticastPort);
        }
    }


    /// <summary>
    /// При запуске сервера он рассылает multicast-сообщение NOTIFY
    /// </summary>
    /// <param name="clientAddress">HOST</param>
    /// <returns></returns>
    static string NotifyMulticastRequest(IPAddress clientAddress)
    {
        Console.WriteLine("Send Notify to: " + clientAddress);

        string request = "NOTIFY * HTTP/1.1\r\n" +
                         "HOST: 239.255.255.250:1900\r\n" +
                         "CACHE-CONTROL: max-age=1800\r\n" +
                         "LOCATION: http://172.16.100.100:8080/path-to-xml-definition\r\n" +
                         "SERVER: Windows_10 UPnP/ 2.0 RMSPrivateServer / 1.0\r\n" +
                         "NTS: ssdp:alive\r\n" +
                         "NT: urn:NTLS:service:RMSPrivateServer:1.0\r\n" +
                         "USN: uuid:01234567-0123-0123-0123-0123456789ab::urn:NTLS:service:RMSPrivateServer:1.0\r\n" +
                         "\r\n";

        Console.WriteLine(request);

        return request;
    }



    public static string ProcessSSDPRequest(string request, IPAddress clientAddress)
    {
        Console.WriteLine("Received request from: " + clientAddress);
        Console.WriteLine(request);

        // Формируем ответ
        string response = "HTTP/1.1 200 OK\r\n" +
                              "CACHE-CONTROL: max-age=1800\r\n" +
                              "LOCATION: http://172.16.100.100:8080/path-to-xml-definition\r\n" +
                              "EXT:\r\n" +
                              "DATE: " + DateTime.Now.ToString("r") + "\r\n" +
                              "SERVER: Windows_10 UPnP/ 2.0 RMSPrivateServer / 1.0\r\n" +
                              "ST: urn: NTLS: service: RMSPrivateServer: 1.0\r\n" +
                              "USN: uuid: 01234567 - 0123 - 0123 - 0123 - 0123456789ab::urn:NTLS: service: RMSPrivateServer: 1.0\r\n" +
                              "\r\n";

        Console.WriteLine(response);

        return response;
    }

    /// <summary>
    /// При запуске сервера он рассылает multicast-сообщение NOTIFY
    /// </summary>
    public static void SendNotifyMessage(IPAddress multicastAddress, UdpClient server, int multicastPort)
    {
        try
        {
            string notify = NotifyMulticastRequest(multicastAddress);
            // Отправляем Notify
            byte[] notifyBytes = Encoding.UTF8.GetBytes(notify);

            IPEndPoint endPoint = new IPEndPoint(multicastAddress, multicastPort);

            server.Send(notifyBytes, notifyBytes.Length, endPoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static void SendControlCommand(string robotAdr = "172.16.10.9")
    {
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


}