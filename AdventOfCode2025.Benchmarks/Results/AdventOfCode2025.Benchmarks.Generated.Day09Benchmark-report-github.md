```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9900X 4.40GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method     | Categories | Input    | Mean        | Error     | StdDev   | StdErr   | Min         | Q1          | Median      | Q3          | Max         | Op/s     | Allocated |
|----------- |----------- |--------- |------------:|----------:|---------:|---------:|------------:|------------:|------------:|------------:|------------:|---------:|----------:|
| Jens_Part1 | Part 1     | Jens.txt |    95.75 μs |  1.642 μs | 1.686 μs | 0.409 μs |    92.77 μs |    94.47 μs |    95.57 μs |    96.83 μs |    99.10 μs | 10,443.8 |         - |
|            |            |          |             |           |          |          |             |             |             |             |             |          |           |
| Jens_Part2 | Part 2     | Jens.txt | 1,704.95 μs | 10.154 μs | 9.002 μs | 2.406 μs | 1,686.19 μs | 1,700.74 μs | 1,704.67 μs | 1,708.53 μs | 1,722.71 μs |    586.5 |         - |
