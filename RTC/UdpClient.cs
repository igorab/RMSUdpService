using RMSUdpService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RMSUdpService.RTC
{
    public class UDPClient
    {
        public ILogger<Worker>? Logger { get; internal set; }

        private UdpClient udpClient;
        //TODO refactoring
        private const int Port = 6633;
        private IPEndPoint remoteEndPoint;

        public UdpClient GetClient => udpClient;

        public UDPClient(IPAddress ipAddress)
        {
            udpClient = new UdpClient();
            remoteEndPoint = new IPEndPoint(ipAddress, Port);
        }

        /// <summary>
        /// Echo message
        /// </summary>
        public void SendEcho()
        {
            Header header = new Header(MsgType.MsgEcho, Guid.NewGuid()); // Пример robotID

            // Сериализация заголовка
            byte[] headerBytes = StructureToByteArray(header);

            byte[] payload = new byte[0]; // Пустая полезная нагрузка для Echo-запроса

            byte[] packet = new byte[headerBytes.Length + payload.Length];

            Buffer.BlockCopy(headerBytes, 0, packet, 0, headerBytes.Length);
            
            udpClient.Send(packet, packet.Length, remoteEndPoint);

            Logger?.LogInformation("Echo request sent.");
        }

        /// <summary>
        ///  Control command to robot
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="turn"></param>
        public void SendControlCommand(float speed, float turn)
        {
            Header header = new Header(MsgType.MsgControlCommand, Guid.NewGuid()); // Пример robotId
            ControlCommand command = new ControlCommand { speed = speed, turn = turn };

            byte[] headerBytes = StructureToByteArray(header);
            byte[] commandBytes = StructureToByteArray(command);
            byte[] packet = new byte[headerBytes.Length + commandBytes.Length];

            Buffer.BlockCopy(headerBytes, 0, packet, 0, headerBytes.Length);
            Buffer.BlockCopy(commandBytes, 0, packet, headerBytes.Length, commandBytes.Length);

            udpClient.Send(packet, packet.Length, remoteEndPoint);
            Logger?.LogInformation($"Control command sent: Speed={speed}, Turn={turn}");
        }

        public void SendStateReport(StateReport stateReport)
        {
            // Предполагается, что stateReport уже содержит заголовок
            byte[] headerBytes = StructureToByteArray(stateReport.header);
            byte[] stateReportBytes = StructureToByteArray(stateReport);
            byte[] packet = new byte[headerBytes.Length + stateReportBytes.Length];

            Buffer.BlockCopy(headerBytes, 0, packet, 0, headerBytes.Length);
            Buffer.BlockCopy(stateReportBytes, 0, packet, headerBytes.Length, stateReportBytes.Length);

            udpClient.Send(packet, packet.Length, remoteEndPoint);
            Logger?.LogInformation("State report sent.");
        }

        private byte[] StructureToByteArray<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf(structure);
            byte[] bytes = new byte[size];
            nint ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, ptr, true);
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return bytes;
        }


    }
}
