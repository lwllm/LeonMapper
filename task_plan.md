# Task Plan: LeonMapper 开发计划

## Goal
为 LeonMapper 对象映射库制定并执行后续开发计划，提升功能完整性、测试覆盖率和项目成熟度。

## Current Phase
Phase 3

## Phases

### Phase 1: 现状评估与需求确认
- [x] 探索项目当前架构和功能完成度
- [x] 识别测试覆盖缺口
- [x] 对比 AutoMapper 识别功能差距
- [x] 与用户确认优先级和开发方向
- **Status:** completed

### Phase 2: 升级目标框架到 .NET 8.0
- [x] 修改 LeonMapper.csproj 目标框架为 net8.0
- [x] 修改 LeonMapper.Test.csproj 目标框架为 net8.0
- [x] 检查并修复 .NET 8.0 下的编译警告
- [x] 运行全部测试确保通过
- **Status:** completed

### Phase 3: 补齐测试覆盖
- [ ] 为 MapFrom 属性注解添加测试
- [ ] 为 IgnoreMapFrom 注解添加测试
- [ ] 为 MapperConfig 配置变更添加测试
- [ ] 为 Validate() 映射验证添加测试
- [ ] 为 GetPlan() 映射计划查询添加测试
- [ ] 为自定义转换器注册添加测试
- [ ] 为 NoEmptyConstructorException 添加测试
- **Status:** in_progress

### Phase 4: 修复文档与代码不一致
- [ ] 修复 README 中 MapTo 重载描述错误
- [ ] 更新 README 中 Emit 引擎状态描述
- [ ] 补充 XML 文档注释（如有缺失）
- **Status:** pending

### Phase 5: 核心功能增强（按优先级选择）
- [ ] 实现 Fluent API 配置（类似 AutoMapper 的 CreateMap）
- [ ] 实现 ForMember 自定义映射
- [ ] ~~实现循环引用处理（PreserveReferences）~~ 【延后】
- [ ] 实现 Enum 类型映射支持
- [ ] 实现 Dictionary 映射支持
- **Status:** pending

### Phase 6: 性能与工程化
- [ ] 引入 BenchmarkDotNet 专业基准测试
- [ ] 添加内存分配/GC 压力测试
- [ ] 评估并修复潜在内存泄漏（DelegateInvoker 静态字典）
- **Status:** pending

### Phase 7: 交付与总结
- [ ] 运行全部测试确保通过
- [ ] 更新 README 和文档
- [ ] 总结变更并交付
- **Status:** pending

## Key Questions
1. ✅ 优先补齐测试覆盖（已确认）
2. ✅ 升级到 .NET 8.0（已确认）
3. ❌ 暂时不需要循环引用处理（已确认）
4. 是否需要准备 NuGet 包发布？（待确认）

## Decisions Made
| Decision | Rationale |
|----------|-----------|
| 优先补齐测试覆盖 | 用户确认，先稳固现有代码质量 |
| 升级到 .NET 8.0 | 用户确认，利用新特性和性能改进 |
| 循环引用处理延后 | 用户确认，当前场景不涉及 |
| 先不实现 Fluent API | 测试补齐后再进行功能增强 |

## Errors Encountered
| Error | Attempt | Resolution |
|-------|---------|------------|
| 无 | - | - |

## Notes
- 项目核心架构（Plan -> Builder -> Compiler）设计清晰，v2 重构质量高
- 双引擎（Expression + Emit）实现完整，集合映射和可空类型支持是亮点
- 最紧迫的缺口：MapFrom 测试、Validate 测试、MapperConfig 测试
- 与 AutoMapper 最大功能差距：Fluent API、ForMember 自定义映射、循环引用处理
