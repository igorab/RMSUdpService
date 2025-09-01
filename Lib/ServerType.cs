/// <summary>
/// Server type
/// </summary>
[Flags]
enum ServerType 
{
    /// <summary>
    /// wait 
    /// </summary>
    None = 0,
    /// <summary>
    /// SSDP обнаружение сетевого сервиса, нужен для передачи роботам IP-адреса сервера;
    /// </summary>
    SSDP = 1 << 0,
    /// <summary>
    /// Remote Transport Control 
    /// UDP-сокет для real-time управления роботами и получения их состояния;
    /// </summary>
    RTC = 1 << 1, 
    /// <summary>
    /// команда Notify каждый заданный период времени
    /// </summary>
    Notify = 1 << 2,  
    UDP = 1 << 3,
    /// <summary>
    /// Robot control command - комана роботу от RMS
    /// </summary>
    ControlComand = 1 << 4,
    
    RMS = 1 << 5 
}



