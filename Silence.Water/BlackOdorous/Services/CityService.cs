using Silence.Water.BlackOdorous.Enums;
using Silence.Water.BlackOdorous.Models;

namespace Silence.Water.BlackOdorous.Services;

/// <summary>
/// 城市黑臭水体服务
/// </summary>
public static class CityService
{
    /// <summary>
    /// 溶解氧轻度阈值
    /// </summary>
    private const decimal DO_MILD_THRESHOLD = 2.0m;

    /// <summary>
    /// 溶解氧重度阈值
    /// </summary>
    private const decimal DO_SEVERE_THRESHOLD = 0.2m;

    /// <summary>
    /// 氨氮轻度阈值
    /// </summary>
    private const decimal NH3N_MILD_THRESHOLD = 8.0m;

    /// <summary>
    /// 氨氮重度阈值
    /// </summary>
    private const decimal NH3N_SEVERE_THRESHOLD = 15.0m;

    /// <summary>
    /// 透明度轻度阈值
    /// </summary>
    private const decimal SD_MILD_THRESHOLD = 25.0m;

    /// <summary>
    /// 透明度重度阈值
    /// </summary>
    private const decimal SD_SEVERE_THRESHOLD = 10.0m;

    /// <summary>
    /// 水深阈值25cm
    /// </summary>
    private const decimal SD_DEPTH_LINE = 25.0m;

    /// <summary>
    /// 水深不足阈值时，透明度与水深的百分比阈值
    /// </summary>
    private const decimal SD_DEPTH_LINE_PERCENTAGE = 0.4m;


    /// <summary>
    /// 根据透明度、溶解氧、氨氮和水深计算黑臭水体等级
    /// </summary>
    /// <param name="SD">透明度(cm)</param>
    /// <param name="DO">溶解氧(mg/L)</param>
    /// <param name="NH3N">氨氮(mg/L)</param>
    /// <param name="depth">水深(cm)</param>
    /// <param name="isClear">是否清澈见底,true是/false否</param>
    /// <returns></returns>
    public static EvaluateResult GetBlackOdorousClass(decimal SD, decimal DO, decimal NH3N, decimal depth, bool isClear)
    {
        var result = new EvaluateResult();
        var sdClass = GetSDClass(SD, depth, isClear);
        var doClass = GetDOClass(DO);
        var nh3nClass = GetNH3NClass(NH3N);
        if (sdClass != null) result.Factors.Add(("透明度", (BlackOdorousClass)sdClass));
        if (doClass != null) result.Factors.Add(("溶解氧", (BlackOdorousClass)doClass));
        if (nh3nClass != null) result.Factors.Add(("氨氮", (BlackOdorousClass)nh3nClass));
        // 计算最终黑臭等级，取最严重的等级
        var classes = result.Factors.Select(f => f.Item2).ToList();
        result.BlackOdorousClass = classes.Count != 0 ? classes.Max() : null;
        return result;
    }

    /// <summary>
    /// 根据透明度和水深获取黑臭等级
    /// </summary>
    /// <param name="SD">透明度(cm)</param>
    /// <param name="depth">水深(cm)</param>
    /// <param name="isClear">是否清澈见底,true是/false否</param>
    /// <returns></returns>
    private static BlackOdorousClass? GetSDClass(decimal SD, decimal depth, bool isClear)
    {
        if (SD < 0) return null;
        if (depth < 0) return null;
        if (depth >= SD_DEPTH_LINE) // 水深足25cm
        {
            return SD switch
            {
                // 透明度大于25cm
                >= SD_MILD_THRESHOLD => BlackOdorousClass.不黑臭,
                // 透明度10-25cm
                < SD_MILD_THRESHOLD and >= SD_SEVERE_THRESHOLD => BlackOdorousClass.轻度黑臭,
                // 透明度小于10cm
                < SD_SEVERE_THRESHOLD => BlackOdorousClass.重度黑臭
            };
        }
        else // 水深不足25cm
        {
            //如果是清澈见底，则不黑臭
            if (isClear) return BlackOdorousClass.不黑臭;
            return SD < depth * SD_DEPTH_LINE_PERCENTAGE ? BlackOdorousClass.重度黑臭 : BlackOdorousClass.轻度黑臭;
        }
    }

    /// <summary>
    /// 根据溶解氧浓度获取黑臭等级
    /// </summary>
    /// <param name="DO"></param>
    /// <returns></returns>
    private static BlackOdorousClass? GetDOClass(decimal DO)
    {
        return DO switch
        {
            < 0 => null,
            >= DO_MILD_THRESHOLD => BlackOdorousClass.不黑臭,
            < DO_MILD_THRESHOLD and >= DO_SEVERE_THRESHOLD => BlackOdorousClass.轻度黑臭,
            < DO_SEVERE_THRESHOLD => BlackOdorousClass.重度黑臭,
        };
    }

    /// <summary>
    /// 根据氨氮浓度获取黑臭等级
    /// </summary>
    /// <param name="NH3N"></param>
    /// <returns></returns>
    private static BlackOdorousClass? GetNH3NClass(decimal NH3N)
    {
        return NH3N switch
        {
            < 0 => null,
            <= NH3N_MILD_THRESHOLD => BlackOdorousClass.不黑臭,
            > NH3N_MILD_THRESHOLD and <= NH3N_SEVERE_THRESHOLD => BlackOdorousClass.轻度黑臭,
            _ => BlackOdorousClass.重度黑臭
        };
    }
}