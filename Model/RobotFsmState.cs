using System;

namespace RMSUdpService.Model;

public enum RobotFsmState : byte
{
    Idle = 0,
    Initializing = 1,
    Ready = 2,
    ManualControl = 3,
    Navigation = 4,
    LinearMotion = 5,
    Rotation = 6,
    ArcMotion = 7,
    Adjustment = 8,
    Loading = 9,
    Unloading = 10,
    LoadingInPlace = 11,
    UnloadingInPlace = 12,
    Parking = 13,
    Charging = 14,
    ChargeFinished = 15,
    WaitSync = 16,
    Paused = 17,
    Alarm = 18
}
