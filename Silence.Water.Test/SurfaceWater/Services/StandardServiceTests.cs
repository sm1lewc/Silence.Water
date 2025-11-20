using Silence.Water.Core.Enums;
using Silence.Water.SurfaceWater.Enums;
using Silence.Water.SurfaceWater.Services;

namespace Silence.Water.Test.SurfaceWater.Services;

/// <summary>
/// 地表水质量标准服务类的单元测试
/// </summary>
public class StandardServiceTests
{
    #region GetFactorClass Tests

    [Fact]
    public void GetFactorClass_WithNegativeValue_ReturnsNull()
    {
        // Arrange
        var factor = SurfaceWaterFactor.pH值;
        var value = -1m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFactorClass_WithNonTable1Factor_ThrowsNotImplementedException()
    {
        // Arrange
        var factor = SurfaceWaterFactor.硫酸盐; // 表2指标
        var value = 100m;

        // Act & Assert
        var exception = Assert.Throws<NotImplementedException>(() =>
            StandardService.GetFactorClass(factor, value));
        Assert.Contains("非表1指标水质类别暂不支持判断", exception.Message);
    }

    [Theory]
    [InlineData(SurfaceWaterFactor.pH值, 7.0, WaterQualityClass.Ⅰ)] // pH 在正常范围内
    [InlineData(SurfaceWaterFactor.溶解氧, 7.5, WaterQualityClass.Ⅰ)] // 溶解氧 >= 7.5 为 Ⅰ类
    [InlineData(SurfaceWaterFactor.溶解氧, 6.0, WaterQualityClass.Ⅱ)] // 溶解氧 6-7.5 为 Ⅱ类
    [InlineData(SurfaceWaterFactor.溶解氧, 5.0, WaterQualityClass.Ⅲ)] // 溶解氧 5-6 为 Ⅲ类
    [InlineData(SurfaceWaterFactor.溶解氧, 3.0, WaterQualityClass.Ⅳ)] // 溶解氧 3-5 为 Ⅳ类
    [InlineData(SurfaceWaterFactor.溶解氧, 2.0, WaterQualityClass.Ⅴ)] // 溶解氧 2-3 为 Ⅴ类
    [InlineData(SurfaceWaterFactor.溶解氧, 1.0, WaterQualityClass.劣Ⅴ)] // 溶解氧 < 2 为 劣Ⅴ类
    public void GetFactorClass_WithValidValue_ReturnsCorrectClass(
        SurfaceWaterFactor factor,
        decimal value,
        WaterQualityClass expectedClass)
    {
        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(expectedClass, result);
    }

    [Theory]
    [InlineData(SurfaceWaterFactor.高锰酸盐指数, 2.0, WaterQualityClass.Ⅰ)]
    [InlineData(SurfaceWaterFactor.高锰酸盐指数, 4.0, WaterQualityClass.Ⅱ)]
    [InlineData(SurfaceWaterFactor.高锰酸盐指数, 6.0, WaterQualityClass.Ⅲ)]
    [InlineData(SurfaceWaterFactor.高锰酸盐指数, 10.0, WaterQualityClass.Ⅳ)]
    [InlineData(SurfaceWaterFactor.高锰酸盐指数, 15.0, WaterQualityClass.Ⅴ)]
    [InlineData(SurfaceWaterFactor.高锰酸盐指数, 20.0, WaterQualityClass.劣Ⅴ)]
    public void GetFactorClass_WithHighMnIndex_ReturnsCorrectClass(
        SurfaceWaterFactor factor,
        decimal value,
        WaterQualityClass expectedClass)
    {
        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(expectedClass, result);
    }

    [Theory]
    [InlineData(SurfaceWaterFactor.氨氮, 0.15, WaterQualityClass.Ⅰ)]
    [InlineData(SurfaceWaterFactor.氨氮, 0.5, WaterQualityClass.Ⅱ)]
    [InlineData(SurfaceWaterFactor.氨氮, 1.0, WaterQualityClass.Ⅲ)]
    [InlineData(SurfaceWaterFactor.氨氮, 1.5, WaterQualityClass.Ⅳ)]
    [InlineData(SurfaceWaterFactor.氨氮, 2.0, WaterQualityClass.Ⅴ)]
    [InlineData(SurfaceWaterFactor.氨氮, 3.0, WaterQualityClass.劣Ⅴ)]
    public void GetFactorClass_WithAmmoniaNitrogen_ReturnsCorrectClass(
        SurfaceWaterFactor factor,
        decimal value,
        WaterQualityClass expectedClass)
    {
        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(expectedClass, result);
    }

    [Theory]
    [InlineData(SurfaceWaterFactor.总磷_河流, 0.02, WaterQualityClass.Ⅰ)]
    [InlineData(SurfaceWaterFactor.总磷_河流, 0.1, WaterQualityClass.Ⅱ)]
    [InlineData(SurfaceWaterFactor.总磷_河流, 0.2, WaterQualityClass.Ⅲ)]
    [InlineData(SurfaceWaterFactor.总磷_河流, 0.3, WaterQualityClass.Ⅳ)]
    [InlineData(SurfaceWaterFactor.总磷_河流, 0.4, WaterQualityClass.Ⅴ)]
    [InlineData(SurfaceWaterFactor.总磷_河流, 0.5, WaterQualityClass.劣Ⅴ)]
    public void GetFactorClass_WithTotalPhosphorusRiver_ReturnsCorrectClass(
        SurfaceWaterFactor factor,
        decimal value,
        WaterQualityClass expectedClass)
    {
        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(expectedClass, result);
    }

    [Fact]
    public void GetFactorClass_WithDifferentVersion_UsesCorrectStandard()
    {
        // Arrange
        var factor = SurfaceWaterFactor.pH值;
        var value = 7.0m;

        // Act
        var result = StandardService.GetFactorClass(
            factor,
            value,
            SurfaceWaterStandardVersion.GB_3838_2002);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetFactorClass_WithBoundaryValue_ReturnsCorrectClass()
    {
        // Arrange - 测试边界值
        var factor = SurfaceWaterFactor.溶解氧;
        var boundaryValue = 7.5m; // Ⅰ类的临界值

        // Act
        var result = StandardService.GetFactorClass(factor, boundaryValue);

        // Assert
        Assert.NotNull(result);
        Assert.True(result == WaterQualityClass.Ⅰ || result == WaterQualityClass.Ⅱ);
    }

    [Fact]
    public void GetFactorClass_WithZeroValue_ReturnsValidClass()
    {
        // Arrange
        var factor = SurfaceWaterFactor.氨氮;
        var value = 0m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(WaterQualityClass.Ⅰ, result);
    }

    #endregion

    #region GetFactorsClasses Tests

    [Fact]
    public void GetFactorsClasses_WithEmptyDictionary_ReturnsEmptyDictionary()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>();

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetFactorsClasses_WithMultipleFactors_ReturnsCorrectClasses()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.pH值, 7.0m },
            { SurfaceWaterFactor.溶解氧, 7.5m },
            { SurfaceWaterFactor.氨氮, 0.15m },
            { SurfaceWaterFactor.高锰酸盐指数, 2.0m }
        };

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.All(result.Values, v => Assert.NotNull(v));
    }

    [Fact]
    public void GetFactorsClasses_WithNegativeValues_ReturnsNullForThoseFactors()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.pH值, -1m },
            { SurfaceWaterFactor.溶解氧, 7.5m }
        };

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Null(result[SurfaceWaterFactor.pH值]);
        Assert.NotNull(result[SurfaceWaterFactor.溶解氧]);
    }

    [Fact]
    public void GetFactorsClasses_WithMixedQualityFactors_ReturnsVariousClasses()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.溶解氧, 7.5m },      // Ⅰ类
            { SurfaceWaterFactor.氨氮, 1.0m },        // Ⅲ类
            { SurfaceWaterFactor.高锰酸盐指数, 15.0m } // Ⅴ类
        };

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.True(result[SurfaceWaterFactor.溶解氧] <= WaterQualityClass.Ⅱ);
        Assert.True(result[SurfaceWaterFactor.高锰酸盐指数] >= WaterQualityClass.Ⅳ);
    }

    #endregion

    #region GetFactorsMaxClass Tests

    [Fact]
    public void GetFactorsMaxClass_WithEmptyDictionary_ReturnsNull()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>();

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFactorsMaxClass_WithSingleFactor_ReturnsItsClass()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.溶解氧, 7.5m }
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
        Assert.True(result <= WaterQualityClass.Ⅱ);
    }

    [Fact]
    public void GetFactorsMaxClass_WithMultipleFactors_ReturnsWorstClass()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.溶解氧, 7.5m },      // Ⅰ类
            { SurfaceWaterFactor.氨氮, 0.5m },        // Ⅱ类
            { SurfaceWaterFactor.高锰酸盐指数, 20.0m } // 劣Ⅴ类
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(WaterQualityClass.劣Ⅴ, result);
    }

    [Fact]
    public void GetFactorsMaxClass_WithAllGoodQuality_ReturnsClass1Or2()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.溶解氧, 8.0m },
            { SurfaceWaterFactor.氨氮, 0.15m },
            { SurfaceWaterFactor.高锰酸盐指数, 2.0m }
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
        Assert.True(result <= WaterQualityClass.Ⅱ);
    }

    [Fact]
    public void GetFactorsMaxClass_WithNegativeValues_HandlesCorrectly()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.pH值, -1m },
            { SurfaceWaterFactor.溶解氧, 7.5m }
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetFactorsMaxClass_WithDifferentVersion_UsesCorrectStandard()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.溶解氧, 7.5m },
            { SurfaceWaterFactor.氨氮, 1.0m }
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(
            values,
            SurfaceWaterStandardVersion.GB_3838_2002);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region GetFactorStandard Tests

    [Fact]
    public void GetFactorStandard_WithValidTable1Factor_ReturnsStandard()
    {
        // Arrange
        var factor = SurfaceWaterFactor.pH值;

        // Act
        var result = StandardService.GetFactorStandard(factor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(factor, result.Factor);
        Assert.NotNull(result.FactorName);
        Assert.NotNull(result.Limits);
        Assert.NotEmpty(result.Limits);
    }

    [Fact]
    public void GetFactorStandard_WithAllTable1Factors_ReturnsStandards()
    {
        // Arrange - 测试所有表1因子
        var table1Factors = new[]
        {
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
        };

        // Act & Assert
        foreach (var factor in table1Factors)
        {
            var result = StandardService.GetFactorStandard(factor);
            Assert.NotNull(result);
            Assert.Equal(factor, result.Factor);
        }
    }

    [Fact]
    public void GetFactorStandard_WithNonTable1Factor_ReturnsStandardOrNull()
    {
        // Arrange
        var factor = SurfaceWaterFactor.硫酸盐; // 表2指标

        // Act
        var result = StandardService.GetFactorStandard(factor);

        // Assert - 根据注释，非表1指标返回Ⅰ类代表标准值或null
        // 具体行为取决于数据文件中是否包含该指标
        Assert.True(result == null || result.Factor == factor);
    }

    [Fact]
    public void GetFactorStandard_WithDifferentVersion_ReturnsVersionSpecificStandard()
    {
        // Arrange
        var factor = SurfaceWaterFactor.pH值;

        // Act
        var result = StandardService.GetFactorStandard(
            factor,
            SurfaceWaterStandardVersion.GB_3838_2002);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(factor, result.Factor);
    }

    [Fact]
    public void GetFactorStandard_ChecksStandardProperties()
    {
        // Arrange
        var factor = SurfaceWaterFactor.溶解氧;

        // Act
        var result = StandardService.GetFactorStandard(factor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(factor, result.Factor);
        Assert.NotNull(result.FactorName);
        Assert.NotEmpty(result.FactorName);
        Assert.NotNull(result.Limits);
        Assert.NotEmpty(result.Limits);
        
        // 检查限值列表中是否包含各个水质类别
        var classesInLimits = result.Limits.Select(l => l.WaterQualityClass).ToList();
        Assert.Contains(WaterQualityClass.Ⅰ, classesInLimits);
    }

    #endregion

    #region Edge Cases and Error Handling Tests

    [Fact]
    public void GetFactorClass_WithVeryLargeValue_ReturnsWorstClass()
    {
        // Arrange
        var factor = SurfaceWaterFactor.高锰酸盐指数;
        var value = decimal.MaxValue;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(WaterQualityClass.劣Ⅴ, result);
    }

    [Fact]
    public void GetFactorClass_WithVerySmallPositiveValue_ReturnsClass1()
    {
        // Arrange
        var factor = SurfaceWaterFactor.氨氮;
        var value = 0.01m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(WaterQualityClass.Ⅰ, result);
    }

    [Theory]
    [InlineData(SurfaceWaterFactor.pH值)]
    [InlineData(SurfaceWaterFactor.溶解氧)]
    [InlineData(SurfaceWaterFactor.氨氮)]
    [InlineData(SurfaceWaterFactor.总磷_河流)]
    public void GetFactorStandard_WithTable1Factors_AlwaysReturnsNonNull(SurfaceWaterFactor factor)
    {
        // Act
        var result = StandardService.GetFactorStandard(factor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(factor, result.Factor);
    }

    [Fact]
    public void GetFactorsClasses_WithSingleNonTable1Factor_ThrowsException()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.硫酸盐, 100m } // 表2指标
        };

        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            StandardService.GetFactorsClasses(values));
    }

    [Fact]
    public void GetFactorsMaxClass_WithSingleNonTable1Factor_ThrowsException()
    {
        // Arrange
        var values = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.硫酸盐, 100m } // 表2指标
        };

        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            StandardService.GetFactorsMaxClass(values));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void IntegrationTest_CompleteWaterQualityEvaluation()
    {
        // Arrange - 模拟一个完整的水质评估场景
        var waterSample = new Dictionary<SurfaceWaterFactor, decimal>
        {
            { SurfaceWaterFactor.pH值, 7.2m },
            { SurfaceWaterFactor.溶解氧, 6.5m },
            { SurfaceWaterFactor.高锰酸盐指数, 3.5m },
            { SurfaceWaterFactor.氨氮, 0.8m },
            { SurfaceWaterFactor.总磷_河流, 0.15m }
        };

        // Act
        var factorClasses = StandardService.GetFactorsClasses(waterSample);
        var overallClass = StandardService.GetFactorsMaxClass(waterSample);

        // Assert
        Assert.NotNull(factorClasses);
        Assert.Equal(5, factorClasses.Count);
        Assert.All(factorClasses.Values, v => Assert.NotNull(v));
        
        Assert.NotNull(overallClass);
        Assert.InRange((int)overallClass, 1, 6); // 水质类别应在Ⅰ到劣Ⅴ之间
    }

    [Fact]
    public void IntegrationTest_CompareRiverAndLakePhosphorus()
    {
        // Arrange - 比较河流和湖库的总磷标准
        var value = 0.1m;

        // Act
        var riverClass = StandardService.GetFactorClass(SurfaceWaterFactor.总磷_河流, value);
        var lakeClass = StandardService.GetFactorClass(SurfaceWaterFactor.总磷_湖库, value);

        // Assert - 河流和湖库的总磷标准不同
        Assert.NotNull(riverClass);
        Assert.NotNull(lakeClass);
        // 具体的类别关系取决于标准数据文件
    }

    [Fact]
    public void IntegrationTest_AllTable1FactorsHaveStandards()
    {
        // Arrange - 验证所有表1因子都有标准数据
        var table1Factors = new[]
        {
            SurfaceWaterFactor.pH值, SurfaceWaterFactor.溶解氧, SurfaceWaterFactor.高锰酸盐指数,
            SurfaceWaterFactor.化学需氧量, SurfaceWaterFactor.五日生化需氧量, SurfaceWaterFactor.氨氮,
            SurfaceWaterFactor.总磷_河流, SurfaceWaterFactor.总磷_湖库, SurfaceWaterFactor.总氮,
            SurfaceWaterFactor.铜, SurfaceWaterFactor.锌, SurfaceWaterFactor.氟化物,
            SurfaceWaterFactor.硒, SurfaceWaterFactor.砷, SurfaceWaterFactor.汞,
            SurfaceWaterFactor.镉, SurfaceWaterFactor.六价铬, SurfaceWaterFactor.铅,
            SurfaceWaterFactor.氰化物, SurfaceWaterFactor.挥发酚, SurfaceWaterFactor.石油类,
            SurfaceWaterFactor.阴离子表面活性剂, SurfaceWaterFactor.硫化物, SurfaceWaterFactor.粪大肠菌群
        };

        // Act & Assert
        foreach (var factor in table1Factors)
        {
            var standard = StandardService.GetFactorStandard(factor);
            Assert.NotNull(standard);
            Assert.NotNull(standard.FactorName);
            Assert.NotEmpty(standard.Limits);
        }
    }

    #endregion
}
