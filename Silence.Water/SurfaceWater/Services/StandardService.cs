using System.Diagnostics.CodeAnalysis;
using Silence.Water.Core.Enums;
using Silence.Water.Core.Services;
using Silence.Water.SurfaceWater.Enums;
using Silence.Water.SurfaceWater.Models;

namespace Silence.Water.SurfaceWater.Services;

/// <summary>
/// 地表水质量标准服务类
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class StandardService
{
    private static readonly
        StandardCacheManager<SurfaceWaterStandardVersion, SurfaceWaterFactor, SurfaceWaterStandard>
        _cacheManager = new(versionString => $"Silence.Water.SurfaceWater.Data.{versionString}.json");

    /// <summary>
    /// 表1 地表水环境质量标准指标列表
    /// </summary>
    private static readonly List<SurfaceWaterFactor> Table1Factors =
    [
        SurfaceWaterFactor.pH值,
        SurfaceWaterFactor.溶解氧,
        SurfaceWaterFactor.高锰酸盐指数,
        SurfaceWaterFactor.化学需氧量,
        SurfaceWaterFactor.五日生化需氧量,
        SurfaceWaterFactor.氨氮,
        SurfaceWaterFactor.总磷_河流,
        SurfaceWaterFactor.总磷_湖库,
        SurfaceWaterFactor.总氮,
        SurfaceWaterFactor.铜,
        SurfaceWaterFactor.锌,
        SurfaceWaterFactor.氟化物,
        SurfaceWaterFactor.硒,
        SurfaceWaterFactor.砷,
        SurfaceWaterFactor.汞,
        SurfaceWaterFactor.镉,
        SurfaceWaterFactor.六价铬,
        SurfaceWaterFactor.铅,
        SurfaceWaterFactor.氰化物,
        SurfaceWaterFactor.挥发酚,
        SurfaceWaterFactor.石油类,
        SurfaceWaterFactor.阴离子表面活性剂,
        SurfaceWaterFactor.硫化物,
        SurfaceWaterFactor.粪大肠菌群
    ];

    /// <summary>
    /// 根据指定的地表水因子和数值，获取对应的水质类别
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="value">仅接受数值</param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static WaterQualityClass? GetFactorClass(SurfaceWaterFactor factor, decimal value,
        SurfaceWaterStandardVersion version = SurfaceWaterStandardVersion.GB_3838_2002)
    {
        if (!Table1Factors.Contains(factor)) throw new NotImplementedException("非表1指标水质类别暂不支持判断");

        // 1. 获取指定版本的标准数据
        var versionStandards = _cacheManager.GetStandardsForVersion(version);
        // 2. 从该版本的标准中查找指标
        if (!versionStandards.TryGetValue(factor, out var factorStandard))
        {
            return null; // 未找到该指标的标准
        }

        foreach (var limit in factorStandard.Limits.OrderBy(x => x.WaterQualityClass)) // 按水质类别从好到坏依次判断
        {
            if (value < 0) return null; // 负值则返回null，未监测
            var limitIsNumeric = decimal.TryParse(limit.LimitValue, out var limitValue);
            var rangeResult = ParseRangeValue(limit.LimitValue); // 尝试解析范围值
            switch (limit.ComparisonType)
            {
                case ComparisonType.GreaterThan:
                    if (limitIsNumeric && value > limitValue)
                        return limit.WaterQualityClass;
                    break;
                case ComparisonType.GreaterThanOrEqual:
                    if (limitIsNumeric && value >= limitValue)
                        return limit.WaterQualityClass;
                    break;
                case ComparisonType.LessThan:
                    if (limitIsNumeric && value < limitValue)
                        return limit.WaterQualityClass;
                    break;
                case ComparisonType.LessThanOrEqual:
                    if (limitIsNumeric && value <= limitValue)
                        return limit.WaterQualityClass;
                    break;
                case ComparisonType.Equality:
                    if (limitIsNumeric && value == limitValue)
                        return limit.WaterQualityClass;
                    break;
                case ComparisonType.BetweenInclusive:
                    if (rangeResult.HasValue)
                    {
                        var (min, max) = rangeResult.Value;
                        if (value >= min && value <= max)
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
                        if (value > min && value < max)
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
                        if (value >= min && value < max)
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
                        if (value > min && value <= max)
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

        // 如果指标Ⅰ-Ⅴ类均未匹配，则返回劣Ⅴ类
        return WaterQualityClass.劣Ⅴ;
    }


    /// <summary>
    /// 根据指定的地表水因子和数值字典，获取对应的水质类别字典
    /// </summary>
    public static Dictionary<SurfaceWaterFactor, WaterQualityClass?> GetFactorsClasses(
        Dictionary<SurfaceWaterFactor, decimal> values,
        SurfaceWaterStandardVersion version = SurfaceWaterStandardVersion.GB_3838_2002)
    {
        var result = new Dictionary<SurfaceWaterFactor, WaterQualityClass?>();
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
    /// 根据指定的地表水因子和数值字典，获取对应的最差水质类别
    /// </summary>
    public static WaterQualityClass? GetFactorsMaxClass(Dictionary<SurfaceWaterFactor, decimal> values,
        SurfaceWaterStandardVersion version = SurfaceWaterStandardVersion.GB_3838_2002)
    {
        var result = new Dictionary<SurfaceWaterFactor, WaterQualityClass?>();
        foreach (var (factor, value) in values)
        {
            var factorClass = GetFactorClass(factor, value, version);
            result[factor] = factorClass;
        }

        return result.Values.Max();
    }

    /// <summary>
    /// 获取指定版本的地表水因子标准对象,非表1指标返回Ⅰ类代表标准值
    /// </summary>
    public static SurfaceWaterStandard? GetFactorStandard(SurfaceWaterFactor factor,
        SurfaceWaterStandardVersion version = SurfaceWaterStandardVersion.GB_3838_2002)
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