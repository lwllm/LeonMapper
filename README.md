# LeonMapper

一个高性能的 C# 对象映射库，通过编译期生成映射代码实现接近手写映射的性能。类似 AutoMapper，但更轻量、更快速。

## 特性

- **双引擎实现** — 提供 Expression Tree（表达式树）和 Reflection.Emit（IL 生成）两套映射引擎
- **零依赖** — 纯 .NET BCL 实现，无任何第三方依赖
- **高性能** — 编译期生成映射代码，性能接近手写赋值
- **类型自动转换** — 内置所有 C# 基元类型之间的转换器
- **嵌套对象映射** — 自动递归处理复杂类型属性
- **属性注解控制** — 通过注解灵活控制映射行为（重命名、忽略等）
- **字段映射** — 支持公共字段的映射

## 安装

将 `LeonMapper` 项目引用添加到你的 `.csproj` 中：

```xml
<ProjectReference Include="..\LeonMapper\LeonMapper.csproj" />
```

## 快速开始

### 基础映射

同名属性自动映射：

```csharp
using LeonMapper;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

// 使用
var mapper = new Mapper<User, UserDto>();
var dto = mapper.MapTo(user);
```

### 嵌套对象映射

复杂类型属性自动递归映射：

```csharp
public class User
{
    public int Id { get; set; }
    public Role Role { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public RoleDto Role { get; set; }
}

var mapper = new Mapper<User, UserDto>();
var dto = mapper.MapTo(user); // Role 也会被自动映射为 RoleDto
```

### 不同类型自动转换

不同基元类型之间自动转换：

```csharp
public class User
{
    public string StudentNumber { get; set; }  // string
}

public class UserDto
{
    public int StudentNumber { get; set; }     // int，自动转换
}
```

## 属性注解

### `[MapTo]` — 源属性映射到不同名称的目标属性

支持一对多映射（`AllowMultiple = true`）：

```csharp
public class User
{
    [MapTo(nameof(UserDto.Name))]
    public string StudentNumber { get; set; }
}

public class UserDto
{
    public string Name { get; set; }
}
```

### `[MapFrom]` — 目标属性从不同名称的源属性拉取

优先级高于 `[MapTo]`：

```csharp
public class User
{
    public string FullName { get; set; }
}

public class UserDto
{
    [MapFrom("FullName")]
    public string DisplayName { get; set; }
}
```

### `[IgnoreMap]` — 完全忽略映射

标注在源或目标属性上均可：

```csharp
public class User
{
    public int Id { get; set; }
    [IgnoreMap]
    public string Password { get; set; }  // 不会被映射
}
```

### `[IgnoreMapTo]` — 源属性禁止映射到目标

仅标注在源属性上：

```csharp
public class User
{
    [IgnoreMapTo]
    public string InternalId { get; set; }  // 不会映射到任何目标属性
}
```

### `[IgnoreMapFrom]` — 目标属性禁止接收映射

仅标注在目标属性上：

```csharp
public class UserDto
{
    [IgnoreMapFrom]
    public string ComputedField { get; set; }  // 不会从任何源属性接收值
}
```

## 配置

通过 `MapperConfig` 静态类进行全局配置：

```csharp
using LeonMapper.Config;

// 设置默认引擎（默认 Expression）
MapperConfig.SetDefaultProcessType(ProcessTypeEnum.Emit);

// 设置转换器范围（默认 Common）
MapperConfig.SetConverterScope(ConverterScope.All);

// 设置是否自动转换不同类型（默认 true）
MapperConfig.SetAutoConvert(false);
```

### 引擎选择

| 引擎 | 说明 | 状态 |
|------|------|------|
| `ProcessTypeEnum.Expression` | 表达式树编译，功能完整 | 已完成（默认） |
| `ProcessTypeEnum.Emit` | IL 动态生成，功能完整 | 已完成 |

也可以在每次调用时指定引擎：

```csharp
// 使用默认引擎（全局配置）
var mapper = new Mapper<User, UserDto>();
var result = mapper.MapTo(user);

// 指定使用 Emit 引擎
var emitMapper = new Mapper<User, UserDto>(ProcessTypeEnum.Emit);
var result2 = emitMapper.MapTo(user);
```

### 转换器范围

- `ConverterScope.Common` — 仅使用标记为 `[CommonConverter]` 的常用转换器
- `ConverterScope.All` — 使用所有可用的转换器（包括可能有精度损失或溢出的转换）

## 运行测试

```bash
dotnet test LeonMapper.Test/LeonMapper.Test.csproj
```

测试包含性能基准，对比手写映射、LeonMapper 和 AutoMapper 的执行耗时。

## License

[MIT](LICENSE)
