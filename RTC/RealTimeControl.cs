using RMSUDPAgent.UDP;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace RMSUdpService.RTC;

public enum MsgType : byte
{
    MsgEcho = 0,
    MsgControlCommand = 1,
    MsgStateReport = 100
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ControlCommand
{
    // Пример структуры команды управления
    public float speed; // Линейная скорость
    public float turn;  // Угловая скорость
}

