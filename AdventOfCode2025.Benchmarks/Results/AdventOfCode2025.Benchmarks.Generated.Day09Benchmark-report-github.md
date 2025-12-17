```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9900X 4.40GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method     | Categories | Input    | Mean      | Error    | StdDev   | StdErr   | Min       | Q1        | Median    | Q3        | Max      | Op/s     | Allocated |
|----------- |----------- |--------- |----------:|---------:|---------:|---------:|----------:|----------:|----------:|----------:|---------:|---------:|----------:|
| Jens_Part1 | Part 1     | Jens.txt |  96.00 μs | 1.888 μs | 3.500 μs | 0.534 μs |  91.12 μs |  93.42 μs |  94.75 μs |  97.97 μs | 103.5 μs | 10,416.5 |         - |
|            |            |          |           |          |          |          |           |           |           |           |          |          |           |
| Jens_Part2 | Part 2     | Jens.txt | 383.08 μs | 4.930 μs | 4.611 μs | 1.191 μs | 376.24 μs | 378.84 μs | 382.24 μs | 387.78 μs | 388.9 μs |  2,610.4 |         - |
