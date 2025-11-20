using System.Diagnostics.CodeAnalysis;
using Silence.Water.Core.Models;
using Silence.Water.Groundwater.Enums;

namespace Silence.Water.Groundwater.Models;

/// <summary>
/// 地下水水质标准
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class GroundwaterStandard
{
    /// <summary>
    /// 监测指标
    /// </summary>
    public GroundwaterFactor Factor { get; set; }

    /// <summary>
    /// 监测指标名称
    /// </summary>
    public string FactorName { get; set; } = null!;

    /// <summary>
    /// 指标描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 指标的标准限值列表
    /// </summary>
    public List<WaterQualityClassLimit> Limits { get; set; } = [];
}