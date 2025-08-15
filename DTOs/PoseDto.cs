namespace RMSUdpService.DTOs;

[Serializable]
public struct PoseDto
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Координата X (м)
    /// </summary>
    public float X { get; set; }
    /// <summary>
    /// Координата Y (м)
    /// </summary>
    public float Y { get; set; }
    /// <summary>
    /// Угол поворота (рад)
    /// </summary>
    public float Heading { get; set; }

    public PoseDto(float x, float y, float heading)
    {
        Id = 0;
        X = x;
        Y = y;
        Heading = heading;
    }
}
