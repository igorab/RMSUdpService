using RMSUdpService.RTC;

namespace RMSUdpService.Lib
{
    public static class SrvRtc
    {
        public static ILogger<Worker>? Logger { private get; set; }

        public static void StartRTCServer(object? parameters)
        {
            CommandParameters? commandParams = parameters as CommandParameters;
            if (commandParams == null)
            {
                Logger?.LogInformation($@"{nameof(StartRTCServer)}:  wrong params");
                return;
            }

            RTCServer rtcServer = new RTCServer();

            rtcServer.HttpClient = commandParams.Client;
            rtcServer.BaseUrl = commandParams.BaseUrl;
            rtcServer.Port = commandParams.Port;
            rtcServer.Logger = Logger;

            rtcServer.Start();
        }
    }
}
