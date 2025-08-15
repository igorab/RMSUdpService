using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace RMSUDPAgent.UDP;

class RobotStateMsg
{
    public void Init()
    {
        // Создаем экземпляр пакета состояния робота
        RobotStatePacket packet = new RobotStatePacket
        {
            header = new Header
            {
                PacketId = 1,
                Length = (uint)Marshal.SizeOf(typeof(RobotState)),
                Checksum = 0 // Здесь можно рассчитать контрольную сумму
            },
            payload = new RobotState
            {
                PositionX = 10.0f,
                PositionY = 20.0f,
                Orientation = 1.57f, // Пример: 90 градусов в радианах
                Speed = 5.0f,
                IsActive = true
            }
        };

        // Сериализуем пакет
        byte[] data = PacketSerializer.Serialize(packet);

        // Отправляем данные через UDP
        SendUdpData(data, "127.0.0.1", 6633);
    }

    static void SendUdpData(byte[] data, string ipAddress, int port)
    {
        using (UdpClient udpClient = new UdpClient())
        {
            try
            {
                // Указываем адрес и порт получателя
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

                // Отправляем данные
                udpClient.Send(data, data.Length, endPoint);
                Console.WriteLine("Данные отправлены на {0}:{1}", ipAddress, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке данных: " + ex.Message);
            }
        }
    }
}
