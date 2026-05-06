namespace LeonMapper.Plan;

/// <summary>
/// 映射策略类型
/// </summary>
public enum MappingStrategy
{
    /// <summary>同类型直接赋值</summary>
    Direct,

    /// <summary>基础类型转换</summary>
    Convert,

    /// <summary>复杂类型递归映射</summary>
    Complex,

    /// <summary>集合类型映射（List、Array、IEnumerable 等）</summary>
    Collection,

    /// <summary>Dictionary 类型映射（Dictionary&lt;K,V&gt; 等）</summary>
    Dictionary
}
