```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9900X 4.40GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method     | Categories | Input    | Mean           | Error       | StdDev        | StdErr      | Min            | Q1             | Median         | Q3             | Max            | Op/s            | Allocated |
|----------- |----------- |--------- |---------------:|------------:|--------------:|------------:|---------------:|---------------:|---------------:|---------------:|---------------:|----------------:|----------:|
| Jens_Part1 | Part 1     | Jens.txt | 21,371.9458 ns | 424.0749 ns | 1,102.2270 ns | 124.0102 ns | 19,962.1552 ns | 20,321.2173 ns | 21,520.8496 ns | 21,950.9064 ns | 23,881.1218 ns |        46,790.3 |         - |
|            |            |          |                |             |               |             |                |                |                |                |                |                 |           |
| Jens_Part2 | Part 2     | Jens.txt |      0.3557 ns |   0.0310 ns |     0.0319 ns |   0.0077 ns |      0.3046 ns |      0.3396 ns |      0.3476 ns |      0.3701 ns |      0.4178 ns | 2,811,437,228.3 |         - |
