using RMSUdpService.DTOs;
using System.Net.Http.Json;

namespace RMSUDPAgent.Services;

internal class RobotApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public RobotApiClient(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<RobotHeaderDto> CreateRobotHeader(RobotHeaderDto robotHeader)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + "headers", robotHeader);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RobotHeaderDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<PoseDto> CreatePose(PoseDto pose)
    {
        var response = await _httpClient.PostAsJsonAsync(_baseUrl + "poses", pose);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PoseDto>();
    }

    public async Task<StateReportDto> CreateStateReport(StateReportDto stateReport)
    {
        var response = await _httpClient.PostAsJsonAsync(_baseUrl + "state-reports", stateReport);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StateReportDto>();
    }
}
