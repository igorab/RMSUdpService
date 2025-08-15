namespace RMSUdpService.Model;

[Serializable]
public struct Pose
{ 
    /// <summary>
    /// Координата X (м)
    /// </summary>
    public float x { get; set; }
    /// <summary>
    /// Координата Y (м)
    /// </summary>
    public float y { get; set; }
    /// <summary>
    /// Угол поворота (рад)
    /// </summary>
    public float heading { get; set; }

    public Pose(float x, float y, float heading)
    {        
        this.x = x;
        this.y = y;
        this.heading = heading;
    }
}
