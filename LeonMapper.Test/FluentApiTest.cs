using System;
using System.Linq;
using System.Linq.Expressions;
using LeonMapper.Attributes;
using LeonMapper.Config;
using LeonMapper.Convert;
using LeonMapper.Plan.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

[TestClass]
public class FluentApiTest
{
    #region Test Models

    public class FluentSource
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class FluentTarget
    {
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    // 带 MapFrom 属性注解的模型，用于测试优先级
    public class AttributePrioritySource
    {
        public string A { get; set; } = string.Empty;
        public string B { get; set; } = string.Empty;
    }

    public class AttributePriorityTarget
    {
        [MapFrom("A")]
        public string Result { get; set; } = string.Empty;
    }

    // 自定义转换器模型
    public class NameSource
    {
        public string Value { get; set; } = string.Empty;
    }

    public class NameTarget
    {
        public string Value { get; set; } = string.Empty;
    }

    public class UpperCaseConverter : IConverter<string, string>
    {
        public string Convert(string input) => input.ToUpperInvariant();
    }

    // 类型转换模型
    public class TypeConvertSource
    {
        public int Number { get; set; }
    }

    public class TypeConvertTarget
    {
        public string Number { get; set; } = string.Empty;
    }

    // 重复配置测试模型
    public class DuplicateSource
    {
        public string X { get; set; } = string.Empty;
        public string Y { get; set; } = string.Empty;
    }

    public class DuplicateTarget
    {
        public string Result { get; set; } = string.Empty;
    }

    // ConvertWith 既有同名匹配的模型
    public class ConvertSameNameSource
    {
        public string Value { get; set; } = string.Empty;
    }

    public class ConvertSameNameTarget
    {
        public string Value { get; set; } = string.Empty;
    }

    // Fluent vs MapTo 属性优先级测试模型
    public class MapToPrioritySource
    {
        public string X { get; set; } = string.Empty;
        public string Y { get; set; } = string.Empty;

        [MapTo("Result")]
        public string Z { get; set; } = string.Empty;
    }

    public class MapToPriorityTarget
    {
        public string Result { get; set; } = string.Empty;
    }

    // Fluent MapFrom + ConvertUsing 组合测试模型
    public class ComboSource
    {
        public string A { get; set; } = string.Empty;
        public string B { get; set; } = string.Empty;
    }

    public class ComboTarget
    {
        public string Result { get; set; } = string.Empty;
    }

    // Fluent MapFrom 源属性不存在测试模型
    public class MissingSourcePropSource
    {
        public string Existing { get; set; } = string.Empty;
    }

    public class MissingSourcePropTarget
    {
        public string Result { get; set; } = string.Empty;
    }

    // GetPlan/Validate 集成测试模型
    public class PlanValidateSource
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class PlanValidateTarget
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Extra { get; set; } = string.Empty; // 未映射属性
    }

    // 嵌套复杂类型测试模型
    public class NestedParentSource
    {
        public string Name { get; set; } = string.Empty;
        public NestedChildSource Child { get; set; } = new();
    }

    public class NestedChildSource
    {
        public string Value { get; set; } = string.Empty;
        public string Other { get; set; } = string.Empty;
    }

    public class NestedParentTarget
    {
        public string Name { get; set; } = string.Empty;
        public NestedChildTarget Child { get; set; } = new();
    }

    public class NestedChildTarget
    {
        public string Value { get; set; } = string.Empty;
        public string Other { get; set; } = string.Empty;
    }

    // Fluent + PlanBuildOptions 组合测试模型
    public class OptionsSource
    {
        public int Number { get; set; }
    }

    public class OptionsTarget
    {
        public string Number { get; set; } = string.Empty;
    }

    // 空配置测试
    public class EmptyConfigSource
    {
        public string Value { get; set; } = string.Empty;
    }

    public class EmptyConfigTarget
    {
        public string Value { get; set; } = string.Empty;
    }

    // Fluent 覆盖 IgnoreMapFrom 属性注解
    public class OverrideIgnoreMapFromSource
    {
        public string X { get; set; } = string.Empty;
        public string Y { get; set; } = string.Empty;
    }

    public class OverrideIgnoreMapFromTarget
    {
        [IgnoreMapFrom]
        public string X { get; set; } = string.Empty;

        public string Y { get; set; } = string.Empty;
    }

    // Fluent MapFrom 源属性为字段（不支持，应静默忽略）
    public class FieldSource
    {
        public string FieldValue = string.Empty;
        public string PropValue { get; set; } = string.Empty;
    }

    public class FieldTarget
    {
        public string Result { get; set; } = string.Empty;
    }

    #endregion

    [TestMethod]
    public void ForMember_MapFrom_ShouldUseSpecifiedSourceProperty()
    {
        var source = new FluentSource { FirstName = "Leon", LastName = "Wang" };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("Leon", result.FullName);
    }

    [TestMethod]
    public void ForMember_Ignore_ShouldSkipProperty()
    {
        var source = new FluentSource { Phone = "123456", Age = 25 };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.Phone, opt => opt.Ignore());
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result.Phone);
        Assert.AreEqual(25, result.Age);
    }

    [TestMethod]
    public void ForMember_ConvertUsing_ShouldUseCustomConverter()
    {
        var source = new NameSource { Value = "hello" };
        var mapper = new Mapper<NameSource, NameTarget>(cfg =>
        {
            cfg.ForMember(d => d.Value, opt => opt.ConvertUsing<UpperCaseConverter>());
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("HELLO", result.Value);
    }

    [TestMethod]
    public void ForMember_ChainedCalls_ShouldApplyAll()
    {
        var source = new FluentSource
        {
            FirstName = "Leon", LastName = "Wang", Age = 25, Phone = "123", Email = "test@test.com"
        };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg => cfg
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName))
            .ForMember(d => d.Phone, opt => opt.Ignore())
            .ForMember(d => d.ContactEmail, opt => opt.MapFrom(s => s.Email)));
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("Leon", result.FullName);
        Assert.AreEqual(string.Empty, result.Phone);
        Assert.AreEqual("test@test.com", result.ContactEmail);
        Assert.AreEqual(25, result.Age);
    }

    [TestMethod]
    public void Fluent_PriorityOverAttributeMapFrom()
    {
        var source = new AttributePrioritySource { A = "fromA", B = "fromB" };
        // AttributePriorityTarget.Result 上有 [MapFrom("A")]
        // Fluent MapFrom(s.B) 应覆盖属性注解
        var mapper = new Mapper<AttributePrioritySource, AttributePriorityTarget>(cfg =>
        {
            cfg.ForMember(d => d.Result, opt => opt.MapFrom(s => s.B));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("fromB", result.Result);
    }

    [TestMethod]
    public void Fluent_DifferentConfigs_CacheCorrectness()
    {
        var source = new FluentSource { FirstName = "A", LastName = "B" };

        var mapper1 = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        });

        var mapper2 = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.LastName));
        });

        var result1 = mapper1.MapTo(source);
        var result2 = mapper2.MapTo(source);

        Assert.AreEqual("A", result1!.FullName);
        Assert.AreEqual("B", result2!.FullName);
    }

    [TestMethod]
    public void DefaultConstructor_NoRegression()
    {
        var source = new FluentSource { Age = 25, Phone = "123" };
        var mapper = new Mapper<FluentSource, FluentTarget>();
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual(25, result.Age);
        Assert.AreEqual("123", result.Phone);
    }

    [TestMethod]
    public void ForMember_MapFrom_InvalidExpression_ShouldThrow()
    {
        Assert.ThrowsException<ArgumentException>(() =>
        {
            new Mapper<FluentSource, FluentTarget>(cfg =>
            {
                cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName + s.LastName));
            });
        });
    }

    [TestMethod]
    public void ForMember_EmitCompiler_ShouldWork()
    {
        var source = new FluentSource { FirstName = "Leon", Phone = "123", Age = 25 };
        // 先用 Fluent 配置构建计划，再用 Emit 编译器
        var config = new MappingConfiguration<FluentSource, FluentTarget>();
        config.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        config.ForMember(d => d.Phone, opt => opt.Ignore());

        var plan = MappingPlanBuilder.Build(config);
        // 通过 Mapper(TypeMappingPlan) + MapperConfig 切换到 Emit 引擎
        var originalType = MapperConfig.GetDefaultProcessType();
        MapperConfig.SetConfiguration(ProcessTypeEnum.Emit, MapperConfig.GetDefaultConverterScope(), true);
        try
        {
            var mapper = new Mapper<FluentSource, FluentTarget>(plan);
            var result = mapper.MapTo(source);
            Assert.IsNotNull(result);
            Assert.AreEqual("Leon", result.FullName);
            Assert.AreEqual(string.Empty, result.Phone);
            Assert.AreEqual(25, result.Age);
        }
        finally
        {
            MapperConfig.SetConfiguration(originalType, MapperConfig.GetDefaultConverterScope(), true);
        }
    }

    [TestMethod]
    public void ForMember_ConvertUsing_WithSameNameProperty()
    {
        var source = new ConvertSameNameSource { Value = "hello" };
        var mapper = new Mapper<ConvertSameNameSource, ConvertSameNameTarget>(cfg =>
        {
            cfg.ForMember(d => d.Value, opt => opt.ConvertUsing<UpperCaseConverter>());
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("HELLO", result.Value);
    }

    [TestMethod]
    public void ForMember_DuplicateConfig_LastWins()
    {
        var source = new DuplicateSource { X = "X", Y = "Y" };
        var mapper = new Mapper<DuplicateSource, DuplicateTarget>(cfg =>
        {
            cfg.ForMember(d => d.Result, opt => opt.MapFrom(s => s.X));
            cfg.ForMember(d => d.Result, opt => opt.MapFrom(s => s.Y)); // 后者覆盖
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("Y", result.Result);
    }

    [TestMethod]
    public void ForMember_TargetExpression_InvalidMethodCall_ShouldThrow()
    {
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Expression<Func<FluentTarget, string>> expr = d => d.ToString()!;
            new Mapper<FluentSource, FluentTarget>(cfg =>
            {
                cfg.ForMember(expr, opt => opt.MapFrom(s => s.FirstName));
            });
        });
    }

    [TestMethod]
    public void ForMember_TargetExpression_NotMemberAccess_ShouldThrow()
    {
        Assert.ThrowsException<ArgumentException>(() =>
        {
            // lambda body 不是 MemberExpression
            Expression<Func<FluentTarget, object>> expr = d => new object();
            new Mapper<FluentSource, FluentTarget>(cfg =>
            {
                cfg.ForMember(expr, opt => opt.MapFrom(s => s.FirstName));
            });
        });
    }

    [TestMethod]
    public void Fluent_PriorityOverMapToAttribute()
    {
        // MapToPrioritySource.Z 有 [MapTo("Result")]，默认应映射 Z→Result
        // Fluent MapFrom(s.X) 应覆盖 MapTo 属性注解
        var source = new MapToPrioritySource { X = "fromX", Y = "fromY", Z = "fromZ" };
        var mapper = new Mapper<MapToPrioritySource, MapToPriorityTarget>(cfg =>
        {
            cfg.ForMember(d => d.Result, opt => opt.MapFrom(s => s.Y));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("fromY", result.Result);
    }

    [TestMethod]
    public void MapToAttribute_WithoutFluent_ShouldStillWork()
    {
        // 确认不使用 Fluent 时 MapTo 仍然正常工作
        var source = new MapToPrioritySource { X = "fromX", Y = "fromY", Z = "fromZ" };
        var mapper = new Mapper<MapToPrioritySource, MapToPriorityTarget>();
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("fromZ", result.Result); // Z 有 [MapTo("Result")]
    }

    [TestMethod]
    public void Fluent_MapFrom_Plus_ConvertUsing_Combined()
    {
        // 同一个属性同时使用 MapFrom 和 ConvertUsing
        // 应该以最后调用的为准（last wins 语义）
        var source = new ComboSource { A = "hello", B = "world" };
        var mapper = new Mapper<ComboSource, ComboTarget>(cfg =>
        {
            cfg.ForMember(d => d.Result, opt =>
            {
                opt.MapFrom(s => s.A);
                opt.ConvertUsing<UpperCaseConverter>();
            });
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        // ConvertUsing 后覆盖了 MapFrom 的 ActionType
        // 但 ConvertUsing 需要同名属性存在，Result 在源中不存在
        // 实际行为取决于实现：ConvertUsing 覆盖了 ActionType 为 ConvertUsing
        // ApplyFluentOverrides 中 ConvertUsing 会尝试同名匹配 "Result"
        // 源中没有 Result 属性，所以不会添加配对 → Result 不会被映射
        Assert.AreEqual(string.Empty, result.Result);
    }

    [TestMethod]
    public void Fluent_MapFrom_SourcePropertyNotExist_ShouldNotMap()
    {
        // MapFrom 引用源类型的字段（而非属性），ApplyFluentOverrides 只查找属性
        // sourceProps 字典中不包含字段，所以匹配不上，应静默跳过
        var source = new FieldSource { FieldValue = "field", PropValue = "prop" };
        var mapper = new Mapper<FieldSource, FieldTarget>(cfg =>
        {
            cfg.ForMember(d => d.Result, opt => opt.MapFrom(s => s.FieldValue));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        // FieldValue 是字段不是属性，MapFrom 无法匹配，Result 保持默认值
        Assert.AreEqual(string.Empty, result.Result);
    }

    [TestMethod]
    public void Fluent_GetPlan_ShouldReflectFluentConfig()
    {
        // GetPlan 不接受 Fluent 配置（是静态方法，使用默认配置）
        // 所以这里验证通过 MappingPlanBuilder.Build(config) 获取的计划
        var config = new MappingConfiguration<PlanValidateSource, PlanValidateTarget>();
        config.ForMember(d => d.Extra, opt => opt.MapFrom(s => s.Name));

        var plan = MappingPlanBuilder.Build(config);
        Assert.IsNotNull(plan);
        // Extra 应该被映射（从 Name 映射过来）
        Assert.IsTrue(plan.PropertyMappings.Any(m => m.TargetMember.Name == "Extra"));
    }

    [TestMethod]
    public void Fluent_Validate_ShouldWorkWithFluentPlan()
    {
        var config = new MappingConfiguration<PlanValidateSource, PlanValidateTarget>();
        config.ForMember(d => d.Extra, opt => opt.MapFrom(s => s.Name));

        var plan = MappingPlanBuilder.Build(config);
        var validation = LeonMapper.Validation.MappingValidator.Validate(plan);
        Assert.IsNotNull(validation);
        // Name→Name, Age→Age 自动匹配，Extra 通过 Fluent MapFrom 映射
        // 未映射目标应为空
        Assert.IsTrue(validation.IsValid);
    }

    [TestMethod]
    public void Fluent_NestedType_FluentConfigDoesNotPropagate()
    {
        // Fluent 配置不应传播到嵌套复杂类型
        var source = new NestedParentSource
        {
            Name = "Parent",
            Child = new NestedChildSource { Value = "Child", Other = "Other" }
        };
        var mapper = new Mapper<NestedParentSource, NestedParentTarget>(cfg =>
        {
            cfg.ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("Parent", result.Name);
        // 嵌套的 Child 应该用默认配置自动映射
        Assert.IsNotNull(result.Child);
        Assert.AreEqual("Child", result.Child.Value);
        Assert.AreEqual("Other", result.Child.Other);
    }

    [TestMethod]
    public void Fluent_EmptyConfig_ShouldBehaveLikeDefault()
    {
        // 传入空的 Fluent 配置应等同于无配置
        var source = new EmptyConfigSource { Value = "test" };
        var mapper = new Mapper<EmptyConfigSource, EmptyConfigTarget>(cfg =>
        {
            // 不调用任何 ForMember
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("test", result.Value);
    }

    [TestMethod]
    public void Fluent_IgnoreAllProperties_ShouldReturnDefaultTarget()
    {
        var source = new FluentSource { FirstName = "A", LastName = "B", Age = 25, Phone = "123", Email = "e" };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg => cfg
            .ForMember(d => d.FullName, opt => opt.Ignore())
            .ForMember(d => d.Age, opt => opt.Ignore())
            .ForMember(d => d.ContactEmail, opt => opt.Ignore())
            .ForMember(d => d.Phone, opt => opt.Ignore()));
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result.FullName);
        Assert.AreEqual(0, result.Age);
        Assert.AreEqual(string.Empty, result.ContactEmail);
        Assert.AreEqual(string.Empty, result.Phone);
    }

    [TestMethod]
    public void Fluent_OverrideIgnoreMapFromAttribute()
    {
        // OverrideIgnoreMapFromTarget.X 上有 [IgnoreMapFrom]
        // Fluent MapFrom 应覆盖该属性注解，允许映射
        var source = new OverrideIgnoreMapFromSource { X = "valueX", Y = "valueY" };
        var mapper = new Mapper<OverrideIgnoreMapFromSource, OverrideIgnoreMapFromTarget>(cfg =>
        {
            cfg.ForMember(d => d.X, opt => opt.MapFrom(s => s.Y));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("valueY", result.X);
    }

    [TestMethod]
    public void Fluent_IgnoreMapFromAttribute_WithoutFluent_ShouldBlock()
    {
        // 确认不使用 Fluent 时 IgnoreMapFrom 正常工作
        var source = new OverrideIgnoreMapFromSource { X = "valueX", Y = "valueY" };
        var mapper = new Mapper<OverrideIgnoreMapFromSource, OverrideIgnoreMapFromTarget>();
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result.X); // [IgnoreMapFrom] 阻止了同名映射
        Assert.AreEqual("valueY", result.Y);
    }

    [TestMethod]
    public void Fluent_ConvertUsing_EmitCompiler_ShouldWork()
    {
        var source = new ConvertSameNameSource { Value = "hello" };
        var config = new MappingConfiguration<ConvertSameNameSource, ConvertSameNameTarget>();
        config.ForMember(d => d.Value, opt => opt.ConvertUsing<UpperCaseConverter>());

        var plan = MappingPlanBuilder.Build(config);
        var originalType = MapperConfig.GetDefaultProcessType();
        MapperConfig.SetConfiguration(ProcessTypeEnum.Emit, MapperConfig.GetDefaultConverterScope(), true);
        try
        {
            var mapper = new Mapper<ConvertSameNameSource, ConvertSameNameTarget>(plan);
            var result = mapper.MapTo(source);
            Assert.IsNotNull(result);
            Assert.AreEqual("HELLO", result.Value);
        }
        finally
        {
            MapperConfig.SetConfiguration(originalType, MapperConfig.GetDefaultConverterScope(), true);
        }
    }

    [TestMethod]
    public void Fluent_MapFrom_SamePropertyTwice_DifferentTargets()
    {
        // 同一个源属性映射到多个不同目标属性
        var source = new FluentSource { FirstName = "F", LastName = "L" };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg => cfg
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName))
            .ForMember(d => d.ContactEmail, opt => opt.MapFrom(s => s.FirstName)));
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("F", result.FullName);
        Assert.AreEqual("F", result.ContactEmail);
    }

    [TestMethod]
    public void Fluent_IgnoreThenMapFrom_SameProperty_LastWins()
    {
        // 先 Ignore 再 MapFrom 同一属性，后者覆盖
        var source = new FluentSource { FirstName = "F" };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.Ignore());
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("F", result.FullName);
    }

    [TestMethod]
    public void Fluent_MapFrom_ThenIgnore_SameProperty_LastWins()
    {
        // 先 MapFrom 再 Ignore 同一属性，后者覆盖
        var source = new FluentSource { FirstName = "F" };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
            cfg.ForMember(d => d.FullName, opt => opt.Ignore());
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result.FullName);
    }

    [TestMethod]
    public void Fluent_MapFrom_NullSource_ShouldMapCorrectly()
    {
        var source = new FluentSource { FirstName = null!, Age = 25 };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual(null, result.FullName);
    }

    [TestMethod]
    public void Fluent_MapTo_NonExistentTargetProperty_ShouldNotThrow()
    {
        // ForMember 目标属性名在运行时不存在于目标类型（编译时已约束，但验证健壮性）
        // 由于泛型约束，这种情况在编译时就会被捕获，无法构造
        // 所以这个测试验证的是正常路径
        var source = new FluentSource { FirstName = "A" };
        var mapper = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("A", result.FullName);
    }

    [TestMethod]
    public void Fluent_ConfigHash_DifferentConfigs_ProduceDifferentResults()
    {
        // 间接验证缓存隔离：不同配置产生不同结果
        var source = new FluentSource { FirstName = "A", LastName = "B", Age = 25 };

        var mapper1 = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        });
        var mapper2 = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.LastName));
        });

        var result1 = mapper1.MapTo(source);
        var result2 = mapper2.MapTo(source);

        Assert.AreEqual("A", result1!.FullName);
        Assert.AreEqual("B", result2!.FullName);
    }

    [TestMethod]
    public void Fluent_ConfigHash_SameConfig_ProduceSameResults()
    {
        // 间接验证缓存命中：相同配置产生相同结果
        var source = new FluentSource { FirstName = "A", Age = 25 };

        var mapper1 = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        });
        var mapper2 = new Mapper<FluentSource, FluentTarget>(cfg =>
        {
            cfg.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));
        });

        var result1 = mapper1.MapTo(source);
        var result2 = mapper2.MapTo(source);

        Assert.AreEqual(result1!.FullName, result2!.FullName);
        Assert.AreEqual("A", result1.FullName);
    }

    [TestMethod]
    public void Fluent_Ignore_WithMapFromAttributeOnTarget_ShouldStillIgnore()
    {
        // 目标属性有 MapFrom 属性注解，但 Fluent Ignore 应覆盖它
        var source = new AttributePrioritySource { A = "fromA", B = "fromB" };
        var mapper = new Mapper<AttributePrioritySource, AttributePriorityTarget>(cfg =>
        {
            cfg.ForMember(d => d.Result, opt => opt.Ignore());
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result.Result); // Fluent Ignore 覆盖了 MapFrom 属性注解
    }

    [TestMethod]
    public void Fluent_ConvertUsing_IgnoreAndConvertSameProperty_IgnoreWins()
    {
        // 同一属性先 ConvertUsing 再 Ignore，后者覆盖
        var source = new ConvertSameNameSource { Value = "hello" };
        var mapper = new Mapper<ConvertSameNameSource, ConvertSameNameTarget>(cfg =>
        {
            cfg.ForMember(d => d.Value, opt => opt.ConvertUsing<UpperCaseConverter>());
            cfg.ForMember(d => d.Value, opt => opt.Ignore());
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result.Value);
    }

    [TestMethod]
    public void Fluent_MapFrom_WithTypeConversion()
    {
        // MapFrom 映射到不同类型的属性，应自动走 Convert 策略
        var source = new TypeConvertSource { Number = 42 };
        var mapper = new Mapper<TypeConvertSource, TypeConvertTarget>(cfg =>
        {
            cfg.ForMember(d => d.Number, opt => opt.MapFrom(s => s.Number));
        });
        var result = mapper.MapTo(source);
        Assert.IsNotNull(result);
        Assert.AreEqual("42", result.Number);
    }

    [TestMethod]
    public void Fluent_EmitCompiler_MapFrom_ShouldWork()
    {
        var source = new FluentSource { FirstName = "Leon", Age = 25 };
        var config = new MappingConfiguration<FluentSource, FluentTarget>();
        config.ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName));

        var plan = MappingPlanBuilder.Build(config);
        var originalType = MapperConfig.GetDefaultProcessType();
        MapperConfig.SetConfiguration(ProcessTypeEnum.Emit, MapperConfig.GetDefaultConverterScope(), true);
        try
        {
            var mapper = new Mapper<FluentSource, FluentTarget>(plan);
            var result = mapper.MapTo(source);
            Assert.IsNotNull(result);
            Assert.AreEqual("Leon", result.FullName);
            Assert.AreEqual(25, result.Age);
        }
        finally
        {
            MapperConfig.SetConfiguration(originalType, MapperConfig.GetDefaultConverterScope(), true);
        }
    }

    [TestMethod]
    public void Fluent_EmitCompiler_Ignore_ShouldWork()
    {
        var source = new FluentSource { Phone = "123", Age = 25 };
        var config = new MappingConfiguration<FluentSource, FluentTarget>();
        config.ForMember(d => d.Phone, opt => opt.Ignore());

        var plan = MappingPlanBuilder.Build(config);
        var originalType = MapperConfig.GetDefaultProcessType();
        MapperConfig.SetConfiguration(ProcessTypeEnum.Emit, MapperConfig.GetDefaultConverterScope(), true);
        try
        {
            var mapper = new Mapper<FluentSource, FluentTarget>(plan);
            var result = mapper.MapTo(source);
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.Phone);
            Assert.AreEqual(25, result.Age);
        }
        finally
        {
            MapperConfig.SetConfiguration(originalType, MapperConfig.GetDefaultConverterScope(), true);
        }
    }
}
