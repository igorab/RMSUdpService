using RMSUdpService.DTOs;
using RMSUdpService.Model;
using RMSUdpService.RTC;
using System.Buffers.Text;

namespace RMSUDPAgent.Services;

internal class RMSClient
{
    private static  HttpClient client = new HttpClient();
    private static  string baseUrl = "https://localhost:7038/api/RMS/";

    private static Pose _Pose  = new Pose() { x = 10.5f, y = 20.3f, heading = 1.57f };
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

    public static void RunStateReport(StateReport stateReport, HttpClient httpClient, string url)
    {
        _StateReport = stateReport;
        _Header = stateReport.header;
        _Pose   = stateReport.targetPose;

        client = httpClient;
        baseUrl = url;

        RunAsync();
    }

    
    public static async Task RunAsync()
    {
        var robotApiClient = new RobotApiClient(client, baseUrl);

        // Создание заголовка робота
        var robotHeader = new RobotHeaderDto((byte)_Header.msgType, _Header.version, _Header.robotId, _Header.timestamp);
        
        var createdHeader = await robotApiClient.CreateRobotHeader(robotHeader);
        Console.WriteLine($"Created RobotHeader with ID: {createdHeader.Id}");

        // Создание позы
        var pose = new PoseDto
        {
            X = _Pose.x,
            Y = _Pose.y,
            Heading = _Pose.heading
        };

        var createdPose = await robotApiClient.CreatePose(pose);
        Console.WriteLine($"Created Pose with ID: {createdPose.Id}");

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
        Console.WriteLine($"Created StateReport for Id: {createdReport.Id}");
    }
}
