namespace RMSUdpService.DTOs;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Класс, представляющий заголовок пакета для сообщений real-time протокола управления роботом.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class RobotHeaderDto
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Тип сообщения (команда, состояние, ошибка и т.д.).
    /// </summary>
    public byte MsgType { get; set; }

    /// <summary>
    /// Версия протокола (начинается с 1).
    /// </summary>
    public byte Version { get; set; }

    /// <summary>
    /// Зарезервировано для будущего расширения.
    /// </summary>
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte Reserve { get; set; }

    /// <summary>
    /// Уникальный идентификатор робота в системе.
    /// </summary>
    public Guid RobotId { get; set; }

    /// <summary>
    /// Метка времени в миллисекундах (Unix Epoch).
    /// </summary>
    public ulong Timestamp { get; set; }

    /// <summary>
    /// Конструктор класса Header.
    /// </summary>
    public RobotHeaderDto(byte msgType, byte version, Guid robotId, ulong timestamp)
    {
        MsgType = msgType;
        Version = version;
        Reserve = 0; // Инициализация зарезервированного поля
        RobotId = robotId;
        Timestamp = timestamp;
    }
}

