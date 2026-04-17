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

## 架构概览

### 入口点
`Mapper<TIn, TOut>` (LeonMapper/Mapper.cs) 是公共 API，泛型类 `TOut : class` 约束。

### 处理器（核心映射引擎）
两种策略实现 `IProcessor<in TInput, out TOutput>`：

1. **ExpressionProcessor** (Processors/ExpressionProcessor/) - **主要实现**
   - 静态构造时编译表达式树为 `Func<TInput, TOutput>`
   - 支持同名属性/字段、复杂类型递归映射、不同类型自动转换

2. **EmitProcessor** (Processors/EmitProcessor/) - **未完成**
   - 仅创建目标对象，属性/字段复制 IL 代码已注释
   - 需要参数less构造器

### AbstractProcessor 基类
- `GetPropertyDictionary()` - 反射发现属性映射关系，支持 `[MapTo]`、`[MapFrom]`
- `GetFieldDictionary()` - 公共字段映射（仅名称匹配）
- `CheckCanMap()` - 检查 `[IgnoreMap]`、`[IgnoreMapTo]`、`[IgnoreMapFrom]`

### 属性系统 (Attributes/)
- `MapToAttribute(string)` - 源成员映射到不同名称目标
- `MapFromAttribute(string)` - 目标成员从不同名称源拉取（优先级高于 MapTo）
- `IgnoreMapAttribute` - 通用忽略
- `IgnoreMapToAttribute` - 源成员禁止映射出
- `IgnoreMapFromAttribute` - 目标成员禁止映射入

### 类型转换系统 (Convert/)
- `IConverter<in TInput, out TOutput>` - 单方法接口
- `ConvertFactory` - 自动发现所有转换器实现，按 `"InputType|OutputType"` 键服务
- `[CommonConverter]` 标记安全/常用转换器
- 15 个转换器文件覆盖所有 C# 基元类型

### 全局配置 (Config/MapperConfig.cs)
```csharp
MapperConfig.DefaultProcessType      // Expression 或 Emit（默认 Expression）
MapperConfig.ConverterScope          // CommonConverters 或 AllConverters
MapperConfig.AutoConvert             // 是否自动转换不同类型（默认 true）
```

## 开发注意事项

- **主库零依赖**：纯 .NET BCL（System.Reflection、System.Reflection.Emit、System.Linq.Expressions）
- **复杂类型映射缓存**：`Constants.COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY` 存储递归映射器
- **EmitProcessor 状态**：未完成，修改时需补充属性/字段 IL 生成
- **测试包含性能基准**：与 AutoMapper 对比（100 万次迭代）

## 代码位置参考

| 组件 | 文件 |
|------|------|
| 公共 API | LeonMapper/Mapper.cs |
| 处理器接口 | LeonMapper/Processors/IProcessor.cs |
| 处理器基类 | LeonMapper/Processors/AbstractProcessor.cs |
| Expression引擎 | LeonMapper/Processors/ExpressionProcessor/ExpressionProcessor.cs |
| Emit引擎 | LeonMapper/Processors/EmitProcessor/EmitProcessor.cs |
| 转换器工厂 | LeonMapper/Convert/ConvertFactory.cs |
| 配置 | LeonMapper/Config/MapperConfig.cs |