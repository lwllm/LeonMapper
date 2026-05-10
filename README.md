# LeonMapper

[![NuGet](https://img.shields.io/nuget/v/LeonMapper)](https://www.nuget.org/packages/LeonMapper/)
[![License](https://img.shields.io/github/license/luowuliang/LeonMapper)](LICENSE)

一个高性能的 C# 对象映射库，通过编译期生成映射代码实现接近手写映射的性能。零依赖、双引擎、完整功能覆盖。

## 安装

```bash
dotnet add package LeonMapper
```

## 快速开始

同名属性自动映射：

```csharp
using LeonMapper;

var mapper = new Mapper<User, UserDto>();
var dto = mapper.MapTo(user);
```

## 特性

- **双引擎** — Expression Tree 和 Reflection.Emit 两套引擎，功能完全一致
- **零依赖** — 纯 .NET BCL，无任何第三方依赖
- **高性能** — 编译期生成映射代码，接近手写映射性能
- **类型自动转换** — 内置所有 C# 基元类型互转，默认 `CultureInfo.InvariantCulture`
- **嵌套对象映射** — 自动递归处理复杂类型属性，支持 3 层以上深度
- **集合映射** — `List<T>`、数组、`IEnumerable<T>` 之间的互转
- **字典映射** — `Dictionary<K,V>` 之间 Key/Value 类型转换
- **枚举映射** — 枚举 ↔ 枚举、枚举 ↔ 基元类型互转
- **可空类型** — `int?` ↔ `long?`、`int?` ↔ `int` 等全组合
- **循环引用保护** — 三层防护，默认最大深度 100，可配置
- **Fluent API** — `ForMember().MapFrom()`、`Ignore()`、`ConvertUsing<T>()`
- **属性注解** — `[MapTo]`、`[MapFrom]`、`[IgnoreMap]` 等控制映射行为
- **非泛型 API** — `IMapper` 接口 + `MapperService`，支持 DI 注入
- **字段映射** — 公共字段自动映射

## 详细文档

### 基础映射

```csharp
var mapper = new Mapper<User, UserDto>();
var dto = mapper.MapTo(user);
```

### 指定引擎

```csharp
// Expression 引擎（默认）
var mapper = new Mapper<User, UserDto>(ProcessTypeEnum.Expression);

// Emit 引擎
var emitMapper = new Mapper<User, UserDto>(ProcessTypeEnum.Emit);
```

### 嵌套对象映射

复杂类型属性自动递归映射，支持多层深度：

```csharp
public class Order
{
    public int Id { get; set; }
    public Customer Customer { get; set; }  // 自动映射为 CustomerDto
    public List<OrderItem> Items { get; set; }  // 自动映射为 List<OrderItemDto>
}

var mapper = new Mapper<Order, OrderDto>();
var dto = mapper.MapTo(order);
```

### 集合映射

`List<T>`、数组、`IEnumerable<T>` 之间的任意组合映射：

```csharp
var source = new List<User> { ... };
var mapper = new Mapper<List<User>, List<UserDto>>();
var result = mapper.MapTo(source);  // 所有元素自动映射
```

### 字典映射

```csharp
public class Source { public string Name { get; set; } }
public class Target { public string Name { get; set; } }

var source = new Dictionary<int, Source> { { 1, new Source { Name = "A" } } };
var mapper = new Mapper<Dictionary<int, Source>, Dictionary<long, Target>>();
var result = mapper.MapTo(source);  // Key 和 Value 自动转换
```

### 属性注解

| 注解 | 作用 | 使用位置 |
|------|------|---------|
| `[MapTo("TargetProp")]` | 源映射到不同名称的目标 | 源属性 |
| `[MapFrom("SourceProp")]` | 目标从不同名称的源拉取（优先于 MapTo） | 目标属性 |
| `[IgnoreMap]` | 完全忽略映射 | 源或目标属性 |
| `[IgnoreMapTo]` | 源禁止映射到目标 | 源属性 |
| `[IgnoreMapFrom]` | 目标禁止接收映射 | 目标属性 |

```csharp
public class Employee
{
    [MapTo(nameof(EmployeeDto.DisplayName))]
    public string Name { get; set; }

    [IgnoreMap]
    public string Password { get; set; }
}

public class EmployeeDto
{
    public string DisplayName { get; set; }
    public string Password { get; set; }  // 不会被映射，保持默认值
}
```

### Fluent API

```csharp
var mapper = new Mapper<Employee, EmployeeDto>(cfg =>
{
    cfg.ForMember(dest => dest.DisplayName, opt =>
    {
        opt.MapFrom(src => src.FullName);
    });

    cfg.ForMember(dest => dest.CalculatedField, opt =>
    {
        opt.Ignore();
    });

    cfg.ForMember(dest => dest.FormattedValue, opt =>
    {
        opt.ConvertUsing<IntToStringConverter>();
    });
});
```

### DI 注入（非泛型 API）

```csharp
// 注册
services.AddSingleton<IMapper, MapperService>();

// 使用
public class UserService
{
    private readonly IMapper _mapper;

    public UserService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public UserDto GetUser(int id)
    {
        var user = _repository.GetUser(id);
        return _mapper.Map<UserDto>(user);
    }
}
```

### 全局配置

```csharp
using LeonMapper.Config;

// 编译引擎（默认 Expression）
MapperConfig.SetDefaultProcessType(ProcessTypeEnum.Emit);

// 转换器范围（默认 Common）
MapperConfig.SetConverterScope(ConverterScope.All);

// 自动类型转换（默认 true）
MapperConfig.SetAutoConvert(true);

// 最大递归深度（默认 100，防循环引用）
MapperConfig.SetMaxDepth(50);

// 成员可见性（默认 Public，设为 All 可映射私有成员）
MapperConfig.SetMemberVisibility(MemberVisibility.Public);
```

### 验证映射配置

```csharp
var validation = Mapper<User, UserDto>.Validate();
if (!validation.IsValid)
    Console.WriteLine(validation.GetReport());
```

### 查看映射计划

```csharp
var plan = Mapper<User, UserDto>.GetPlan();
Console.WriteLine(plan);
```

## 循环引用保护

当对象图存在循环引用（如 `Employee.Manager` 指向自身）时，默认最大深度为 100 层，超出返回 `null`。可通过 `MapperConfig.SetMaxDepth()` 调整或监听 `MappingDepthTracker.OnDepthOverflow` 事件记录日志。

## 注意事项

- 目标类型必须有公共无参构造函数（`where TDestination : class`）
- 默认仅映射公共属性/字段，可通过 `MemberVisibility.All` 启用私有成员映射
- 映射器实例是线程安全的，可全局共享
- 计划缓存和映射器缓存仅在进程内有效，可通过 `CachedMapperFactory.ClearCache()` 清理

## Benchmark

```bash
dotnet run --project LeonMapper.Benchmarks/LeonMapper.Benchmarks.csproj -c Release
```

## 测试

```bash
dotnet test LeonMapper.Test/LeonMapper.Test.csproj
```

## License

[MIT](LICENSE)
