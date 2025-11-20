using Silence.Water.BlackOdorous.Enums;

namespace Silence.Water.BlackOdorous.Models;

/// <summary>
/// 黑臭水体评估结果
/// </summary>
public class EvaluateResult
{
    /// <summary>
    /// 黑臭水体等级
    /// </summary>
    public BlackOdorousClass? BlackOdorousClass { get; set; }

    /// <summary>
    /// 黑臭指标及其对应的黑臭等级("透明度"/"溶解氧"/"氨氮", 黑臭等级)
    /// </summary>
    public List<(string, BlackOdorousClass)> Factors { get; set; } = [];
}