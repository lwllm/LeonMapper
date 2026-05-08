using System;

namespace LeonMapper.Utils;

/// <summary>
/// 映射深度追踪器：检测循环引用，防止 StackOverflow。
/// 使用 AsyncLocal 实现线程安全，支持 async/await 场景。
/// </summary>
internal static class MappingDepthTracker
{
    private static readonly AsyncLocal<int> _depth = new();
    private static volatile int _maxDepth = 100;

    /// <summary>
    /// 当映射深度超出限制时触发（可用于日志记录）
    /// </summary>
    internal static event Action<Type, Type, int>? OnDepthOverflow;

    /// <summary>
    /// 设置最大映射深度。超出此深度时 MapTo 将返回 default。
    /// </summary>
    public static void SetMaxDepth(int maxDepth)
    {
        _maxDepth = Math.Max(1, maxDepth);
    }

    /// <summary>
    /// 获取当前最大映射深度
    /// </summary>
    public static int GetMaxDepth() => _maxDepth;

    /// <summary>
    /// 尝试递增深度。如果已超过最大深度，返回 false 并保持深度不变。
    /// </summary>
    public static bool TryIncrement()
    {
        if (_depth.Value >= _maxDepth)
        {
            return false;
        }

        _depth.Value++;
        return true;
    }

    /// <summary>
    /// 递减深度。必须在 TryIncrement 返回 true 后配对调用。
    /// </summary>
    public static void Decrement()
    {
        if (_depth.Value > 0)
        {
            _depth.Value--;
        }
    }

    /// <summary>
    /// 重置当前上下文的深度（用于测试）
    /// </summary>
    public static void Reset()
    {
        _depth.Value = 0;
    }

    /// <summary>
    /// 触发深度溢出事件（由 Mapper.MapTo 在返回 default 前调用）
    /// </summary>
    internal static void NotifyOverflow(Type sourceType, Type targetType)
    {
        OnDepthOverflow?.Invoke(sourceType, targetType, _maxDepth);
    }
}
