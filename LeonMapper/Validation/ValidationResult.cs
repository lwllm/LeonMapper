using System.Reflection;
using System.Text;
using LeonMapper.Plan;

namespace LeonMapper.Validation;

/// <summary>
/// 映射验证结果
/// </summary>
public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public IReadOnlyList<string> Errors { get; }
    public IReadOnlyList<string> Warnings { get; }

    public ValidationResult(IReadOnlyList<string> errors, IReadOnlyList<string> warnings)
    {
        Errors = errors;
        Warnings = warnings;
    }

    public string GetReport()
    {
        var sb = new StringBuilder();
        if (Errors.Count > 0)
        {
            sb.AppendLine("Errors:");
            foreach (var error in Errors)
                sb.AppendLine($"  - {error}");
        }

        if (Warnings.Count > 0)
        {
            sb.AppendLine("Warnings:");
            foreach (var warning in Warnings)
                sb.AppendLine($"  - {warning}");
        }

        if (IsValid && Warnings.Count == 0)
            sb.AppendLine("All mappings are valid.");

        return sb.ToString();
    }
}
