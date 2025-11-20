using Silence.Water.BlackOdorous.Enums;
using Silence.Water.BlackOdorous.Models;
using Silence.Water.BlackOdorous.Services;

namespace Silence.Water.Test.BlackOdorous.Services;

/// <summary>
/// 城市黑臭水体服务类的单元测试
/// </summary>
public class CityServiceTests
{
    #region GetBlackOdorousClass - 综合评估测试

    [Fact]
    public void GetBlackOdorousClass_AllFactorsNotBlackOdorous_ReturnsNotBlackOdorous()
    {
        // Arrange - 所有指标都不黑臭
        decimal SD = 30m;      // 透明度 > 25cm
        decimal DO = 5.0m;     // 溶解氧 > 2.0mg/L
        decimal NH3N = 5.0m;   // 氨氮 <= 8.0mg/L
        decimal depth = 50m;   // 水深充足
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.不黑臭, result.BlackOdorousClass);
        Assert.Equal(3, result.Factors.Count);
        Assert.All(result.Factors, f => Assert.Equal(BlackOdorousClass.不黑臭, f.Item2));
    }

    [Fact]
    public void GetBlackOdorousClass_AllFactorsMildBlackOdorous_ReturnsMildBlackOdorous()
    {
        // Arrange - 所有指标都是轻度黑臭
        decimal SD = 15m;      // 透明度 10-25cm
        decimal DO = 1.0m;     // 溶解氧 0.2-2.0mg/L
        decimal NH3N = 10.0m;  // 氨氮 8-15mg/L
        decimal depth = 50m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.轻度黑臭, result.BlackOdorousClass);
        Assert.Equal(3, result.Factors.Count);
        Assert.All(result.Factors, f => Assert.Equal(BlackOdorousClass.轻度黑臭, f.Item2));
    }

    [Fact]
    public void GetBlackOdorousClass_AllFactorsSevereBlackOdorous_ReturnsSevereBlackOdorous()
    {
        // Arrange - 所有指标都是重度黑臭
        decimal SD = 5m;       // 透明度 < 10cm
        decimal DO = 0.1m;     // 溶解氧 < 0.2mg/L
        decimal NH3N = 20.0m;  // 氨氮 > 15mg/L
        decimal depth = 50m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.重度黑臭, result.BlackOdorousClass);
        Assert.Equal(3, result.Factors.Count);
        Assert.All(result.Factors, f => Assert.Equal(BlackOdorousClass.重度黑臭, f.Item2));
    }

    [Fact]
    public void GetBlackOdorousClass_MixedFactors_ReturnsWorstClass()
    {
        // Arrange - 混合等级，应返回最严重的等级
        decimal SD = 30m;      // 透明度不黑臭
        decimal DO = 1.0m;     // 溶解氧轻度黑臭
        decimal NH3N = 20.0m;  // 氨氮重度黑臭
        decimal depth = 50m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.重度黑臭, result.BlackOdorousClass); // 取最严重的等级
        Assert.Equal(3, result.Factors.Count);
        Assert.Contains(result.Factors, f => f.Item1 == "透明度" && f.Item2 == BlackOdorousClass.不黑臭);
        Assert.Contains(result.Factors, f => f.Item1 == "溶解氧" && f.Item2 == BlackOdorousClass.轻度黑臭);
        Assert.Contains(result.Factors, f => f.Item1 == "氨氮" && f.Item2 == BlackOdorousClass.重度黑臭);
    }

    [Fact]
    public void GetBlackOdorousClass_WithNegativeSD_IgnoresSDFactor()
    {
        // Arrange - 包含负值的透明度
        decimal SD = -1m;      // 无效透明度
        decimal DO = 5.0m;     // 溶解氧不黑臭
        decimal NH3N = 5.0m;   // 氨氮不黑臭
        decimal depth = 50m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Factors.Count); // 只有2个有效因子
        Assert.DoesNotContain(result.Factors, f => f.Item1 == "透明度");
        Assert.Equal(BlackOdorousClass.不黑臭, result.BlackOdorousClass);
    }

    [Fact]
    public void GetBlackOdorousClass_WithNegativeDO_IgnoresDOFactor()
    {
        // Arrange - 包含负值的溶解氧
        decimal SD = 30m;
        decimal DO = -1m;      // 无效溶解氧
        decimal NH3N = 5.0m;
        decimal depth = 50m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Factors.Count);
        Assert.DoesNotContain(result.Factors, f => f.Item1 == "溶解氧");
    }

    [Fact]
    public void GetBlackOdorousClass_WithNegativeNH3N_IgnoresNH3NFactor()
    {
        // Arrange - 包含负值的氨氮
        decimal SD = 30m;
        decimal DO = 5.0m;
        decimal NH3N = -1m;    // 无效氨氮
        decimal depth = 50m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Factors.Count);
        Assert.DoesNotContain(result.Factors, f => f.Item1 == "氨氮");
    }

    [Fact]
    public void GetBlackOdorousClass_WithNegativeDepth_IgnoresSDFactor()
    {
        // Arrange - 包含负值的水深
        decimal SD = 30m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        decimal depth = -1m;   // 无效水深
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Factors.Count);
        Assert.DoesNotContain(result.Factors, f => f.Item1 == "透明度");
    }

    [Fact]
    public void GetBlackOdorousClass_AllNegativeValues_ReturnsNullClass()
    {
        // Arrange - 所有值都无效
        decimal SD = -1m;
        decimal DO = -1m;
        decimal NH3N = -1m;
        decimal depth = 50m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.BlackOdorousClass);
        Assert.Empty(result.Factors);
    }

    #endregion

    #region 透明度 (SD) 测试 - 水深充足场景

    [Theory]
    [InlineData(30)]   // 透明度 > 25cm
    [InlineData(25)]   // 透明度 = 25cm，边界值
    [InlineData(100)]  // 透明度很大
    [InlineData(26)]   // 刚超过阈值
    public void GetBlackOdorousClass_SDNotBlackOdorous_WaterDepthSufficient(decimal SD)
    {
        // Arrange - 水深充足，透明度 >= 25cm
        decimal depth = 50m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.不黑臭, sdFactor.Item2);
    }

    [Theory]
    [InlineData(15)]   // 透明度 10-25cm 中间值
    [InlineData(10)]   // 透明度 = 10cm，下边界值
    [InlineData(24)]   // 接近上限
    [InlineData(24.9)] // 非常接近上限
    [InlineData(10.1)] // 刚超过下限
    public void GetBlackOdorousClass_SDMildBlackOdorous_WaterDepthSufficient(decimal SD)
    {
        // Arrange - 水深充足，10cm <= 透明度 < 25cm
        decimal depth = 50m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.轻度黑臭, sdFactor.Item2);
    }

    [Theory]
    [InlineData(5)]    // 透明度 < 10cm
    [InlineData(9)]    // 接近临界值
    [InlineData(0)]    // 透明度为0
    [InlineData(9.9)]  // 非常接近临界值
    [InlineData(0.1)]  // 极小透明度
    public void GetBlackOdorousClass_SDSevereBlackOdorous_WaterDepthSufficient(decimal SD)
    {
        // Arrange - 水深充足，透明度 < 10cm
        decimal depth = 50m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.重度黑臭, sdFactor.Item2);
    }

    [Fact]
    public void GetBlackOdorousClass_DepthExactly25_TreatsAsSufficient()
    {
        // Arrange - 水深正好25cm（边界值）
        decimal SD = 30m;
        decimal depth = 25m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.不黑臭, sdFactor.Item2);
    }

    #endregion

    #region 透明度 (SD) 测试 - 水深不足场景

    [Fact]
    public void GetBlackOdorousClass_SDClearToBottom_WaterDepthInsufficient_ReturnsNotBlackOdorous()
    {
        // Arrange - 水深不足25cm，但清澈见底
        decimal SD = 15m;
        decimal depth = 20m;   // 水深 < 25cm
        bool isClear = true;   // 清澈见底
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.不黑臭, sdFactor.Item2);
    }

    [Theory]
    [InlineData(10, 20)]   // SD(10) >= depth(20) * 0.4 = 8
    [InlineData(9, 20)]    // SD(9) >= depth(20) * 0.4 = 8
    [InlineData(8, 20)]    // SD(8) >= depth(20) * 0.4 = 8，边界值
    [InlineData(8.1, 20)]  // 刚超过边界
    [InlineData(15, 20)]   // 远超过边界
    public void GetBlackOdorousClass_SDMildBlackOdorous_WaterDepthInsufficient(decimal SD, decimal depth)
    {
        // Arrange - 水深不足25cm，透明度 >= 40%水深
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.轻度黑臭, sdFactor.Item2);
    }

    [Theory]
    [InlineData(5, 20)]    // SD(5) < depth(20) * 0.4 = 8
    [InlineData(7, 20)]    // SD(7) < depth(20) * 0.4 = 8
    [InlineData(0, 20)]    // SD = 0
    [InlineData(7.9, 20)]  // 接近但未达到边界
    [InlineData(5.9, 15)]    // SD(6) = depth(15) * 0.4，边界值
    public void GetBlackOdorousClass_SDSevereBlackOdorous_WaterDepthInsufficient(decimal SD, decimal depth)
    {
        // Arrange - 水深不足25cm，透明度 < 40%水深
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.重度黑臭, sdFactor.Item2);
    }

    [Theory]
    [InlineData(5, true)]   // 清澈见底
    [InlineData(10, true)]  // 清澈见底
    [InlineData(15, true)]  // 清澈见底
    public void GetBlackOdorousClass_ShallowWaterClear_AlwaysNotBlackOdorous(decimal SD, bool isClear)
    {
        // Arrange - 水深不足，但清澈见底，无论透明度多少都不黑臭
        decimal depth = 20m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.不黑臭, sdFactor.Item2);
    }

    #endregion

    #region 溶解氧 (DO) 测试

    [Theory]
    [InlineData(5.0)]   // DO > 2.0mg/L
    [InlineData(2.0)]   // DO = 2.0mg/L，边界值
    [InlineData(10.0)]  // 高溶解氧
    [InlineData(100.0)] // 极高溶解氧
    [InlineData(2.1)]   // 刚超过阈值
    public void GetBlackOdorousClass_DONotBlackOdorous(decimal DO)
    {
        // Arrange - DO >= 2.0mg/L
        decimal SD = 30m;
        decimal depth = 50m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var doFactor = result.Factors.FirstOrDefault(f => f.Item1 == "溶解氧");
        Assert.NotEqual(default, doFactor);
        Assert.Equal(BlackOdorousClass.不黑臭, doFactor.Item2);
    }

    [Theory]
    [InlineData(1.0)]   // 0.2 <= DO < 2.0
    [InlineData(0.5)]   // 中间值
    [InlineData(0.2)]   // DO = 0.2，下边界值
    [InlineData(1.9)]   // 接近上限
    [InlineData(1.99)]  // 非常接近上限
    [InlineData(0.21)]  // 刚超过下限
    public void GetBlackOdorousClass_DOMildBlackOdorous(decimal DO)
    {
        // Arrange - 0.2 <= DO < 2.0mg/L
        decimal SD = 30m;
        decimal depth = 50m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var doFactor = result.Factors.FirstOrDefault(f => f.Item1 == "溶解氧");
        Assert.NotEqual(default, doFactor);
        Assert.Equal(BlackOdorousClass.轻度黑臭, doFactor.Item2);
    }

    [Theory]
    [InlineData(0.1)]   // DO < 0.2mg/L
    [InlineData(0.0)]   // DO = 0
    [InlineData(0.19)]  // 接近临界值
    [InlineData(0.01)]  // 极低溶解氧
    public void GetBlackOdorousClass_DOSevereBlackOdorous(decimal DO)
    {
        // Arrange - DO < 0.2mg/L
        decimal SD = 30m;
        decimal depth = 50m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var doFactor = result.Factors.FirstOrDefault(f => f.Item1 == "溶解氧");
        Assert.NotEqual(default, doFactor);
        Assert.Equal(BlackOdorousClass.重度黑臭, doFactor.Item2);
    }

    #endregion

    #region 氨氮 (NH3N) 测试

    [Theory]
    [InlineData(0.0)]   // NH3N = 0
    [InlineData(5.0)]   // NH3N < 8.0mg/L
    [InlineData(8.0)]   // NH3N = 8.0，边界值
    [InlineData(7.9)]   // 接近上限
    [InlineData(0.1)]   // 极低氨氮
    public void GetBlackOdorousClass_NH3NNotBlackOdorous(decimal NH3N)
    {
        // Arrange - NH3N <= 8.0mg/L
        decimal SD = 30m;
        decimal depth = 50m;
        decimal DO = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var nh3nFactor = result.Factors.FirstOrDefault(f => f.Item1 == "氨氮");
        Assert.NotEqual(default, nh3nFactor);
        Assert.Equal(BlackOdorousClass.不黑臭, nh3nFactor.Item2);
    }

    [Theory]
    [InlineData(10.0)]  // 8 < NH3N <= 15
    [InlineData(8.1)]   // 刚超过下限
    [InlineData(15.0)]  // NH3N = 15，上边界值
    [InlineData(14.9)]  // 接近上限
    [InlineData(12.0)]  // 中间值
    public void GetBlackOdorousClass_NH3NMildBlackOdorous(decimal NH3N)
    {
        // Arrange - 8 < NH3N <= 15mg/L
        decimal SD = 30m;
        decimal depth = 50m;
        decimal DO = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var nh3nFactor = result.Factors.FirstOrDefault(f => f.Item1 == "氨氮");
        Assert.NotEqual(default, nh3nFactor);
        Assert.Equal(BlackOdorousClass.轻度黑臭, nh3nFactor.Item2);
    }

    [Theory]
    [InlineData(20.0)]   // NH3N > 15mg/L
    [InlineData(15.1)]   // 刚超过临界值
    [InlineData(100.0)]  // 高浓度
    [InlineData(1000.0)] // 极高浓度
    [InlineData(16.0)]   // 略高于阈值
    public void GetBlackOdorousClass_NH3NSevereBlackOdorous(decimal NH3N)
    {
        // Arrange - NH3N > 15mg/L
        decimal SD = 30m;
        decimal depth = 50m;
        decimal DO = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var nh3nFactor = result.Factors.FirstOrDefault(f => f.Item1 == "氨氮");
        Assert.NotEqual(default, nh3nFactor);
        Assert.Equal(BlackOdorousClass.重度黑臭, nh3nFactor.Item2);
    }

    #endregion

    #region 边界值综合测试

    [Fact]
    public void GetBlackOdorousClass_AllBoundaryValuesNotBlackOdorous()
    {
        // Arrange - 所有边界值都在不黑臭范围
        decimal SD = 25m;     // 透明度边界值
        decimal depth = 25m;  // 水深边界值
        decimal DO = 2.0m;    // 溶解氧边界值
        decimal NH3N = 8.0m;  // 氨氮边界值
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.不黑臭, result.BlackOdorousClass);
        Assert.Equal(3, result.Factors.Count);
    }

    [Fact]
    public void GetBlackOdorousClass_AllBoundaryValuesMildBlackOdorous()
    {
        // Arrange - 所有边界值都在轻度黑臭范围
        decimal SD = 10m;     // 透明度下边界
        decimal depth = 50m;
        decimal DO = 0.2m;    // 溶解氧下边界
        decimal NH3N = 15.0m; // 氨氮上边界
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.轻度黑臭, result.BlackOdorousClass);
        Assert.Equal(3, result.Factors.Count);
    }

    [Fact]
    public void GetBlackOdorousClass_ZeroValues_ValidEvaluation()
    {
        // Arrange - 零值测试
        decimal SD = 0m;
        decimal depth = 50m;
        decimal DO = 0m;
        decimal NH3N = 0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Factors.Count);
        // DO=0应该是重度黑臭，SD=0应该是重度黑臭，NH3N=0应该是不黑臭
        Assert.Equal(BlackOdorousClass.重度黑臭, result.BlackOdorousClass);
    }

    #endregion

    #region 实际场景集成测试

    [Fact]
    public void IntegrationTest_TypicalCleanRiver()
    {
        // Arrange - 模拟典型的清洁河流
        decimal SD = 80m;      // 透明度好
        decimal depth = 150m;  // 水深充足
        decimal DO = 8.5m;     // 溶解氧充足
        decimal NH3N = 0.5m;   // 氨氮低
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.不黑臭, result.BlackOdorousClass);
        Assert.Equal(3, result.Factors.Count);
        Assert.All(result.Factors, f => Assert.Equal(BlackOdorousClass.不黑臭, f.Item2));
    }

    [Fact]
    public void IntegrationTest_PollutedUrbanWaterBody()
    {
        // Arrange - 模拟严重污染的城市水体
        decimal SD = 8m;       // 透明度差
        decimal depth = 80m;
        decimal DO = 0.15m;    // 溶解氧低
        decimal NH3N = 18.0m;  // 氨氮高
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.重度黑臭, result.BlackOdorousClass);
        Assert.Equal(3, result.Factors.Count);
        Assert.All(result.Factors, f => Assert.Equal(BlackOdorousClass.重度黑臭, f.Item2));
    }

    [Fact]
    public void IntegrationTest_ShallowClearPond()
    {
        // Arrange - 模拟浅而清的池塘
        decimal SD = 18m;
        decimal depth = 20m;   // 水深不足25cm
        bool isClear = true;   // 清澈见底
        decimal DO = 6.0m;
        decimal NH3N = 3.0m;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.不黑臭, result.BlackOdorousClass);
        Assert.Contains(result.Factors, f => f.Item1 == "透明度" && f.Item2 == BlackOdorousClass.不黑臭);
    }

    [Fact]
    public void IntegrationTest_ShallowTurbidPond()
    {
        // Arrange - 模拟浅而浑浊的池塘
        decimal SD = 5m;       // 透明度 < depth * 0.4
        decimal depth = 20m;   // 水深不足25cm
        bool isClear = false;  // 不清澈
        decimal DO = 6.0m;
        decimal NH3N = 3.0m;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        var sdFactor = result.Factors.FirstOrDefault(f => f.Item1 == "透明度");
        Assert.NotEqual(default, sdFactor);
        Assert.Equal(BlackOdorousClass.重度黑臭, sdFactor.Item2);
    }

    [Fact]
    public void IntegrationTest_BorderlineMildBlackOdorous()
    {
        // Arrange - 模拟边界轻度黑臭水体
        decimal SD = 12m;      // 轻度黑臭
        decimal depth = 100m;
        decimal DO = 1.5m;     // 轻度黑臭
        decimal NH3N = 9.0m;   // 轻度黑臭
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.轻度黑臭, result.BlackOdorousClass);
        Assert.Equal(3, result.Factors.Count);
        Assert.All(result.Factors, f => Assert.Equal(BlackOdorousClass.轻度黑臭, f.Item2));
    }

    [Fact]
    public void IntegrationTest_ImprovingWaterQuality()
    {
        // Arrange - 模拟改善中的水质（部分指标好转）
        decimal SD = 12m;      // 轻度黑臭
        decimal depth = 50m;
        decimal DO = 3.0m;     // 不黑臭（已改善）
        decimal NH3N = 9.0m;   // 轻度黑臭
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.轻度黑臭, result.BlackOdorousClass);
        Assert.Contains(result.Factors, f => f.Item1 == "溶解氧" && f.Item2 == BlackOdorousClass.不黑臭);
        Assert.Contains(result.Factors, f => f.Item1 == "透明度" && f.Item2 == BlackOdorousClass.轻度黑臭);
    }

    [Fact]
    public void IntegrationTest_OnlyOneFactorSevere()
    {
        // Arrange - 只有一个因子重度黑臭
        decimal SD = 30m;      // 不黑臭
        decimal depth = 50m;
        decimal DO = 5.0m;     // 不黑臭
        decimal NH3N = 20.0m;  // 重度黑臭
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.重度黑臭, result.BlackOdorousClass);
    }

    [Fact]
    public void IntegrationTest_ExtremeValues()
    {
        // Arrange - 极端值测试
        decimal SD = 10000m;    // 极大透明度
        decimal depth = 10000m; // 极深水体
        decimal DO = 100m;      // 极高溶解氧
        decimal NH3N = 0.001m;  // 极低氨氮
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BlackOdorousClass.不黑臭, result.BlackOdorousClass);
        Assert.All(result.Factors, f => Assert.Equal(BlackOdorousClass.不黑臭, f.Item2));
    }

    #endregion

    #region EvaluateResult 结构测试

    [Fact]
    public void GetBlackOdorousClass_AlwaysReturnsNonNullResult()
    {
        // Arrange
        decimal SD = 30m;
        decimal depth = 50m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Factors);
        Assert.IsType<EvaluateResult>(result);
    }

    [Fact]
    public void GetBlackOdorousClass_FactorNamesAreCorrect()
    {
        // Arrange
        decimal SD = 30m;
        decimal depth = 50m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.Contains(result.Factors, f => f.Item1 == "透明度");
        Assert.Contains(result.Factors, f => f.Item1 == "溶解氧");
        Assert.Contains(result.Factors, f => f.Item1 == "氨氮");
    }

    [Fact]
    public void GetBlackOdorousClass_FactorsListIsInitialized()
    {
        // Arrange
        decimal SD = 30m;
        decimal depth = 50m;
        decimal DO = 5.0m;
        decimal NH3N = 5.0m;
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        Assert.NotNull(result.Factors);
        Assert.IsType<List<(string, BlackOdorousClass)>>(result.Factors);
        Assert.True(result.Factors.Count <= 3);
    }

    [Fact]
    public void GetBlackOdorousClass_BlackOdorousClassMatchesMaxFactor()
    {
        // Arrange
        decimal SD = 15m;      // 轻度黑臭
        decimal depth = 50m;
        decimal DO = 0.1m;     // 重度黑臭
        decimal NH3N = 5.0m;   // 不黑臭
        bool isClear = false;

        // Act
        var result = CityService.GetBlackOdorousClass(SD, DO, NH3N, depth, isClear);

        // Assert
        var maxClass = result.Factors.Select(f => f.Item2).Max();
        Assert.Equal(maxClass, result.BlackOdorousClass);
        Assert.Equal(BlackOdorousClass.重度黑臭, result.BlackOdorousClass);
    }

    #endregion
}
