using System.Diagnostics.CodeAnalysis;
using Silence.Water.Core.Enums;

namespace Silence.Water.Core.Models;

/// <summary>
/// 水质类别标准限值信息
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class WaterQualityClassLimit
{
    /// <summary>
    /// 水质类别
    /// </summary>
    public WaterQualityClass WaterQualityClass { get; set; }

    /// <summary>
    /// 判断方法
    /// </summary>
    public ComparisonType ComparisonType { get; set; }

    /// <summary>
    /// 标准限值,string类型(decimal/无/有/min-max)
    /// </summary>
    public string LimitValue { get; set; } = null!;
}