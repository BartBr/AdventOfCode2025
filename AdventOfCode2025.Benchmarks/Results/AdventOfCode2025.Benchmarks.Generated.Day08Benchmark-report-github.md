```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9900X 4.40GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v4


```
| Method     | Categories | Input    | Mean      | Error     | StdDev    | StdErr    | Min       | Q1        | Median    | Q3        | Max       | Op/s   | Allocated |
|----------- |----------- |--------- |----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|-------:|----------:|
| Jens_Part1 | Part 1     | Bart.txt |  2.194 ms | 0.0401 ms | 0.0355 ms | 0.0095 ms |  2.161 ms |  2.169 ms |  2.183 ms |  2.205 ms |  2.269 ms | 455.75 |         - |
| Bart_Part1 | Part 1     | Bart.txt | 23.872 ms | 0.2401 ms | 0.2246 ms | 0.0580 ms | 23.584 ms | 23.758 ms | 23.780 ms | 24.018 ms | 24.290 ms |  41.89 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |        |           |
| Jens_Part1 | Part 1     | Jens.txt |  2.206 ms | 0.0405 ms | 0.0379 ms | 0.0098 ms |  2.155 ms |  2.181 ms |  2.199 ms |  2.235 ms |  2.278 ms | 453.25 |         - |
| Bart_Part1 | Part 1     | Jens.txt | 24.284 ms | 0.2813 ms | 0.2631 ms | 0.0679 ms | 24.031 ms | 24.081 ms | 24.186 ms | 24.430 ms | 24.751 ms |  41.18 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |        |           |
| Jens_Part2 | Part 2     | Bart.txt |  8.876 ms | 0.1763 ms | 0.1886 ms | 0.0445 ms |  8.606 ms |  8.696 ms |  8.881 ms |  8.990 ms |  9.172 ms | 112.66 |         - |
| Bart_Part2 | Part 2     | Bart.txt | 23.665 ms | 0.3655 ms | 0.3419 ms | 0.0883 ms | 23.172 ms | 23.364 ms | 23.471 ms | 23.985 ms | 24.083 ms |  42.26 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |        |           |
| Jens_Part2 | Part 2     | Jens.txt |  7.620 ms | 0.0963 ms | 0.0854 ms | 0.0228 ms |  7.499 ms |  7.557 ms |  7.599 ms |  7.664 ms |  7.778 ms | 131.24 |         - |
| Bart_Part2 | Part 2     | Jens.txt | 24.399 ms | 0.3330 ms | 0.3115 ms | 0.0804 ms | 24.085 ms | 24.146 ms | 24.265 ms | 24.758 ms | 24.882 ms |  40.98 |         - |
