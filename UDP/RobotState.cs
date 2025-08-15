using System.Runtime.InteropServices;
namespace RMSUDPAgent.UDP;

// Определение заголовка пакета
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Header
{
    public uint PacketId;      // Идентификатор пакета
    public uint Length;        // Длина полезных данных
    public uint Checksum;      // Контрольная сумма
}

// Структура состояния робота (Payload)
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RobotState
{
    public float PositionX;    // Позиция по оси X
    public float PositionY;    // Позиция по оси Y
    public float Orientation;   // Ориентация робота
    public float Speed;         // Скорость робота
    public bool IsActive;       // Статус активности робота
}

// Структура пакета, содержащая заголовок и полезные данные
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RobotStatePacket
{
    public Header header;          // Заголовок пакета
    public RobotState payload;     // Полезные данные (состояние робота)
}