```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8117/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9900X 4.40GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v4


```
| Method     | Categories | Input    | Mean      | Error     | StdDev    | StdErr    | Min       | Q1        | Median    | Q3        | Max       | Op/s   | Allocated |
|----------- |----------- |--------- |----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|-------:|----------:|
| Jens_Part1 | Part 1     | Bart.txt |  2.264 ms | 0.0382 ms | 0.0357 ms | 0.0092 ms |  2.224 ms |  2.241 ms |  2.250 ms |  2.288 ms |  2.336 ms | 441.69 |         - |
| Noe_Part1  | Part 1     | Bart.txt |  2.531 ms | 0.0096 ms | 0.0089 ms | 0.0023 ms |  2.515 ms |  2.525 ms |  2.530 ms |  2.540 ms |  2.541 ms | 395.14 |         - |
| Bart_Part1 | Part 1     | Bart.txt | 24.142 ms | 0.1279 ms | 0.1196 ms | 0.0309 ms | 24.002 ms | 24.027 ms | 24.127 ms | 24.209 ms | 24.358 ms |  41.42 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |        |           |
| Noe_Part1  | Part 1     | Jens.txt |  2.172 ms | 0.0108 ms | 0.0101 ms | 0.0026 ms |  2.159 ms |  2.165 ms |  2.170 ms |  2.181 ms |  2.189 ms | 460.33 |         - |
| Jens_Part1 | Part 1     | Jens.txt |  2.247 ms | 0.0192 ms | 0.0160 ms | 0.0044 ms |  2.226 ms |  2.231 ms |  2.246 ms |  2.259 ms |  2.277 ms | 445.07 |         - |
| Bart_Part1 | Part 1     | Jens.txt | 25.034 ms | 0.1161 ms | 0.1086 ms | 0.0280 ms | 24.900 ms | 24.936 ms | 25.043 ms | 25.093 ms | 25.286 ms |  39.95 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |        |           |
| Jens_Part1 | Part 1     | Noe.txt  |  2.294 ms | 0.0259 ms | 0.0229 ms | 0.0061 ms |  2.255 ms |  2.281 ms |  2.289 ms |  2.305 ms |  2.340 ms | 436.01 |         - |
| Noe_Part1  | Part 1     | Noe.txt  |  2.559 ms | 0.0108 ms | 0.0101 ms | 0.0026 ms |  2.546 ms |  2.552 ms |  2.556 ms |  2.567 ms |  2.581 ms | 390.75 |         - |
| Bart_Part1 | Part 1     | Noe.txt  | 24.495 ms | 0.0345 ms | 0.0306 ms | 0.0082 ms | 24.429 ms | 24.484 ms | 24.491 ms | 24.518 ms | 24.553 ms |  40.82 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |        |           |
| Noe_Part2  | Part 2     | Bart.txt |  4.313 ms | 0.0294 ms | 0.0275 ms | 0.0071 ms |  4.273 ms |  4.292 ms |  4.317 ms |  4.334 ms |  4.359 ms | 231.85 |         - |
| Jens_Part2 | Part 2     | Bart.txt |  9.059 ms | 0.1125 ms | 0.0997 ms | 0.0266 ms |  8.879 ms |  9.007 ms |  9.059 ms |  9.097 ms |  9.231 ms | 110.39 |         - |
| Bart_Part2 | Part 2     | Bart.txt | 24.491 ms | 0.1008 ms | 0.0942 ms | 0.0243 ms | 24.370 ms | 24.430 ms | 24.455 ms | 24.540 ms | 24.686 ms |  40.83 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |        |           |
| Noe_Part2  | Part 2     | Jens.txt |  4.324 ms | 0.0214 ms | 0.0201 ms | 0.0052 ms |  4.297 ms |  4.307 ms |  4.321 ms |  4.336 ms |  4.371 ms | 231.26 |         - |
| Jens_Part2 | Part 2     | Jens.txt |  8.193 ms | 0.1629 ms | 0.1600 ms | 0.0400 ms |  7.979 ms |  8.090 ms |  8.152 ms |  8.302 ms |  8.554 ms | 122.05 |         - |
| Bart_Part2 | Part 2     | Jens.txt | 24.947 ms | 0.1349 ms | 0.1196 ms | 0.0320 ms | 24.685 ms | 24.897 ms | 24.987 ms | 25.022 ms | 25.096 ms |  40.09 |         - |
|            |            |          |           |           |           |           |           |           |           |           |           |        |           |
| Noe_Part2  | Part 2     | Noe.txt  |  4.429 ms | 0.0154 ms | 0.0144 ms | 0.0037 ms |  4.412 ms |  4.417 ms |  4.430 ms |  4.435 ms |  4.460 ms | 225.77 |         - |
| Jens_Part2 | Part 2     | Noe.txt  |  8.198 ms | 0.1351 ms | 0.1197 ms | 0.0320 ms |  8.050 ms |  8.116 ms |  8.149 ms |  8.286 ms |  8.436 ms | 121.98 |         - |
| Bart_Part2 | Part 2     | Noe.txt  | 24.741 ms | 0.0982 ms | 0.0870 ms | 0.0233 ms | 24.588 ms | 24.679 ms | 24.775 ms | 24.802 ms | 24.848 ms |  40.42 |         - |
