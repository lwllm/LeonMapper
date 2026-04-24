# Findings & Decisions

## Requirements
- 用户要求检查当前项目并计划接下来的开发计划
- 需要基于项目现状制定合理的开发路线图

## Research Findings

### 项目架构现状
- 分层架构清晰：公共 API 层 -> 映射计划层 -> 编译引擎层 -> 类型转换层 -> 配置层
- 双编译引擎完整：ExpressionCompiler（主要）+ EmitCompiler（完整实现）
- 类型转换系统完善：16 个转换器文件覆盖所有 C# 基元类型
- 属性注解系统完整：MapTo、MapFrom、IgnoreMap、IgnoreMapTo、IgnoreMapFrom

### 测试覆盖缺口（重要）
| 缺失测试 | 重要程度 |
|----------|---------|
| MapFrom 属性注解 | 高 |
| IgnoreMapFrom 注解 | 中 |
| MapperConfig 配置变更 | 高 |
| Validate() 映射验证 | 高 |
| GetPlan() 映射计划查询 | 中 |
| 自定义转换器注册 | 中 |
| NoEmptyConstructorException | 中 |
| 多线程并发映射 | 中 |

### 与 AutoMapper 功能差距
| 功能 | 重要程度 |
|------|---------|
| Fluent API 配置 | 高 |
| ForMember 自定义映射 | 高 |
| Value Resolver | 高 |
| 循环引用处理 (PreserveReferences) | 高 |
| Queryable Extensions (ProjectTo) | 高 |
| Enum 映射 | 中 |
| Dictionary 映射 | 中 |
| ReverseMap 双向映射 | 中 |
| BeforeMap / AfterMap | 中 |

### 代码质量问题
1. README 中 `MapTo(user, ProcessTypeEnum.Emit)` 调用方式与实际代码不符
2. README 中 Emit 引擎标注为"开发中"，实际已实现
3. `ConvertFactory.GetTheBaseTypeConverter` 标记 [Obsolete] 但仍保留
4. DelegateInvoker 静态字典无清理机制，长期运行可能内存泄漏
5. ConvertFactory 使用非线程安全 Dictionary（当前无并发写入问题）

### 性能基准现状
- 使用 Stopwatch 手动计时，非专业基准测试框架
- 已对比：手写映射 vs LeonMapper (Expression) vs AutoMapper，1 亿次迭代
- 已对比：Expression vs Emit 编译器性能
- 缺失：BenchmarkDotNet、内存分配测试、GC 压力测试

## Technical Decisions
| Decision | Rationale |
|----------|-----------|
| 待用户确认 | 待确认开发优先级 |

## Issues Encountered
| Issue | Resolution |
|-------|------------|
| 无 | - |

## Resources
- 项目根目录: D:\Projects\Personnel\LeonMapper
- 主库项目: LeonMapper/LeonMapper.csproj
- 测试项目: LeonMapper.Test/LeonMapper.Test.csproj
- 公共 API: LeonMapper/Mapper.cs
- 表达式编译器: LeonMapper/Compilers/ExpressionCompiler.cs
- IL 编译器: LeonMapper/Compilers/EmitCompiler.cs
- 计划构建器: LeonMapper/Plan/Builder/MappingPlanBuilder.cs

## Visual/Browser Findings
- 无
