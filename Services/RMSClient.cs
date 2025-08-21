using RMSUdpService;
using RMSUdpService.DTOs;
using RMSUdpService.Model;
using RMSUdpService.RTC;
using System.Buffers.Text;

namespace RMSUDPAgent.Services;

internal class RMSClient
{
    public static ILogger<Worker>? Logger { get; internal set; }
    private static HttpClient? client { get; set; } = new HttpClient();
    private static string? baseUrl { get; set; } = "https://localhost:7038/api/RMS/";

    private static Pose _Pose  = new Pose() { x = 0, y = 0, heading = 0 };
    private static Header _Header = new Header { msgType = MsgType.MsgStateReport, version = 1, robotId = Guid.NewGuid(), timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
    private static StateReport _StateReport = new StateReport() {
        velocity = 1.5f,
        angularVelocity = 0.5f,
        battery = 80,
        FSMstate = RobotFsmState.Ready,
        alarmStatus = 0,
        pallet = 1,
        subtaskIdx = 0,
        subtaskNum = 5,
        progress = 50.0f
    };

    public static async void RunStateReport(StateReport stateReport, HttpClient? httpClient, string? url)
    {
        _StateReport = stateReport;
        _Header = stateReport.header;
        _Pose   = stateReport.targetPose;

        client = httpClient;
        baseUrl = url;

        await RunAsync();
    }

    /// <summary>
    ///  Go!
    /// </summary>
    /// <returns></returns>
    public static async Task RunAsync()
    {
        if (client == null) Logger?.LogInformation($@"{nameof(RunAsync)}: wrong params");
        

        var robotApiClient = new RobotApiClient(client, baseUrl??"");

        // Создание заголовка робота
        var robotHeader = new RobotHeaderDto((byte)_Header.msgType, _Header.version, _Header.robotId, _Header.timestamp);
        
        var createdHeader = await robotApiClient.CreateRobotHeader(robotHeader);
        Logger?.LogInformation($"Created RobotHeader with ID: {createdHeader?.Id}");

        // Создание позы
        var pose = new PoseDto
        {
            X = _Pose.x,
            Y = _Pose.y,
            Heading = _Pose.heading
        };

        var createdPose = await robotApiClient.CreatePose(pose);
        Logger?.LogInformation($"Created Pose with ID: {createdPose.Id}");

        // Создание отчета о состоянии
        var stateReport = new StateReportDto
        {
            HeaderId  = createdHeader.Id,
            CurrentPoseId = createdPose.Id,
            TargetPoseId = createdPose.Id,
            Velocity = _StateReport.velocity,
            AngularVelocity = _StateReport.angularVelocity,
            Battery = _StateReport.battery,
            FsmState = (short)_StateReport.FSMstate,
            AlarmStatus = _StateReport.alarmStatus,
            Pallet = _StateReport.pallet,
            SubtaskIdx = _StateReport.subtaskIdx,
            SubtaskNum = _StateReport.subtaskNum,
            Progress = _StateReport.progress
        };

        var createdReport = await robotApiClient.CreateStateReport(stateReport);
        Logger?.LogInformation($"Created StateReport for Id: {createdReport.Id}");
    }
}
