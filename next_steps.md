# LeonMapper 后续工作计划

> 本文档记录 Phase 2-4 完成后的后续开发计划，按优先级排序。
> 最后更新：2026-04-24

---

## 已完成工作回顾

### Phase 2: 升级目标框架到 .NET 8.0 ✅
- 目标框架已升级到 net8.0
- 修复 26 个 nullable 编译警告
- 50 → 89 个测试全部通过

### Phase 3: 补齐测试覆盖 ✅
- 新增 5 个测试文件，39 个新测试
- 覆盖：MapFrom/IgnoreMapFrom、MapperConfig、Validate/GetPlan、ConvertFactory、NoEmptyConstructorException

### Phase 4: 修复文档与代码不一致 ✅
- 修复 README 中 ConverterScope 枚举名错误
- 更新 Emit 引擎状态为"已完成"
- 修正指定引擎的调用方式
- 确认 XML 文档注释完整

---

## Phase 5: 核心功能增强（建议优先级）

### 5.1 Enum 类型映射支持（优先级：高）
**现状**：Enum 类型之间无法自动映射

**需求**：
- 同类型 Enum 直接赋值
- 不同 Enum 类型之间按名称映射（如 `StatusEnum.Active` → `StatusDtoEnum.Active`）
- Enum 与基础类型之间的转换（如 `int` ↔ `Enum`）

**涉及文件**：
- `LeonMapper/Plan/Builder/MappingPlanBuilder.cs` - CreateMemberMapping 方法中添加 Enum 判断
- `LeonMapper/Convert/` - 可能需要新增 Enum 转换器

**测试要求**：
- 同类型 Enum 映射
- 不同名称 Enum 按名称映射
- Enum ↔ int/string 转换
- 标记 [IgnoreMap] 的 Enum 字段

---

### 5.2 Dictionary 映射支持（优先级：中）
**现状**：Dictionary 类型未被识别为集合类型，无法映射

**需求**：
- `Dictionary<TKey, TValue>` → `Dictionary<TKey, TValueNew>` 的键值对映射
- `IDictionary<TKey, TValue>` 接口支持

**涉及文件**：
- `LeonMapper/Utils/TypeUtils.cs` - IsCollectionType / GetCollectionElementType 需要支持 Dictionary
- `LeonMapper/Plan/Builder/MappingPlanBuilder.cs` - 集合映射逻辑需要支持 Dictionary
- `LeonMapper/Compilers/ExpressionCompiler.cs` - Dictionary 的表达式生成
- `LeonMapper/Compilers/EmitCompiler.cs` - Dictionary 的 IL 生成

**测试要求**：
- Dictionary<string, int> → Dictionary<string, string>（值类型转换）
- Dictionary<string, Role> → Dictionary<string, RoleNew>（值类型复杂映射）
- IDictionary 接口映射

---

### 5.3 Fluent API 配置（优先级：高，但工作量大）
**现状**：仅支持属性注解配置，无法运行时动态配置

**需求**：
- 类似 AutoMapper 的 `CreateMap<TSource, TTarget>()`
- `ForMember(dest => dest.Prop, opt => opt.MapFrom(src => src.OtherProp))`
- 运行时配置映射关系，替代部分属性注解功能

**涉及文件**：
- 新增 `LeonMapper/Fluent/` 目录
- 新增 `MappingConfiguration` 类
- 新增 `IMappingExpression<TSource, TTarget>` 接口
- 修改 `MappingPlanBuilder` 支持从 Fluent 配置构建计划

**测试要求**：
- 基本 ForMember 配置
- 忽略字段配置
- 自定义转换器配置
- Fluent 配置与属性注解的优先级

**工作量评估**：大（预计 2-3 天）

---

### 5.4 ForMember 自定义映射（优先级：高，依赖 Fluent API）
**现状**：无法对单个成员指定自定义映射逻辑

**需求**：
- 支持表达式树定义自定义映射：`opt.MapFrom(src => src.A + src.B)`
- 支持常量映射：`opt.MapFrom("constant")`
- 支持条件映射：`opt.Condition(src => src.A > 0)`

**涉及文件**：
- 依赖 Fluent API 基础设施
- `ExpressionCompiler` 需要支持自定义表达式嵌入
- `EmitCompiler` 需要支持自定义委托调用

**工作量评估**：大（预计 2-3 天，与 Fluent API 一起实现）

---

### 5.5 ReverseMap 双向映射（优先级：中）
**现状**：需要手动创建两个 Mapper 实例

**需求**：
- `CreateMap<A, B>().ReverseMap()` 自动生成 B→A 的映射

**涉及文件**：
- 依赖 Fluent API 基础设施

**工作量评估**：中（依赖 Fluent API）

---

## Phase 6: 性能与工程化

### 6.1 引入 BenchmarkDotNet（优先级：中）
**现状**：使用 Stopwatch 手动计时，非专业基准测试

**需求**：
- 添加 BenchmarkDotNet 项目
- 对比：手写映射 vs LeonMapper (Expression) vs LeonMapper (Emit) vs AutoMapper
- 测试场景：简单对象、嵌套对象、集合映射

**涉及文件**：
- 新增 `LeonMapper.Benchmarks/` 项目

**工作量评估**：小（预计 0.5 天）

---

### 6.2 内存分配/GC 压力测试（优先级：中）
**现状**：无内存分配测试

**需求**：
- 使用 BenchmarkDotNet 的 MemoryDiagnoser
- 检测映射过程中的堆分配
- 对比 Expression 和 Emit 编译器的内存分配差异

**工作量评估**：小（与 BenchmarkDotNet 一起）

---

### 6.3 评估 DelegateInvoker 静态字典内存泄漏（优先级：中）
**现状**：`DelegateInvoker._converters` 是静态字典，只增不减

**需求**：
- 分析长期运行场景下的内存增长
- 考虑使用 `ConditionalWeakTable` 或 LRU 缓存替代

**涉及文件**：
- `LeonMapper/Compilers/EmitCompiler.cs` - DelegateInvoker 类

**工作量评估**：小（预计 0.5 天）

---

## Phase 7: 交付与总结

### 7.1 运行全部测试确保通过
### 7.2 更新 README 和文档
### 7.3 总结变更并交付

---

## 决策记录

| 决策 | 状态 | 说明 |
|------|------|------|
| 循环引用处理 (PreserveReferences) | ❌ 延后 | 用户确认当前场景不涉及 |
| 优先补齐测试覆盖 | ✅ 已完成 | Phase 3 已完成 |
| 升级到 .NET 8.0 | ✅ 已完成 | Phase 2 已完成 |
| Fluent API | ⏳ 待开始 | 建议作为 Phase 5 重点 |
| Enum 映射 | ⏳ 待开始 | 建议优先于 Dictionary |
| BenchmarkDotNet | ⏳ 待开始 | Phase 6 |

---

## 推荐的执行顺序

1. **Enum 类型映射支持**（1-2 天）
   - 工作量适中，用户场景常见
   - 不需要大规模架构改动

2. **Dictionary 映射支持**（1-2 天）
   - 在 Enum 之后进行
   - 涉及集合映射逻辑扩展

3. **Fluent API + ForMember**（3-5 天）
   - 最大的功能增强
   - 建议单独作为一个迭代周期

4. **BenchmarkDotNet + 内存测试**（1 天）
   - 在功能稳定后引入

5. **DelegateInvoker 内存优化**（0.5 天）
   - 作为收尾优化

---

## 技术债务

| 问题 | 严重程度 | 说明 |
|------|---------|------|
| 字段 MapFrom 未实现 | 低 | 属性级别已实现，字段级别未实现 |
| ConvertFactory 只扫描主程序集 | 低 | 测试程序集中的自定义转换器无法自动注册 |
| DelegateInvoker 静态字典无清理 | 中 | 长期运行可能内存泄漏 |

---

*本计划应根据实际开发进度和用户反馈动态调整。*
