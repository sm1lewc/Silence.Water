using System.Diagnostics.CodeAnalysis;
using Silence.Water.Core.Enums;
using Silence.Water.Core.Services;
using Silence.Water.Groundwater.Enums;
using Silence.Water.Groundwater.Models;

namespace Silence.Water.Groundwater.Services;

/// <summary>
/// 地下水质量标准服务类
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class StandardService
{
    private static readonly
        StandardCacheManager<GroundwaterStandardVersion, GroundwaterFactor, GroundwaterStandard>
        _cacheManager = new(versionString => $"Silence.Water.Groundwater.Data.{versionString}.json");

    /// <summary>
    /// 获取地下水某项指标的水质类别
    /// 如果指标浓度包含检出限值（如 "0.01L"），则返回Ⅰ类
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="value"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static WaterQualityClass? GetFactorClass(GroundwaterFactor factor, object value,
        GroundwaterStandardVersion version = GroundwaterStandardVersion.GBT_14848_2017)
    {
        // 1. 获取指定版本的标准数据
        var versionStandards = _cacheManager.GetStandardsForVersion(version);
        // 2. 从该版本的标准中查找指标
        if (!versionStandards.TryGetValue(factor, out var factorStandard))
        {
            return null; // 未找到该指标的标准
        }

        // 尝试将value转为decimal，用于数值比较
        var isNumeric = decimal.TryParse(value.ToString(), out var numericValue);

        foreach (var limit in factorStandard.Limits.OrderBy(x => x.WaterQualityClass)) // 按水质类别从好到坏依次判断
        {
            if (limit.ComparisonType == ComparisonType.TextMatch) // 文本匹配
            {
                if (value.ToString() == limit.LimitValue)
                {
                    return limit.WaterQualityClass;
                }
            }
            else if (limit.ComparisonType == ComparisonType.NotDetected) // 未检出比较
            {
                if (!string.IsNullOrWhiteSpace(value.ToString())
                    && (value.ToString() == "未检出"
                        || value.ToString()!.EndsWith("L", StringComparison.OrdinalIgnoreCase)))
                {
                    return limit.WaterQualityClass;
                }
            }
            else if (isNumeric) // 数值比较
            {
                if (numericValue < 0) return null; // 负值则返回null，未监测
                var limitIsNumeric = decimal.TryParse(limit.LimitValue, out var limitValue);
                var rangeResult = ParseRangeValue(limit.LimitValue); // 尝试解析范围值
                switch (limit.ComparisonType)
                {
                    case ComparisonType.GreaterThan:
                        if (limitIsNumeric && numericValue > limitValue)
                            return limit.WaterQualityClass;
                        break;
                    case ComparisonType.GreaterThanOrEqual:
                        if (limitIsNumeric && numericValue >= limitValue)
                            return limit.WaterQualityClass;
                        break;
                    case ComparisonType.LessThan:
                        if (limitIsNumeric && numericValue < limitValue)
                            return limit.WaterQualityClass;
                        break;
                    case ComparisonType.LessThanOrEqual:
                        if (limitIsNumeric && numericValue <= limitValue)
                            return limit.WaterQualityClass;
                        break;
                    case ComparisonType.Equality:
                        if (limitIsNumeric && numericValue == limitValue)
                            return limit.WaterQualityClass;
                        break;
                    case ComparisonType.BetweenInclusive:
                        if (rangeResult.HasValue)
                        {
                            var (min, max) = rangeResult.Value;
                            if (numericValue >= min && numericValue <= max)
                                return limit.WaterQualityClass;
                        }
                        else
                        {
                            throw new FormatException($"{factorStandard.FactorName} 无法使用范围目标值进行水质类别判断");
                        }

                        break;
                    case ComparisonType.BetweenExclusive:
                        if (rangeResult.HasValue)
                        {
                            var (min, max) = rangeResult.Value;
                            if (numericValue > min && numericValue < max)
                                return limit.WaterQualityClass;
                        }
                        else
                        {
                            throw new FormatException($"{factorStandard.FactorName} 无法使用范围目标值进行水质类别判断");
                        }

                        break;
                    case ComparisonType.BetweenInclusiveMinExclusiveMax:
                        if (rangeResult.HasValue)
                        {
                            var (min, max) = rangeResult.Value;
                            if (numericValue >= min && numericValue < max)
                                return limit.WaterQualityClass;
                        }
                        else
                        {
                            throw new FormatException($"{factorStandard.FactorName} 无法使用范围目标值进行水质类别判断");
                        }

                        break;
                    case ComparisonType.BetweenExclusiveMinInclusiveMax:
                        if (rangeResult.HasValue)
                        {
                            var (min, max) = rangeResult.Value;
                            if (numericValue > min && numericValue <= max)
                                return limit.WaterQualityClass;
                        }
                        else
                        {
                            throw new FormatException($"{factorStandard.FactorName} 无法使用范围目标值进行水质类别判断");
                        }

                        break;
                    default: return null;
                }
            }
            else
            {
                if (value.ToString()!.EndsWith("L", StringComparison.OrdinalIgnoreCase)) // 检出限值，返回Ⅰ类
                {
                    return WaterQualityClass.Ⅰ;
                }
                else // 非数值、非检出限值、非未检出值，无法比较，跳过
                {
                    return null;
                }
            }
        }

        // 如果指标Ⅰ-Ⅳ类均未匹配，则返回Ⅴ类
        return WaterQualityClass.Ⅴ;
    }

    /// <summary>
    /// 获取多个地下水指标的水质类别
    /// </summary>
    public static Dictionary<GroundwaterFactor, WaterQualityClass?> GetFactorsClasses(
        Dictionary<GroundwaterFactor, object> values,
        GroundwaterStandardVersion version = GroundwaterStandardVersion.GBT_14848_2017)
    {
        var result = new Dictionary<GroundwaterFactor, WaterQualityClass?>();
        foreach (var kvp in values)
        {
            var factor = kvp.Key;
            var value = kvp.Value;
            var factorClass = GetFactorClass(factor, value, version);
            result[factor] = factorClass;
        }

        return result;
    }

    /// <summary>
    /// 获取多个地下水指标中的最差水质类别,忽略无法计算的指标和参数错误的指标
    /// </summary>
    public static WaterQualityClass? GetFactorsMaxClass(Dictionary<GroundwaterFactor, object> values,
        GroundwaterStandardVersion version = GroundwaterStandardVersion.GBT_14848_2017)
    {
        var result = new Dictionary<GroundwaterFactor, WaterQualityClass?>();
        foreach (var kvp in values)
        {
            var factor = kvp.Key;
            var value = kvp.Value;
            var factorClass = GetFactorClass(factor, value, version);
            result[factor] = factorClass;
        }

        return result.Values.Max();
    }

    /// <summary>
    /// 获取地下水某项指标的标准限值信息
    /// </summary>
    public static GroundwaterStandard? GetFactorStandard(GroundwaterFactor factor,
        GroundwaterStandardVersion version = GroundwaterStandardVersion.GBT_14848_2017)
    {
        var versionStandards = _cacheManager.GetStandardsForVersion(version);
        return versionStandards.GetValueOrDefault(factor);
    }

    /// <summary>
    /// 解析范围值，格式应为 "min-max"
    /// </summary>
    private static (decimal, decimal)? ParseRangeValue(string limitValue)
    {
        var parts = limitValue.Split('-');
        if (parts.Length == 2 &&
            decimal.TryParse(parts[0].Trim(), out var min) &&
            decimal.TryParse(parts[1].Trim(), out var max))
        {
            return (min, max);
        }

        return null;
    }
}
