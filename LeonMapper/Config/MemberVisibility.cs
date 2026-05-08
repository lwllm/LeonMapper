namespace LeonMapper.Config;

/// <summary>
/// 成员可见性级别：控制映射器可访问的成员可见性范围
/// </summary>
public enum MemberVisibility
{
    /// <summary>仅公共成员（默认，推荐生产环境使用）</summary>
    Public = 0,

    /// <summary>公共和内部成员</summary>
    PublicAndInternal = 1,

    /// <summary>所有成员（包括 private，与旧版行为兼容）</summary>
    All = 2
}
