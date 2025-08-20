using RMSUDPAgent.Services;
using RMSUdpService.Model;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RMSUdpService.RTC;

public class RTCServer
{
    public HttpClient? client { private get; set; } // = new HttpClient();

    public string? baseUrl { private get; set; } // = "https://localhost:7038/api/RMS/";

    private UdpClient _UdpServer;

    private const int Port = 6633;

    public RTCServer()
    {
        _UdpServer = new UdpClient(Port);
    }

    public RTCServer(UdpClient udpServer)
    {
        _UdpServer = udpServer;
    }

    public void ReceiveData(IPAddress ipaddr )
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(ipaddr, Port);
        byte[] receivedBytes = _UdpServer.Receive(ref remoteEndPoint);
        ProcessReceivedData(receivedBytes, remoteEndPoint);
    }


    public void Start()
    {
        Console.WriteLine("RTC Server started...");
        while (true)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, Port);
            byte[] receivedBytes = _UdpServer.Receive(ref remoteEndPoint);
            ProcessReceivedData(receivedBytes, remoteEndPoint);
        }
    }

    private void ProcessReceivedData(byte[] data, IPEndPoint remoteEndPoint)
    {
        // Десериализация заголовка
        Header header = ByteArrayToStructure<Header>(data);

        // Проверка версии
        if (header.version != 1)
        {
            Console.WriteLine("Unsupported version.");
            return;
        }

        // Обработка в зависимости от типа сообщения
        switch (header.msgType)
        {
            case MsgType.MsgEcho:
                Console.WriteLine("Received Echo request.");
                // Отправить ответ, если необходимо
                break;
            case MsgType.MsgControlCommand:
                // Десериализация полезной нагрузки
                ControlCommand command = ByteArrayToStructure<ControlCommand>( data[Marshal.SizeOf<Header>()..] );
                Console.WriteLine($"Received Control Command: Speed={command.speed}, Turn={command.turn}");
                break;
            case MsgType.MsgStateReport:
                int from = Marshal.SizeOf<StateReport>();
                byte[] bytes = data[..];
                StateReport stateReport = ByteArrayToStructure<StateReport>(bytes);

                RMSClient.RunStateReport(stateReport, client, baseUrl);

                Console.WriteLine("Received State Report.");
                break;
            default:
                Console.WriteLine("Unknown message type.");
                break;
        }
    }

    private T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T structure = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
        handle.Free();
        return structure;
    }
}
