using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace RMSUdpService.SSDP;

public class SSDPServer
{
    public static ILogger<Worker>? Logger { private get; set; }

    /// <summary>
    /// // Парсим запрос
    /// </summary>
    /// <param name="rawRequest">строка запроса</param>
    /// <returns></returns>
    public static SSDPRequest ParseSSDPRequest(string rawRequest)
    {
        var request = new SSDPRequest();

        // Разделяем запрос на строки
        string[] lines = rawRequest.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
            return request;

        // Парсим первую строку (метод)
        string[] methodParts = lines[0].Split(' ');
        if (methodParts.Length > 0)
        {
            request.Method = methodParts[0];
        }

        // Парсим заголовки
        for (int i = 1; i < lines.Length; i++)
        {
            int separatorIndex = lines[i].IndexOf(':');
            if (separatorIndex > 0)
            {
                string key = lines[i].Substring(0, separatorIndex).Trim();
                string value = lines[i].Substring(separatorIndex + 1).Trim();
                request.Headers[key] = value;
            }
        }

        return request;
    }

    /// <summary>
    /// // Логируем детали запроса
    /// </summary>
    /// <param name="request"></param>
    /// <param name="remoteEndPoint"></param>
    public static void LogRequestDetails(SSDPRequest request, IPEndPoint remoteEndPoint)
    {
        Logger?.LogInformation($"Request from: {remoteEndPoint.Address}:{remoteEndPoint.Port}");
        Logger?.LogInformation($"Method: {request.Method}");

        Logger?.LogInformation("Headers:");
        foreach (var header in request.Headers)
        {
            Logger?.LogInformation($"  {header.Key}: {header.Value}");
        }
        Logger?.LogInformation("-----");
    }

    /// <summary>
    ///  Генерация ответа
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GenerateResponse(SSDPRequest request)
    {
        // Проверяем, является ли запрос M-SEARCH
        if (request.Method != "M-SEARCH")
        {
            return string.Empty;
        }

        // Проверяем обязательные заголовки
        if (!request.Headers.ContainsKey("MAN") ||
            request.Headers["MAN"] != "\"ssdp:discover\"")
        {
            return string.Empty;
        }

        // Определяем тип устройства из ST (Search Target)
        string searchTarget = request.Headers.ContainsKey("ST")
            ? request.Headers["ST"]
            : "ssdp:all";

        // Генерируем уникальный идентификатор устройства
        string uuid = Guid.NewGuid().ToString();

        // Формируем ответ в зависимости от типа поиска
        switch (searchTarget)
        {
            case "ssdp:all":
            case "upnp:rootdevice":
                return CreateSSDPResponse(
                    uuid,
                    "upnp:rootdevice",
                    "http://192.168.1.100:8080/description.xml"
                );

            case "uuid:":
                return CreateSSDPResponse(
                    uuid,
                    "uuid:" + uuid,
                    "http://192.168.1.100:8080/description.xml"
                );

            default:
                return string.Empty;
        }
    }

    private static string CreateSSDPResponse(string uuid, string searchTarget, string location)
    {
        return "HTTP/1.1 200 OK\r\n" +
               $"CACHE-CONTROL: max-age=1800\r\n" +
               $"DATE: {DateTime.UtcNow:R}\r\n" +
               "EXT:\r\n" +
               $"LOCATION: {location}\r\n" +
               "OPT: \"http://schemas.upnp.org/upnp/1/0/\"; ns=01\r\n" +
               $"01-NLS: {Guid.NewGuid()}\r\n" +
               "SERVER: Windows/10.0 UPnP/1.0 MyCustomServer/1.0\r\n" +
               $"ST: {searchTarget}\r\n" +
               $"USN: uuid:{uuid}::{searchTarget}\r\n" +
               "\r\n";
    }

    internal static bool ValidateRequest(SSDPRequest request)
    {
        // Проверяем, является ли запрос M-SEARCH
        if (request.Method != "M-SEARCH")
        {
            return false;
        }

        //// Проверяем обязательные заголовки
        //if (!request.Headers.ContainsKey("MAN") ||
        //    request.Headers["MAN"] != "\"ssdp:discover\"")
        //{
        //    return string.Empty;
        //}

        // Определяем тип устройства из ST (Search Target)
        if (request.Headers.ContainsKey("ST"))
        {
            string searchTarget = request.Headers["ST"];

            if (searchTarget.Contains("RMSPrivateServer"))
            {
                return true;
            }
            else
                return false;
        }

        return true;
    }
}
