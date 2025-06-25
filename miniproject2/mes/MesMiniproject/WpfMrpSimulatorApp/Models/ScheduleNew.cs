using System;
using System.Collections.Generic;

namespace WpfMrpSimulatorApp.Models;

public partial class ScheduleNew
{
    public int SchIdx { get; set; }

    /// <summary>
    /// 공장코드
    /// </summary>
    public string PlantCode { get; set; } = null!;

    public string PlantName { get; set; }

    public DateOnly SchDate { get; set; }

    /// <summary>
    /// 로드타입(초)
    /// 
    /// </summary>
    public int LoadTime { get; set; }

    public TimeOnly? SchStartTime { get; set; }

    public TimeOnly? SchEndTime { get; set; }

    public string? SchFacilityId { get; set; }

    public string? SchFacilityName { get; set; }

    /// <summary>
    /// 계획 목표 수량 
    /// 
    /// </summary>
    public int SchAmount { get; set; }

    public DateTime? RegDt { get; set; }

    /// <summary>
    /// 수정일
    /// </summary>
    public DateTime? ModDt { get; set; }

    public virtual ICollection<Process> Processes { get; set; } = new List<Process>();
}
