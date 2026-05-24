namespace LeonMapper.Compilers;

/// <summary>
/// 映射器缓存统计信息（通过 CachedMapperFactory.GetCacheStatistics() 获取）
/// </summary>
public sealed class CacheStatistics
{
    /// <summary>缓存的 Mapper 实例数量</summary>
    public int MapperCount { get; }

    /// <summary>缓存的映射委托数量</summary>
    public int MapFuncCount { get; }

    /// <summary>缓存的空计划映射器数量（用于自引用类型保护）</summary>
    public int EmptyMapperCount { get; }

    /// <summary>缓存的映射计划数量</summary>
    public int PlanCacheCount { get; }

    internal CacheStatistics(
        int mapperCount,
        int mapFuncCount,
        int emptyMapperCount,
        int planCacheCount)
    {
        MapperCount = mapperCount;
        MapFuncCount = mapFuncCount;
        EmptyMapperCount = emptyMapperCount;
        PlanCacheCount = planCacheCount;
    }
}
