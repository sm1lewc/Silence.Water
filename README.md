# Silence.Water

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/github/license/sm1lewc/Silence.Water)](https://github.com/sm1lewc/Silence.Water/blob/master/LICENSE)

一个用于水质标准判定和评估的 .NET 库，支持地表水、地下水质量标准及黑臭水体评估。

## ✨ 功能特性

### 📊 地表水质量标准（Surface Water）
- 支持 GB 3838-2002《地表水环境质量标准》
- 自动判定水质类别（Ⅰ类、Ⅱ类、Ⅲ类、Ⅳ类、Ⅴ类、劣Ⅴ类）
- 支持多项指标的综合评估
- 支持 24 项基本指标的水质类别判定

### 💧 地下水质量标准（Groundwater）
- 支持 GB/T 14848-2017《地下水质量标准》
- 完整的地下水质量因子评估体系

### 🌊 黑臭水体评估（Black Odorous Water）
- 支持城市黑臭水体评估
- 基于透明度（SD）、溶解氧（DO）、氨氮（NH₃-N）指标
- 自动判定黑臭等级：不黑臭、轻度黑臭、重度黑臭

### 🚀 其他
- 使用内存缓存提高查询性能
- 标准数据以嵌入式资源形式存储，无需外部文件依赖
- 支持多版本标准切换

## 📦 安装

### 使用 NuGet 包管理器
```bash
dotnet add package Silence.Water
```

### 使用 Package Manager Console
```powershell
Install-Package Silence.Water
```

## 🔧 快速开始

### 1. 地表水质量类别判定

```csharp
using Silence.Water.SurfaceWater.Services;
using Silence.Water.SurfaceWater.Enums;

// 单个指标判定
var ph = 7.5m;
var phClass = StandardService.GetFactorClass(SurfaceWaterFactor.pH值, ph);
Console.WriteLine($"pH值为 {ph} 时，水质类别为：{phClass}");

// 多个指标综合判定
var waterQualityData = new Dictionary<SurfaceWaterFactor, decimal>
{
    { SurfaceWaterFactor.pH值, 7.5m },
    { SurfaceWaterFactor.溶解氧, 6.0m },
    { SurfaceWaterFactor.高锰酸盐指数, 4.5m },
    { SurfaceWaterFactor.氨氮, 0.8m },
    { SurfaceWaterFactor.总磷_河流, 0.15m }
};

// 获取各指标的水质类别
var factorClasses = StandardService.GetFactorsClasses(waterQualityData);
foreach (var (factor, waterClass) in factorClasses)
{
    Console.WriteLine($"{factor}: {waterClass}");
}

// 获取最差水质类别（综合水质）
var maxClass = StandardService.GetFactorsMaxClass(waterQualityData);
Console.WriteLine($"综合水质类别：{maxClass}");
```

### 2. 黑臭水体评估

```csharp
using Silence.Water.BlackOdorous.Services;

// 城市黑臭水体评估
var evaluateResult = CityService.Evaluate(
    SD: 15m,        // 透明度 15cm
    DO: 1.5m,       // 溶解氧 1.5 mg/L
    NH3N: 5.0m,     // 氨氮 5.0 mg/L
    ORP: -50m       // 氧化还原电位 -50 mV
);

Console.WriteLine($"黑臭等级：{evaluateResult.Class}");
Console.WriteLine($"透明度指标：{evaluateResult.SDClass}");
Console.WriteLine($"溶解氧指标：{evaluateResult.DOClass}");
Console.WriteLine($"氨氮指标：{evaluateResult.NH3NClass}");
Console.WriteLine($"氧化还原电位指标：{evaluateResult.ORPClass}");
```

### 3. 获取标准限值信息

```csharp
using Silence.Water.SurfaceWater.Services;
using Silence.Water.SurfaceWater.Enums;

// 获取某个因子的标准信息
var doStandard = StandardService.GetFactorStandard(SurfaceWaterFactor.溶解氧);
if (doStandard != null)
{
    Console.WriteLine($"指标名称：{doStandard.FactorName}");
    Console.WriteLine($"单位：{doStandard.Unit}");
    Console.WriteLine($"描述：{doStandard.Description}");
    
    foreach (var limit in doStandard.Limits)
    {
        Console.WriteLine($"{limit.WaterQualityClass}: {limit.LimitValue} {doStandard.Unit}");
    }
}
```

## 📖 API 文档

### 地表水质量标准服务 (StandardService)

#### 主要方法

| 方法 | 说明 | 返回值 |
|------|------|--------|
| `GetFactorClass(factor, value, version)` | 获取单个因子的水质类别 | `WaterQualityClass?` |
| `GetFactorsClasses(values, version)` | 获取多个因子的水质类别字典 | `Dictionary<SurfaceWaterFactor, WaterQualityClass?>` |
| `GetFactorsMaxClass(values, version)` | 获取最差水质类别（综合评估） | `WaterQualityClass?` |
| `GetFactorStandard(factor, version)` | 获取因子的标准限值信息 | `SurfaceWaterStandard?` |

#### 支持的地表水因子

- pH值、溶解氧、高锰酸盐指数、化学需氧量、五日生化需氧量
- 氨氮、总磷（河流/湖库）、总氮
- 重金属：铜、锌、硒、砷、汞、镉、六价铬、铅
- 其他：氟化物、氰化物、挥发酚、石油类、阴离子表面活性剂、硫化物、粪大肠菌群

### 黑臭水体评估服务 (CityService)

#### 主要方法

| 方法 | 说明 | 返回值 |
|------|------|--------|
| `Evaluate(SD, DO, NH3N, ORP)` | 综合评估黑臭水体等级 | `EvaluateResult` |

#### 评估参数

- `SD` - 透明度 (cm)
- `DO` - 溶解氧 (mg/L)
- `NH3N` - 氨氮 (mg/L)
- `ORP` - 氧化还原电位 (mV)

### 水质类别枚举 (WaterQualityClass)

```csharp
public enum WaterQualityClass
{
    Ⅰ = 1,
    Ⅱ = 2,
    Ⅲ = 3,
    Ⅳ = 4,
    Ⅴ = 5,
    劣Ⅴ = 6
}
```

### 黑臭等级枚举 (BlackOdorousClass)

```csharp
public enum BlackOdorousClass
{
    不黑臭 = 0,
    轻度黑臭 = 1,
    重度黑臭 = 2
}
```

## 🔬 技术栈

- **.NET 8.0** - 目标框架
- **System.Text.Json** - JSON 序列化/反序列化
- **xUnit** - 单元测试框架

## 📂 项目结构

```
Silence.Water/
├── Core/                          # 核心功能
│   ├── Enums/                     # 通用枚举
│   ├── Models/                    # 通用模型
│   └── Services/                  # 通用服务
├── SurfaceWater/                  # 地表水模块
│   ├── Data/                      # 标准数据文件
│   ├── Enums/                     # 地表水枚举
│   ├── Models/                    # 地表水模型
│   └── Services/                  # 地表水服务
├── Groundwater/                   # 地下水模块
│   ├── Data/                      # 标准数据文件
│   ├── Enums/                     # 地下水枚举
│   ├── Models/                    # 地下水模型
│   └── Services/                  # 地下水服务
└── BlackOdorous/                  # 黑臭水体模块
    ├── Enums/                     # 黑臭水体枚举
    ├── Models/                    # 黑臭水体模型
    └── Services/                  # 黑臭水体服务
```

## 📝 标准依据

- GB 3838-2002《地表水环境质量标准》
- GB/T 14848-2017《地下水质量标准》
- 《城市黑臭水体整治工作指南》

## 🧪 测试

项目包含完整的单元测试，使用 xUnit 测试框架。

```bash
# 运行所有测试
dotnet test

# 运行测试并生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage"
```

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 许可证

本项目采用 MIT 许可证。详见 [LICENSE](LICENSE) 文件。

## 👨‍💻 作者

- GitHub: [@sm1lewc](https://github.com/sm1lewc)

## 🔗 相关链接

- [GitHub 仓库](https://github.com/sm1lewc/Silence.Water)
- [问题反馈](https://github.com/sm1lewc/Silence.Water/issues)

---

如果这个项目对您有帮助，请给一个 ⭐️ Star！