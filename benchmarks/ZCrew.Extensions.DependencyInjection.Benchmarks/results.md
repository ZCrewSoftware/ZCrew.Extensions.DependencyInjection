```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
Intel Core i9-14900K 3.20GHz, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.101
  [Host] : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  Dry    : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3

Job=Dry  IterationCount=1  LaunchCount=1
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1

```
| Method                              | Size  |         Mean |      Error |     StdDev |     Gen0 |    Gen1 |  Allocated |
|-------------------------------------|-------|-------------:|-----------:|-----------:|---------:|--------:|-----------:|
| ZCrew_AllInterfaces                 | Small |     3.862 us |  0.0584 us |  0.0518 us |   0.7324 |  0.0153 |   13.58 KB |
| Scrutor_AllInterfaces               | Small |    23.428 us |  0.3330 us |  0.3115 us |   1.4343 |  0.0305 |    26.6 KB |
| Windsor_AllInterfaces               | Small |   137.629 us |  2.2286 us |  2.0847 us |  11.2305 |  1.9531 |  207.15 KB |
| ZCrew_DefaultInterfaces             | Small |     4.263 us |  0.0280 us |  0.0262 us |   0.6866 |  0.0076 |   12.67 KB |
| Scrutor_DefaultInterfaces           | Small |    18.686 us |  0.0905 us |  0.0846 us |   1.0071 |       - |   18.76 KB |
| Windsor_DefaultInterfaces           | Small |   134.955 us |  1.4715 us |  1.3764 us |  11.2305 |  1.9531 |  206.81 KB |
| ZCrew_AsSelf                        | Small |     3.242 us |  0.0191 us |  0.0160 us |   0.5035 |  0.0076 |    9.28 KB |
| Scrutor_AsSelf                      | Small |    17.061 us |  0.0609 us |  0.0570 us |   0.7935 |       - |   14.67 KB |
| Windsor_AsSelf                      | Small |   127.974 us |  0.8612 us |  0.8056 us |  10.4980 |  1.9531 |  193.07 KB |
| ZCrew_InternalTypes_AllInterfaces   | Small |     3.667 us |  0.0213 us |  0.0189 us |   0.7629 |  0.0191 |   14.04 KB |
| Scrutor_InternalTypes_AllInterfaces | Small |    23.222 us |  0.0687 us |  0.0643 us |   1.4648 |  0.0305 |   27.08 KB |
| Windsor_InternalTypes_AllInterfaces | Small |   141.719 us |  0.8514 us |  0.7964 us |  11.4746 |  1.9531 |  213.24 KB |
| ZCrew_BasedOn_AsInterface           | Small |     5.046 us |  0.0217 us |  0.0193 us |   0.7477 |       - |   13.77 KB |
| Scrutor_BasedOn_AsInterface         | Small |     3.051 us |  0.0195 us |  0.0182 us |   0.3853 |  0.0038 |    7.09 KB |
| Windsor_BasedOn_AllInterfaces       | Small |    13.941 us |  0.0709 us |  0.0663 us |   1.0529 |  0.0153 |   19.49 KB |
| ZCrew_FirstInterface                | Small |     3.466 us |  0.0308 us |  0.0288 us |   0.5074 |  0.0076 |    9.33 KB |
| Scrutor_FirstInterface              | Small |    17.272 us |  0.0254 us |  0.0237 us |   0.7935 |       - |   14.82 KB |
| Windsor_FirstInterface              | Small |   125.897 us |  2.3488 us |  2.5132 us |  10.4980 |  1.7090 |  194.83 KB |
| ZCrew_AllInterfaces                 | Large |    49.601 us |  0.1223 us |  0.0955 us |  10.7422 |  2.6855 |  197.88 KB |
| Scrutor_AllInterfaces               | Large |   334.086 us |  1.8187 us |  1.7012 us |  17.0898 |  3.4180 |  314.68 KB |
| Windsor_AllInterfaces               | Large | 2,206.090 us |  6.1620 us |  5.1456 us | 210.9375 | 74.2188 | 3909.26 KB |
| ZCrew_DefaultInterfaces             | Large |    43.211 us |  0.4040 us |  0.3779 us |   5.7373 |  0.1221 |  105.82 KB |
| Scrutor_DefaultInterfaces           | Large |   260.708 us |  1.9183 us |  1.7944 us |   9.7656 |  0.9766 |  180.89 KB |
| Windsor_DefaultInterfaces           | Large | 2,200.297 us | 29.4502 us | 27.5477 us | 210.9375 | 74.2188 |  3886.4 KB |
| ZCrew_AsSelf                        | Large |    30.996 us |  0.2707 us |  0.2399 us |   5.4932 |  0.9766 |  101.95 KB |
| Scrutor_AsSelf                      | Large |   243.776 us |  0.6436 us |  0.6020 us |   7.8125 |  1.4648 |  148.87 KB |
| Windsor_AsSelf                      | Large | 2,090.316 us | 23.8190 us | 21.1149 us | 199.2188 | 78.1250 | 3725.71 KB |
| ZCrew_InternalTypes_AllInterfaces   | Large |    72.013 us |  1.4021 us |  1.3771 us |  15.8691 |  4.7607 |  293.57 KB |
| Scrutor_InternalTypes_AllInterfaces | Large |   399.811 us |  3.3233 us |  3.1086 us |  24.9023 |  6.3477 |  462.46 KB |
| Windsor_InternalTypes_AllInterfaces | Large | 3,336.976 us | 42.1975 us | 39.4716 us | 351.5625 |  7.8125 | 6511.51 KB |
| ZCrew_BasedOn_AsInterface           | Large |    65.905 us |  0.5018 us |  0.4694 us |   9.1553 |  0.2441 |   170.4 KB |
| Scrutor_BasedOn_AsInterface         | Large |    45.609 us |  0.2890 us |  0.2703 us |   3.7231 |  0.3662 |   68.92 KB |
| Windsor_BasedOn_AllInterfaces       | Large |   272.554 us |  2.1689 us |  2.0288 us |  11.2305 |  0.9766 |  208.25 KB |
| ZCrew_FirstInterface                | Large |    41.450 us |  0.2323 us |  0.1939 us |   6.3477 |  0.9766 |  117.16 KB |
| Scrutor_FirstInterface              | Large |   259.348 us |  1.4662 us |  1.3715 us |   8.7891 |  1.4648 |  161.63 KB |
| Windsor_FirstInterface              | Large | 2,063.896 us | 24.5522 us | 22.9661 us | 199.2188 | 70.3125 |  3720.1 KB |

