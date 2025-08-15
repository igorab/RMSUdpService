using System;
using System.Runtime.InteropServices;

namespace RMSUdpService.Model;

// Определение заголовка пакета
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ControlHeader
{
    // Поля заголовка (примерные поля, могут быть изменены в зависимости от требований)
    public uint PacketId;      // Идентификатор пакета
    public uint Length;        // Длина пакета
    public uint Checksum;      // Контрольная сумма
}

// Перечисление команд управления
public enum ControlCommand : byte
{
    NoCommand = 0,          // Пустая команда (отсутствие действия)
    TaskPending = 1,        // Уведомление о наличии новой задачи для выполнения
    Synchronize = 2,        // Вывод робота из режима ожидания синхронизации
    PauseOperation = 3,     // Приостановка выполнения текущей подзадачи
    ResumeOperation = 4      // Возобновление выполнения подзадачи
}

// Структура управляющей команды
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ControlCommandPacket
{
    public ControlHeader header;          // Заголовок пакета
    public ControlCommand command; // Команда управления
}

