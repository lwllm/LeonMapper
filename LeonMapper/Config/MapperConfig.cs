using System.Collections.Concurrent;
using LeonMapper.Plan;
using LeonMapper.Utils;

namespace LeonMapper.Config;

/// <summary>
/// 全局映射配置：控制默认编译策略、转换器范围和自动转换行为
/// </summary>
/// <remarks>
/// 本类使用线程安全的数据结构存储配置，支持在多线程环境中安全地读取和修改配置。
/// 对于 ASP.NET 等多租户场景，建议在应用启动时一次性设置配置。
///
/// <b>线程安全说明</b>：
/// - 单个配置项的读取/写入是线程安全的（使用 volatile + lock）
/// - 但跨多个配置项的原子读取（如同时读取 ProcessType 和 ConverterScope）
///   不保证一致性。如果需要严格的多字段一致性，请使用 <see cref="SetConfiguration"/> 方法
///   设置所有字段，并在读取时确保没有并发修改
/// </remarks>
public class MapperConfig
{
    // 使用volatile确保多线程可见性
    private static volatile ProcessTypeEnum _defaultProcessType = ProcessTypeEnum.Expression;
    private static volatile ConverterScope _defaultConverterScope = ConverterScope.Common;
    private static volatile bool _autoConverter = true;

    // 用于更细粒度配置的线程安全字典（未来扩展用）
    private static readonly ConcurrentDictionary<string, object?> _customSettings = new();

    // 锁对象，用于需要原子操作的场景
    private static readonly object _configLock = new();

    /// <summary>
    /// 设置默认编译策略
    /// </summary>
    /// <param name="defaultProcessType">编译策略类型</param>
    public static void SetDefaultProcessType(ProcessTypeEnum defaultProcessType)
    {
        lock (_configLock)
        {
            _defaultProcessType = defaultProcessType;
        }
    }

    /// <summary>
    /// 设置最大映射深度，防止循环引用导致 StackOverflow。
    /// 超出此深度时 MapTo 返回 default。默认值 100。
    /// </summary>
    public static void SetMaxDepth(int maxDepth)
    {
        MappingDepthTracker.SetMaxDepth(maxDepth);
    }

    /// <summary>
    /// 获取当前最大映射深度
    /// </summary>
    public static int GetMaxDepth()
    {
        return MappingDepthTracker.GetMaxDepth();
    }

    /// <summary>
    /// 设置默认转换器查找范围
    /// </summary>
    /// <param name="converterScope">转换器查找范围</param>
    public static void SetConverterScope(ConverterScope converterScope)
    {
        lock (_configLock)
        {
            _defaultConverterScope = converterScope;
        }
    }

    /// <summary>
    /// 设置是否自动转换不同基础类型
    /// </summary>
    /// <param name="autoConvert">是否自动转换</param>
    public static void SetAutoConvert(bool autoConvert)
    {
        lock (_configLock)
        {
            _autoConverter = autoConvert;
        }
    }

    /// <summary>
    /// 获取默认编译策略
    /// </summary>
    /// <returns>当前配置的编译策略</returns>
    public static ProcessTypeEnum GetDefaultProcessType()
    {
        return _defaultProcessType;
    }

    /// <summary>
    /// 获取默认转换器查找范围
    /// </summary>
    /// <returns>当前配置的转换器查找范围</returns>
    public static ConverterScope GetDefaultConverterScope()
    {
        return _defaultConverterScope;
    }

    /// <summary>
    /// 获取是否自动转换不同基础类型
    /// </summary>
    /// <returns>是否启用自动转换</returns>
    public static bool GetAutoConvert()
    {
        return _autoConverter;
    }

    /// <summary>
    /// 原子性地设置多个配置项
    /// </summary>
    /// <param name="processType">编译策略</param>
    /// <param name="converterScope">转换器范围</param>
    /// <param name="autoConvert">自动转换</param>
    public static void SetConfiguration(ProcessTypeEnum processType, ConverterScope converterScope, bool autoConvert)
    {
        lock (_configLock)
        {
            _defaultProcessType = processType;
            _defaultConverterScope = converterScope;
            _autoConverter = autoConvert;
        }
    }

    /// <summary>
    /// 设置自定义配置项
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    public static void SetCustomSetting(string key, object? value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (value == null)
        {
            _customSettings.TryRemove(key, out _);
        }
        else
        {
            _customSettings[key] = value;
        }
    }

    /// <summary>
    /// 获取自定义配置项
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>配置值，不存在时返回 null</returns>
    public static object? GetCustomSetting(string key)
    {
        _customSettings.TryGetValue(key, out var value);
        return value;
    }
}
