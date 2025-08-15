namespace RMSUdpService.DTOs;

using System;

/// <summary>
/// Класс, представляющий отчет о состоянии робота.
/// </summary>
public class StateReportDto
{
    /// <summary>
    /// Уникальный идентификатор отчета о состоянии.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор заголовка, к которому относится этот отчет.
    /// </summary>
    public int HeaderId { get; set; }

    /// <summary>
    /// Идентификатор текущей позы робота.
    /// </summary>
    public int CurrentPoseId { get; set; }

    /// <summary>
    /// Идентификатор целевой позы робота.
    /// </summary>
    public int TargetPoseId { get; set; }

    /// <summary>
    /// Линейная скорость робота в метрах в секунду (м/с).
    /// </summary>
    public float Velocity { get; set; }

    /// <summary>
    /// Угловая скорость робота в радианах в секунду (рад/с).
    /// </summary>
    public float AngularVelocity { get; set; }

    /// <summary>
    /// Уровень заряда батареи в процентах (%).
    /// </summary>
    public short Battery { get; set; }

    /// <summary>
    /// Текущее состояние конечного автомата (FSM) робота.
    /// </summary>
    public short FsmState { get; set; }

    /// <summary>
    /// Уровень аварии (0 — норма).
    /// </summary>
    public short AlarmStatus { get; set; }

    /// <summary>
    /// Наличие палеты (0 — нет, 1 — есть).
    /// </summary>
    public short Pallet { get; set; }

    /// <summary>
    /// Индекс текущей подзадачи.
    /// </summary>
    public short SubtaskIdx { get; set; }

    /// <summary>
    /// Общее число подзадач.
    /// </summary>
    public short SubtaskNum { get; set; }

    /// <summary>
    /// Прогресс выполнения текущей подзадачи в процентах (%).
    /// </summary>
    public float Progress { get; set; }
}

