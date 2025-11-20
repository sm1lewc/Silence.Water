using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Silence.Water.Core.Services;

/// <summary>
/// 水质标准缓存和加载的通用帮助类
/// </summary>
/// <typeparam name="TVersion">标准版本枚举</typeparam>
/// <typeparam name="TFactor">因子枚举</typeparam>
/// <typeparam name="TStandard">标准模型类</typeparam>
internal class StandardCacheManager<TVersion, TFactor, TStandard>
    where TVersion : Enum
    where TFactor : Enum
{
    private readonly Func<string, string> _resourceNameBuilder;
    private readonly Assembly _standardsAssembly;

    private readonly ConcurrentDictionary<TVersion, Dictionary<TFactor, TStandard>> _cachedStandards = new();

    /// <summary>
    /// 构造一个新的缓存管理器
    /// </summary>
    /// <param name="resourceNameBuilder">一个函数，用于根据版本字符串构建完整的嵌入式资源名称</param>
    public StandardCacheManager(Func<string, string> resourceNameBuilder)
    {
        _resourceNameBuilder = resourceNameBuilder;
        _standardsAssembly = Assembly.GetExecutingAssembly();
    }

    /// <summary>
    /// 获取指定版本的标准字典
    /// </summary>
    public Dictionary<TFactor, TStandard> GetStandardsForVersion(TVersion version)
    {
        return _cachedStandards.GetOrAdd(version, LoadStandardsFromDataFile);
    }

    /// <summary>
    /// 从嵌入式资源文件加载并反序列化水质标准数据
    /// </summary>
    private Dictionary<TFactor, TStandard> LoadStandardsFromDataFile(TVersion version)
    {
        var versionString = version.ToString().ToUpper();
        var resourceName = _resourceNameBuilder(versionString);

        using var stream = _standardsAssembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new NotSupportedException($"标准数据文件未找到: {resourceName}");

        using var reader = new StreamReader(stream);
        var jsonContent = reader.ReadToEnd();

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true,
        };

        var standards = JsonSerializer.Deserialize<Dictionary<TFactor, TStandard>>(jsonContent, options);
        return standards ?? new Dictionary<TFactor, TStandard>();
    }
}