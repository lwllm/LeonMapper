using BenchmarkDotNet.Running;
using LeonMapper.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(SimpleBenchmark).Assembly).Run(args);