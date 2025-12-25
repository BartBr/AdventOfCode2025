```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9900X 4.40GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method     | Categories | Input    | Mean      | Error     | StdDev    | StdErr    | Min       | Q1        | Median    | Q3        | Max       | Op/s      | Allocated |
|----------- |----------- |--------- |----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|
| Jens_Part1 | Part 1     | Jens.txt |  3.904 μs | 0.0154 μs | 0.0128 μs | 0.0036 μs |  3.883 μs |  3.893 μs |  3.907 μs |  3.915 μs |  3.919 μs | 256,178.7 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |           |           |
| Jens_Part2 | Part 2     | Jens.txt | 44.435 μs | 0.2225 μs | 0.1858 μs | 0.0515 μs | 44.013 μs | 44.332 μs | 44.471 μs | 44.522 μs | 44.770 μs |  22,504.9 |         - |
