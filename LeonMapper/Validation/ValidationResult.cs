using System.Text;

namespace LeonMapper.Validation;

/// <summary>
/// 映射验证结果：包含错误和警告信息
/// </summary>
public class ValidationResult
{
    /// <summary>映射配置是否有效（无错误）</summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>错误列表</summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>警告列表</summary>
    public IReadOnlyList<string> Warnings { get; }

    /// <summary>
    /// 构造验证结果
    /// </summary>
    public ValidationResult(IReadOnlyList<string> errors, IReadOnlyList<string> warnings)
    {
        Errors = errors;
        Warnings = warnings;
    }

    /// <summary>
    /// 生成可读的验证报告
    /// </summary>
    public string GetReport()
    {
        var sb = new StringBuilder();
        if (Errors.Count > 0)
        {
            sb.AppendLine("Errors:");
            foreach (var error in Errors)
            {
                sb.AppendLine($"  - {error}");
            }
        }

        if (Warnings.Count > 0)
        {
            sb.AppendLine("Warnings:");
            foreach (var warning in Warnings)
            {
                sb.AppendLine($"  - {warning}");
            }
        }

        if (IsValid && Warnings.Count == 0)
        {
            sb.AppendLine("All mappings are valid.");
        }

        return sb.ToString();
    }
}
