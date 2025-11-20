namespace Silence.Water.Core.Enums;

/// <summary>
/// 比较类型枚举
/// </summary>
public enum ComparisonType
{
    // --- Range 范围比较 ---

    /// <summary>
    /// 介于...之间（包含边界值）
    /// </summary>
    BetweenInclusive,

    /// <summary>
    /// 介于...之间（不包含边界值）
    /// </summary>
    BetweenExclusive,

    /// <summary>
    /// 介于...之间（包含最小值，不包含最大值）
    /// </summary>
    BetweenInclusiveMinExclusiveMax,

    /// <summary>
    /// 介于...之间（不包含最小值，包含最大值）
    /// </summary>
    BetweenExclusiveMinInclusiveMax,

    // --- Simple 单值比较 ---

    /// <summary>
    /// 大于
    /// </summary>
    GreaterThan,

    /// <summary>
    /// 大于等于
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// 小于
    /// </summary>
    LessThan,

    /// <summary>
    /// 小于等于
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// 等于
    /// </summary>
    Equality,

    // --- Other 其他比较 ---

    /// <summary>
    /// 文本匹配
    /// </summary>
    TextMatch,

    /// <summary>
    /// 不得检出
    /// </summary>
    NotDetected
}