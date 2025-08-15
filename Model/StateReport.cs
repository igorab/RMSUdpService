using System.Runtime.InteropServices;

namespace RMSUdpService.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct StateReport 
{       
    public Header header { get; set; }  // Заголовок

    public Pose currentPose { get; set; }            // Текущие координаты и угол
    public Pose targetPose { get; set; }              // Целевые координаты и угол
    public float velocity { get; set; }                // Линейная скорость (м/с)
    public float angularVelocity { get; set; }         // Угловая скорость (рад/с)
    public byte battery { get; set; }                  // Уровень заряда батареи (%)
    public RobotFsmState FSMstate { get; set; }       // Текущее состояние КА робота
    public byte alarmStatus { get; set; }              // Уровень аварии (0 — норма)
    public byte pallet { get; set; }                   // Наличие палеты (0 — нет, 1 — есть)
    public short subtaskIdx { get; set; }             // Индекс текущей подзадачи
    public short subtaskNum { get; set; }             // Общее число подзадач
    public float progress { get; set; }                // Прогресс выполнения текущей подзадачи (%)

}

