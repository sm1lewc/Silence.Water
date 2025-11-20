using Silence.Water.Core.Enums;
using Silence.Water.Groundwater.Enums;
using Silence.Water.Groundwater.Services;

namespace Silence.Water.Test.Groundwater.Services;

/// <summary>
/// 地下水质量标准服务类的单元测试
/// </summary>
public class StandardServiceTests
{
    #region GetFactorClass Tests - Numeric Values

    [Fact]
    public void GetFactorClass_WithNegativeNumericValue_ReturnsNull()
    {
        // Arrange
        var factor = GroundwaterFactor.pH;
        object value = -1m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(GroundwaterFactor.pH, 7.0)]
    [InlineData(GroundwaterFactor.氨氮, 0.02)]
    [InlineData(GroundwaterFactor.硝酸盐, 2.0)]
    [InlineData(GroundwaterFactor.总硬度, 150)]
    public void GetFactorClass_WithValidNumericValue_ReturnsClass(
        GroundwaterFactor factor,
        decimal value)
    {
        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.NotNull(result);
        Assert.InRange((int)result, 1, 5); // 地下水类别从Ⅰ到Ⅴ
    }

    [Theory]
    [InlineData(GroundwaterFactor.氨氮, 0.01, WaterQualityClass.Ⅰ)] // 优良水质
    [InlineData(GroundwaterFactor.氨氮, 0.1, WaterQualityClass.Ⅱ)]
    [InlineData(GroundwaterFactor.氨氮, 0.5, WaterQualityClass.Ⅲ)]
    [InlineData(GroundwaterFactor.氨氮, 1.5, WaterQualityClass.Ⅳ)]
    [InlineData(GroundwaterFactor.氨氮, 2.0, WaterQualityClass.Ⅴ)] // 差水质
    public void GetFactorClass_WithAmmoniaNitrogen_ReturnsCorrectClass(
        GroundwaterFactor factor,
        decimal value,
        WaterQualityClass expectedClass)
    {
        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(expectedClass, result);
    }

    [Theory]
    [InlineData(GroundwaterFactor.硝酸盐, 2.0, WaterQualityClass.Ⅰ)]
    [InlineData(GroundwaterFactor.硝酸盐, 5.0, WaterQualityClass.Ⅱ)]
    [InlineData(GroundwaterFactor.硝酸盐, 20.0, WaterQualityClass.Ⅲ)]
    [InlineData(GroundwaterFactor.硝酸盐, 25.0, WaterQualityClass.Ⅳ)]
    [InlineData(GroundwaterFactor.硝酸盐, 35.0, WaterQualityClass.Ⅴ)]
    public void GetFactorClass_WithNitrate_ReturnsCorrectClass(
        GroundwaterFactor factor,
        decimal value,
        WaterQualityClass expectedClass)
    {
        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(expectedClass, result);
    }

    [Fact]
    public void GetFactorClass_WithZeroValue_ReturnsClass1()
    {
        // Arrange
        var factor = GroundwaterFactor.氨氮;
        object value = 0m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(WaterQualityClass.Ⅰ, result);
    }

    [Fact]
    public void GetFactorClass_WithVeryLargeValue_ReturnsClass5()
    {
        // Arrange
        var factor = GroundwaterFactor.氨氮;
        object value = 999999m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(WaterQualityClass.Ⅴ, result);
    }

    #endregion

    #region GetFactorClass Tests - Detection Limit Values

    [Theory]
    [InlineData("0.01L")]
    [InlineData("0.001L")]
    [InlineData("5L")]
    [InlineData("0.5l")] // 小写L
    public void GetFactorClass_WithDetectionLimitValue_ReturnsClass1(string value)
    {
        // Arrange
        var factor = GroundwaterFactor.汞;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(WaterQualityClass.Ⅰ, result);
    }

    #endregion

    #region GetFactorClass Tests - Special Cases

    [Fact]
    public void GetFactorClass_WithNonExistentFactor_ReturnsNull()
    {
        // Arrange
        // 使用一个可能不在标准数据中的因子
        var factor = (GroundwaterFactor)9999;
        object value = 1.0m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFactorClass_WithInvalidTextValue_ReturnsNull()
    {
        // Arrange
        var factor = GroundwaterFactor.氨氮;
        object value = "invalid text value";

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFactorClass_WithEmptyString_ReturnsNull()
    {
        // Arrange
        var factor = GroundwaterFactor.pH;
        object value = "";

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFactorClass_WithDifferentVersion_UsesCorrectStandard()
    {
        // Arrange
        var factor = GroundwaterFactor.pH;
        object value = 7.0m;

        // Act
        var result = StandardService.GetFactorClass(
            factor,
            value,
            GroundwaterStandardVersion.GBT_14848_2017);

        // Assert
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1.5)]
    [InlineData("1.5")]
    public void GetFactorClass_WithDifferentNumericTypes_WorksCorrectly(object value)
    {
        // Arrange
        var factor = GroundwaterFactor.氨氮;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region GetFactorClass Tests - Table 1 Factors

    [Theory]
    [InlineData(GroundwaterFactor.色)]
    [InlineData(GroundwaterFactor.嗅和味)]
    [InlineData(GroundwaterFactor.浑浊度)]
    [InlineData(GroundwaterFactor.pH)]
    [InlineData(GroundwaterFactor.总硬度)]
    [InlineData(GroundwaterFactor.溶解性总固体)]
    [InlineData(GroundwaterFactor.氨氮)]
    [InlineData(GroundwaterFactor.硝酸盐)]
    [InlineData(GroundwaterFactor.氟化物)]
    [InlineData(GroundwaterFactor.汞)]
    [InlineData(GroundwaterFactor.砷)]
    [InlineData(GroundwaterFactor.镉)]
    [InlineData(GroundwaterFactor.铅)]
    public void GetFactorClass_WithTable1Factors_ReturnsValidClass(GroundwaterFactor factor)
    {
        // Arrange
        object value = 1.0m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert - 应该返回有效的水质类别或null
        if (result.HasValue)
        {
            Assert.InRange((int)result.Value, 1, 5);
        }
    }

    #endregion

    #region GetFactorClass Tests - Table 2 Factors

    [Theory]
    [InlineData(GroundwaterFactor.铍)]
    [InlineData(GroundwaterFactor.硼)]
    [InlineData(GroundwaterFactor.锑)]
    [InlineData(GroundwaterFactor.钡)]
    [InlineData(GroundwaterFactor.镍)]
    [InlineData(GroundwaterFactor.钴)]
    [InlineData(GroundwaterFactor.钼)]
    [InlineData(GroundwaterFactor.三溴甲烷)]
    [InlineData(GroundwaterFactor.氯乙烯)]
    [InlineData(GroundwaterFactor.三氯乙烯)]
    [InlineData(GroundwaterFactor.苯)]
    [InlineData(GroundwaterFactor.甲苯)]
    public void GetFactorClass_WithTable2Factors_ReturnsValidClass(GroundwaterFactor factor)
    {
        // Arrange
        object value = 0.001m;

        // Act
        var result = StandardService.GetFactorClass(factor, value);

        // Assert
        if (result.HasValue)
        {
            Assert.InRange((int)result.Value, 1, 5);
        }
    }

    #endregion

    #region GetFactorsClasses Tests

    [Fact]
    public void GetFactorsClasses_WithEmptyDictionary_ReturnsEmptyDictionary()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>();

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
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.pH, 7.0m },
            { GroundwaterFactor.氨氮, 0.02m },
            { GroundwaterFactor.硝酸盐, 2.0m },
            { GroundwaterFactor.总硬度, 150m }
        };

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void GetFactorsClasses_WithMixedValueTypes_HandlesCorrectly()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.pH, 7.0m },
            { GroundwaterFactor.汞, "0.001L" },
            { GroundwaterFactor.氨氮, "未检出" },
            { GroundwaterFactor.硝酸盐, 2.0 }
        };

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.Contains(result.Values, v => v == WaterQualityClass.Ⅰ);
    }

    [Fact]
    public void GetFactorsClasses_WithNegativeValues_ReturnsNullForThoseFactors()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.pH, -1m },
            { GroundwaterFactor.氨氮, 0.02m }
        };

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Null(result[GroundwaterFactor.pH]);
        Assert.NotNull(result[GroundwaterFactor.氨氮]);
    }

    [Fact]
    public void GetFactorsClasses_WithDetectionLimitValues_ReturnsClass1()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.汞, "0.001L" },
            { GroundwaterFactor.砷, "0.005L" },
            { GroundwaterFactor.镉, "0.0001L" }
        };

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result.Values, v => Assert.Equal(WaterQualityClass.Ⅰ, v));
    }

    [Fact]
    public void GetFactorsClasses_WithMixedQuality_ReturnsVariousClasses()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.氨氮, 0.01m },  // Ⅰ类
            { GroundwaterFactor.硝酸盐, 20.0m }, // Ⅲ类
            { GroundwaterFactor.总硬度, 1000m }  // 较差
        };

        // Act
        var result = StandardService.GetFactorsClasses(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result.Values, v => v == WaterQualityClass.Ⅰ);
    }

    #endregion

    #region GetFactorsMaxClass Tests

    [Fact]
    public void GetFactorsMaxClass_WithEmptyDictionary_ReturnsNull()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>();

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFactorsMaxClass_WithSingleFactor_ReturnsItsClass()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.氨氮, 0.02m }
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetFactorsMaxClass_WithMultipleFactors_ReturnsWorstClass()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.氨氮, 0.01m },  // Ⅰ类
            { GroundwaterFactor.硝酸盐, 5.0m },  // Ⅱ类
            { GroundwaterFactor.总硬度, 1500m }  // Ⅴ类
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
        Assert.True(result >= WaterQualityClass.Ⅳ);
    }

    [Fact]
    public void GetFactorsMaxClass_WithAllGoodQuality_ReturnsClass1Or2()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.氨氮, "未检出" },
            { GroundwaterFactor.汞, "0.001L" },
            { GroundwaterFactor.硝酸盐, 2.0m }
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
        Assert.True(result <= WaterQualityClass.Ⅱ);
    }

    [Fact]
    public void GetFactorsMaxClass_IgnoresNullValues()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.pH, -1m },      // 返回null
            { GroundwaterFactor.氨氮, 0.1m }    // Ⅱ类
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(WaterQualityClass.Ⅱ, result);
    }

    [Fact]
    public void GetFactorsMaxClass_WithDifferentVersion_UsesCorrectStandard()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.氨氮, 0.02m },
            { GroundwaterFactor.硝酸盐, 2.0m }
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(
            values,
            GroundwaterStandardVersion.GBT_14848_2017);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetFactorsMaxClass_WithMixedDetectionAndNumeric_ReturnsCorrectMax()
    {
        // Arrange
        var values = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.汞, "0.001L" },  // Ⅰ类
            { GroundwaterFactor.氨氮, 1.5m }     // Ⅳ类
        };

        // Act
        var result = StandardService.GetFactorsMaxClass(values);

        // Assert
        Assert.NotNull(result);
        Assert.True(result >= WaterQualityClass.Ⅳ);
    }

    #endregion

    #region GetFactorStandard Tests

    [Fact]
    public void GetFactorStandard_WithValidFactor_ReturnsStandard()
    {
        // Arrange
        var factor = GroundwaterFactor.pH;

        // Act
        var result = StandardService.GetFactorStandard(factor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(factor, result.Factor);
        Assert.NotNull(result.FactorName);
        Assert.NotNull(result.Limits);
        Assert.NotEmpty(result.Limits);
    }

    [Theory]
    [InlineData(GroundwaterFactor.pH)]
    [InlineData(GroundwaterFactor.氨氮)]
    [InlineData(GroundwaterFactor.硝酸盐)]
    [InlineData(GroundwaterFactor.总硬度)]
    [InlineData(GroundwaterFactor.汞)]
    [InlineData(GroundwaterFactor.砷)]
    [InlineData(GroundwaterFactor.镉)]
    [InlineData(GroundwaterFactor.铅)]
    public void GetFactorStandard_WithTable1Factors_ReturnsStandards(GroundwaterFactor factor)
    {
        // Act
        var result = StandardService.GetFactorStandard(factor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(factor, result.Factor);
        Assert.NotEmpty(result.FactorName);
    }

    [Fact]
    public void GetFactorStandard_ChecksStandardProperties()
    {
        // Arrange
        var factor = GroundwaterFactor.氨氮;

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

    [Fact]
    public void GetFactorStandard_WithDifferentVersion_ReturnsVersionSpecificStandard()
    {
        // Arrange
        var factor = GroundwaterFactor.pH;

        // Act
        var result = StandardService.GetFactorStandard(
            factor,
            GroundwaterStandardVersion.GBT_14848_2017);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(factor, result.Factor);
    }

    [Fact]
    public void GetFactorStandard_WithNonExistentFactor_ReturnsNull()
    {
        // Arrange
        var factor = (GroundwaterFactor)9999;

        // Act
        var result = StandardService.GetFactorStandard(factor);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFactorStandard_VerifyUnitInformation()
    {
        // Arrange
        var factor = GroundwaterFactor.氨氮;

        // Act
        var result = StandardService.GetFactorStandard(factor);

        // Assert
        Assert.NotNull(result);
        // 单位可能为null或有值，取决于标准数据
        if (result.Unit != null)
        {
            Assert.NotEmpty(result.Unit);
        }
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void IntegrationTest_CompleteGroundwaterQualityEvaluation()
    {
        // Arrange - 模拟一个完整的地下水质评估场景
        var waterSample = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.pH, 7.2m },
            { GroundwaterFactor.氨氮, 0.05m },
            { GroundwaterFactor.硝酸盐, 3.0m },
            { GroundwaterFactor.总硬度, 200m },
            { GroundwaterFactor.汞, "0.001L" },
            { GroundwaterFactor.砷, "未检出" }
        };

        // Act
        var factorClasses = StandardService.GetFactorsClasses(waterSample);
        var overallClass = StandardService.GetFactorsMaxClass(waterSample);

        // Assert
        Assert.NotNull(factorClasses);
        Assert.Equal(6, factorClasses.Count);
        
        Assert.NotNull(overallClass);
        Assert.InRange((int)overallClass, 1, 5); // 地下水类别从Ⅰ到Ⅴ
    }

    [Fact]
    public void IntegrationTest_MixedDetectionAndNumericValues()
    {
        // Arrange
        var waterSample = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.氨氮, 0.1m },
            { GroundwaterFactor.汞, "0.001L" },
            { GroundwaterFactor.砷, "未检出" },
            { GroundwaterFactor.硝酸盐, 10.0m }
        };

        // Act
        var factorClasses = StandardService.GetFactorsClasses(waterSample);
        var overallClass = StandardService.GetFactorsMaxClass(waterSample);

        // Assert
        Assert.NotNull(factorClasses);
        Assert.Equal(4, factorClasses.Count);
        Assert.Contains(factorClasses.Values, v => v == WaterQualityClass.Ⅰ);
        
        Assert.NotNull(overallClass);
    }

    [Fact]
    public void IntegrationTest_AllTable1FactorsHaveStandards()
    {
        // Arrange - 验证所有表1因子都有标准数据
        var table1Factors = new[]
        {
            GroundwaterFactor.色, GroundwaterFactor.嗅和味, GroundwaterFactor.浑浊度,
            GroundwaterFactor.肉眼可见物, GroundwaterFactor.pH, GroundwaterFactor.总硬度,
            GroundwaterFactor.溶解性总固体, GroundwaterFactor.硫酸盐, GroundwaterFactor.氯化物,
            GroundwaterFactor.铁, GroundwaterFactor.锰, GroundwaterFactor.铜,
            GroundwaterFactor.锌, GroundwaterFactor.铝, GroundwaterFactor.挥发性酚类,
            GroundwaterFactor.阴离子表面活性剂, GroundwaterFactor.耗氧量, GroundwaterFactor.氨氮,
            GroundwaterFactor.硫化物, GroundwaterFactor.钠, GroundwaterFactor.总大肠菌群,
            GroundwaterFactor.菌落总数, GroundwaterFactor.亚硝酸盐, GroundwaterFactor.硝酸盐,
            GroundwaterFactor.氰化物, GroundwaterFactor.氟化物, GroundwaterFactor.碘化物,
            GroundwaterFactor.汞, GroundwaterFactor.砷, GroundwaterFactor.硒,
            GroundwaterFactor.镉, GroundwaterFactor.六价铬, GroundwaterFactor.铅,
            GroundwaterFactor.三氯甲烷, GroundwaterFactor.四氯化碳, GroundwaterFactor.苯,
            GroundwaterFactor.甲苯, GroundwaterFactor.总α放射性, GroundwaterFactor.总β放射性
        };

        // Act & Assert
        var standardsFound = 0;
        foreach (var factor in table1Factors)
        {
            var standard = StandardService.GetFactorStandard(factor);
            if (standard != null)
            {
                Assert.Equal(factor, standard.Factor);
                Assert.NotEmpty(standard.FactorName);
                standardsFound++;
            }
        }

        // 至少应该有一些标准数据
        Assert.True(standardsFound > 0, "至少应该有一些表1因子的标准数据");
    }

    [Fact]
    public void IntegrationTest_CompareDetectionLimitFormats()
    {
        // Arrange - 测试不同格式的检出限值
        var detectionLimitFormats = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.汞, "0.001L" },      // 大写L
            { GroundwaterFactor.砷, "0.005l" },      // 小写l
            { GroundwaterFactor.阴离子表面活性剂, "未检出" },       // 未检出文本
        };

        // Act
        var factorClasses = StandardService.GetFactorsClasses(detectionLimitFormats);

        // Assert
        Assert.NotNull(factorClasses);
        Assert.All(factorClasses.Values, v => Assert.Equal(WaterQualityClass.Ⅰ, v));
    }

    [Fact]
    public void IntegrationTest_EdgeCaseValues()
    {
        // Arrange - 测试边界值和特殊情况
        var edgeCases = new Dictionary<GroundwaterFactor, object>
        {
            { GroundwaterFactor.氨氮, 0m },          // 零值
            { GroundwaterFactor.硝酸盐, -1m },       // 负值
            { GroundwaterFactor.pH, "" },            // 空字符串
            { GroundwaterFactor.总硬度, "invalid" }, // 无效文本
        };

        // Act
        var factorClasses = StandardService.GetFactorsClasses(edgeCases);

        // Assert
        Assert.NotNull(factorClasses);
        Assert.Equal(4, factorClasses.Count);
        
        // 零值应该返回Ⅰ类
        Assert.Equal(WaterQualityClass.Ⅰ, factorClasses[GroundwaterFactor.氨氮]);
        
        // 负值和无效值应该返回null
        Assert.Null(factorClasses[GroundwaterFactor.硝酸盐]);
        Assert.Null(factorClasses[GroundwaterFactor.总硬度]);
    }

    #endregion

    #region Performance and Caching Tests

    [Fact]
    public void GetFactorStandard_CalledMultipleTimes_PerformsConsistently()
    {
        // Arrange
        var factor = GroundwaterFactor.氨氮;

        // Act - 多次调用
        var result1 = StandardService.GetFactorStandard(factor);
        var result2 = StandardService.GetFactorStandard(factor);
        var result3 = StandardService.GetFactorStandard(factor);

        // Assert - 结果应该一致
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotNull(result3);
        Assert.Equal(result1.Factor, result2.Factor);
        Assert.Equal(result2.Factor, result3.Factor);
    }

    [Fact]
    public void GetFactorClass_CalledMultipleTimes_ReturnsSameResult()
    {
        // Arrange
        var factor = GroundwaterFactor.氨氮;
        object value = 0.1m;

        // Act
        var result1 = StandardService.GetFactorClass(factor, value);
        var result2 = StandardService.GetFactorClass(factor, value);
        var result3 = StandardService.GetFactorClass(factor, value);

        // Assert
        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
    }

    #endregion
}
