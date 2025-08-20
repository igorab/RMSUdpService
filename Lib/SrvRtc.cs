using RMSUdpService.RTC;

namespace RMSUdpService.Lib
{
    public static class SrvRtc
    {
        public static void StartRTCServer(object? parameters)
        {
            CommandParameters? commandParams = parameters as CommandParameters;
            if (commandParams == null)
            {
                // log
                return;
            }

            RTCServer rtcServer = new RTCServer();

            rtcServer.client = commandParams.Client;
            rtcServer.baseUrl = commandParams.BaseUrl;

            rtcServer.Start();
        }
    }
}
