/// <summary>
/// Server type
/// </summary>
enum ServerType 
{ 
    None = 0,
    /// <summary>
    /// SSDP обнаружение сетевого сервиса, нужен для передачи роботам IP-адреса сервера;
    /// </summary>
    SSDP = 1,
    /// <summary>
    /// Remote Transport Control 
    /// UDP-сокет для real-time управления роботами и получения их состояния;
    /// </summary>
    RTC = 2 , 
    Notify = 3,  
    UDP = 4, 
    ControlComand =5, 
    RMS = 6 
}



