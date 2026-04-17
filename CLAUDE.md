# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

LeonMapper 是一个 C# 对象映射库，类似 AutoMapper，通过编译表达式树或 IL 生成实现高性能的对象到对象映射。目标框架：.NET 6.0。

## 常用命令

### 构建
```bash
dotnet build LeonMapper.sln
dotnet build LeonMapper/LeonMapper.csproj
```

### 运行测试
```bash
dotnet test LeonMapper.Test/LeonMapper.Test.csproj
# 运行单个测试
dotnet test LeonMapper.Test/LeonMapper.Test.csproj --filter "FullyQualifiedName~MapperTest"
```

### 运行演示
```bash
dotnet run --project LeonMapper.Demos/LeonMapper.Demos.csproj
```

## 架构概览（v2 重构后）

### 分层架构

```
公共 API 层 (Mapper.cs)
    ↓
映射计划层 (Plan/)
    ↓
编译引擎层 (Compilers/)
    ↓
类型发现层 (Plan/Builder/)
    ↓
类型转换层 (Convert/)
    ↓
配置层 (Config/)
```

### 入口点
`Mapper<TIn, TOut>` (LeonMapper/Mapper.cs) 是公共 API，泛型类 `TOut : class` 约束。

### 映射计划层 (Plan/)
核心抽象模型，将反射发现的映射规则抽象为可查询、可验证的数据结构：
- `TypeMappingPlan` - 源类型到目标类型的完整映射规则
- `MemberMapping` - 单个成员的映射规则（源→目标、策略、转换器）
- `MappingStrategy` - 映射策略枚举（Direct/Convert/Complex）
- `PlanBuildOptions` - 映射计划构建选项

### 映射计划构建器 (Plan/Builder/)
- `MappingPlanBuilder` - 从源类型和目标类型生成 TypeMappingPlan
  - 反射扫描属性/字段
  - 解析 MapTo/MapFrom/Ignore 等注解
  - 为每个成员映射确定策略（Direct/Convert/Complex）
  - 查找转换器、构建嵌套计划

### 编译引擎层 (Compilers/)
两种编译策略实现 `ICompiler<in TSource, out TTarget>`：

1. **ExpressionCompiler** (Compilers/ExpressionCompiler.cs) - **主要实现**
   - 从 TypeMappingPlan 编译表达式树为 `Func<TSource, TTarget>`
   - 支持同名属性/字段、复杂类型递归映射、不同类型自动转换

2. **EmitCompiler** (Compilers/EmitCompiler.cs) - **完整实现**
   - 从 TypeMappingPlan 通过 IL Emit 生成映射委托
   - 支持属性/字段的 Direct/Convert/Complex 三种策略

3. **CachedMapperFactory** - 缓存复杂类型的映射器实例和委托

### 属性系统 (Attributes/)
- `MapToAttribute(string)` - 源成员映射到不同名称目标（支持多个，替代同名映射）
- `MapFromAttribute(string)` - 目标成员从不同名称源拉取（优先级高于 MapTo）
- `IgnoreMapAttribute` - 通用忽略
- `IgnoreMapToAttribute` - 源成员禁止映射出
- `IgnoreMapFromAttribute` - 目标成员禁止映射入

### 类型转换系统 (Convert/)
- `IConverter<in TInput, out TOutput>` - 单方法接口
- `ConvertFactory` - 自动发现所有转换器实现，支持 Common/All 两种范围查找
- `[CommonConverter]` 标记安全/常用转换器
- 15 个转换器文件覆盖所有 C# 基元类型

### 验证系统 (Validation/)
- `MappingValidator` - 验证映射计划，检测未映射字段、类型转换可行性
- `ValidationResult` - 验证结果，包含错误和警告列表

### 全局配置 (Config/MapperConfig.cs)
```csharp
MapperConfig.SetDefaultProcessType(ProcessTypeEnum.Expression);  // Expression 或 Emit
MapperConfig.SetConverterScope(ConverterScopeEnum.CommonConverters);
MapperConfig.SetAutoConvert(true);
```

## 公共 API 使用

```csharp
// 简单用法（向后兼容）
var mapper = new Mapper<User, UserNew>();
var result = mapper.MapTo(user);

// 指定编译策略
var emitMapper = new Mapper<User, UserNew>(ProcessTypeEnum.Emit);
var result2 = emitMapper.MapTo(user);

// 查看映射计划
var plan = Mapper<User, UserNew>.GetPlan();
Console.WriteLine(plan.ToString());

// 验证映射配置
var validation = Mapper<User, UserNew>.Validate();
if (!validation.IsValid)
    Console.WriteLine(validation.GetReport());
```

## 开发注意事项

- **主库零依赖**：纯 .NET BCL
- **旧代码保留**：Processors/ 目录下的旧实现保留但不再使用，可安全删除
- **映射优先级**：MapFrom > MapTo > 同名匹配；MapTo 会替代同名映射（源属性不会同时映射到 MapTo 目标和同名目标）
- **测试包含性能基准**：与 AutoMapper 对比（1 亿次迭代）

## 代码位置参考

| 组件 | 文件 |
|------|------|
| 公共 API | LeonMapper/Mapper.cs |
| 映射计划模型 | LeonMapper/Plan/TypeMappingPlan.cs |
| 成员映射模型 | LeonMapper/Plan/MemberMapping.cs |
| 计划构建器 | LeonMapper/Plan/Builder/MappingPlanBuilder.cs |
| 表达式编译器 | LeonMapper/Compilers/ExpressionCompiler.cs |
| IL 编译器 | LeonMapper/Compilers/EmitCompiler.cs |
| 编译器接口 | LeonMapper/Compilers/ICompiler.cs |
| 缓存工厂 | LeonMapper/Compilers/CachedMapperFactory.cs |
| 转换器工厂 | LeonMapper/Convert/ConvertFactory.cs |
| 映射验证器 | LeonMapper/Validation/MappingValidator.cs |
| 配置 | LeonMapper/Config/MapperConfig.cs |
