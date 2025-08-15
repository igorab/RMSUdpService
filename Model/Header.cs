using RMSUdpService.RTC;
using System.Runtime.InteropServices;

namespace RMSUdpService.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Header
{   
    /// <summary>
    /// // Тип сообщения
    /// </summary>
    public MsgType msgType { get; set; }
    /// <summary>
    /// // Версия протокола
    /// </summary>
    public byte version { get; set; }
    /// <summary>
    /// Зарезервировано
    /// </summary>
    private ushort _reserve { get; set; }

    /// <summary>
    /// Уникальный идентификатор роботаУникальный идентификатор робота
    /// </summary>
    public Guid robotId { get; set; }

    /// <summary>
    ///  Метка времени в миллисекундах
    /// </summary>
    public ulong timestamp { get; set; }

    public Header(MsgType msgType, Guid robotId)
    {        
        this.msgType = msgType;
        version = 1; // Начальная версия
        _reserve = 0; // Зарезервировано
        this.robotId = robotId;
        timestamp = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}

