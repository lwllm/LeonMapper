using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using LeonMapper.Config;

namespace LeonMapper.Benchmarks;

[Config(typeof(BenchmarkConfig))]
[MemoryDiagnoser]
[RankColumn]
public abstract class BenchmarkBase
{
    protected class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddJob(Job.ShortRun
                .WithWarmupCount(3)
                .WithIterationCount(10));
            AddColumn(StatisticColumn.P95);
        }
    }

    protected IMapper AutoMapperInstance { get; private set; } = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SimpleSource, SimpleTarget>();
            cfg.CreateMap<AddressSource, AddressTarget>();
            cfg.CreateMap<NestedSource, NestedTarget>();
            cfg.CreateMap<ItemSource, ItemTarget>();
            cfg.CreateMap<CollectionSource, CollectionTarget>();
            cfg.CreateMap<TypeConvertSource, TypeConvertTarget>();
        });
        AutoMapperInstance = config.CreateMapper();

        SetupData();
    }

    protected virtual void SetupData() { }
}

/// <summary>
/// 简单对象映射基准：同名同类型属性
/// </summary>
public class SimpleBenchmark : BenchmarkBase
{
    private Mapper<SimpleSource, SimpleTarget> _exprMapper = null!;
    private Mapper<SimpleSource, SimpleTarget> _emitMapper = null!;
    private SimpleSource _source = null!;

    protected override void SetupData()
    {
        _exprMapper = new Mapper<SimpleSource, SimpleTarget>();
        _emitMapper = new Mapper<SimpleSource, SimpleTarget>(ProcessTypeEnum.Emit);
        _source = new SimpleSource { Id = 1, Name = "Test", Age = 25, Score = 99.5, IsActive = true };
    }

    [Benchmark(Baseline = true)]
    public SimpleTarget Manual() => new()
    {
        Id = _source.Id, Name = _source.Name, Age = _source.Age,
        Score = _source.Score, IsActive = _source.IsActive
    };

    [Benchmark]
    public SimpleTarget LeonMapper_Expression() => _exprMapper.MapTo(_source)!;

    [Benchmark]
    public SimpleTarget LeonMapper_Emit() => _emitMapper.MapTo(_source)!;

    [Benchmark]
    public SimpleTarget AutoMapper() => AutoMapperInstance.Map<SimpleSource, SimpleTarget>(_source);
}

/// <summary>
/// 嵌套复杂类型映射基准
/// </summary>
public class NestedBenchmark : BenchmarkBase
{
    private Mapper<NestedSource, NestedTarget> _exprMapper = null!;
    private Mapper<NestedSource, NestedTarget> _emitMapper = null!;
    private NestedSource _source = null!;

    protected override void SetupData()
    {
        _exprMapper = new Mapper<NestedSource, NestedTarget>();
        _emitMapper = new Mapper<NestedSource, NestedTarget>(ProcessTypeEnum.Emit);
        _source = new NestedSource
        {
            Id = 1, Name = "Test",
            Address = new AddressSource { City = "Beijing", Street = "Chaoyang Rd", ZipCode = "100000" }
        };
    }

    [Benchmark(Baseline = true)]
    public NestedTarget Manual() => new()
    {
        Id = _source.Id, Name = _source.Name,
        Address = new AddressTarget
        {
            City = _source.Address.City,
            Street = _source.Address.Street,
            ZipCode = _source.Address.ZipCode
        }
    };

    [Benchmark]
    public NestedTarget LeonMapper_Expression() => _exprMapper.MapTo(_source)!;

    [Benchmark]
    public NestedTarget LeonMapper_Emit() => _emitMapper.MapTo(_source)!;

    [Benchmark]
    public NestedTarget AutoMapper() => AutoMapperInstance.Map<NestedSource, NestedTarget>(_source);
}

/// <summary>
/// 集合映射基准
/// </summary>
public class CollectionBenchmark : BenchmarkBase
{
    private Mapper<CollectionSource, CollectionTarget> _exprMapper = null!;
    private Mapper<CollectionSource, CollectionTarget> _emitMapper = null!;
    private CollectionSource _source = null!;

    protected override void SetupData()
    {
        _exprMapper = new Mapper<CollectionSource, CollectionTarget>();
        _emitMapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        _source = new CollectionSource
        {
            Id = 1,
            Items = Enumerable.Range(1, 10).Select(i => new ItemSource { Id = i, Value = $"Item{i}" }).ToList()
        };
    }

    [Benchmark(Baseline = true)]
    public CollectionTarget Manual() => new()
    {
        Id = _source.Id,
        Items = _source.Items.Select(i => new ItemTarget { Id = i.Id, Value = i.Value }).ToList()
    };

    [Benchmark]
    public CollectionTarget LeonMapper_Expression() => _exprMapper.MapTo(_source)!;

    [Benchmark]
    public CollectionTarget LeonMapper_Emit() => _emitMapper.MapTo(_source)!;

    [Benchmark]
    public CollectionTarget AutoMapper() => AutoMapperInstance.Map<CollectionSource, CollectionTarget>(_source);
}

/// <summary>
/// 类型转换映射基准：不同类型属性（int→string, double→string, string→int）
/// </summary>
public class TypeConvertBenchmark : BenchmarkBase
{
    private Mapper<TypeConvertSource, TypeConvertTarget> _exprMapper = null!;
    private Mapper<TypeConvertSource, TypeConvertTarget> _emitMapper = null!;
    private TypeConvertSource _source = null!;

    protected override void SetupData()
    {
        _exprMapper = new Mapper<TypeConvertSource, TypeConvertTarget>();
        _emitMapper = new Mapper<TypeConvertSource, TypeConvertTarget>(ProcessTypeEnum.Emit);
        _source = new TypeConvertSource { IntValue = 42, DoubleValue = 3.14, StringValue = "100" };
    }

    [Benchmark(Baseline = true)]
    public TypeConvertTarget Manual() => new()
    {
        IntValue = _source.IntValue.ToString(),
        DoubleValue = _source.DoubleValue.ToString(),
        StringValue = int.Parse(_source.StringValue)
    };

    [Benchmark]
    public TypeConvertTarget LeonMapper_Expression() => _exprMapper.MapTo(_source)!;

    [Benchmark]
    public TypeConvertTarget LeonMapper_Emit() => _emitMapper.MapTo(_source)!;

    [Benchmark]
    public TypeConvertTarget AutoMapper() => AutoMapperInstance.Map<TypeConvertSource, TypeConvertTarget>(_source);
}
