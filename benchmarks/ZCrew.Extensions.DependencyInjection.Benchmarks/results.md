```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
Intel Core i9-14900K 3.20GHz, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.401
  [Host] : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3

Toolchain=InProcessEmitToolchain

```
| Method                              | Size  |         Mean |      Error |     StdDev |     Gen0 |    Gen1 |  Allocated |
|-------------------------------------|-------|-------------:|-----------:|-----------:|---------:|--------:|-----------:|
| ZCrew_AllInterfaces                 | Small |     9.499 us |  0.0504 us |  0.0471 us |   1.2665 |  0.0305 |   23.55 KB |
| Scrutor_AllInterfaces               | Small |    42.588 us |  0.3369 us |  0.3152 us |   2.4414 |  0.0610 |   44.96 KB |
| Windsor_AllInterfaces               | Small |   341.060 us |  6.3209 us |  5.9126 us |  27.3438 |  7.8125 |   504.8 KB |
| ZCrew_DefaultInterfaces             | Small |     9.995 us |  0.0690 us |  0.0645 us |   1.3123 |  0.0153 |   24.38 KB |
| Scrutor_DefaultInterfaces           | Small |    41.138 us |  0.1535 us |  0.1436 us |   1.6479 |  0.0610 |   30.51 KB |
| Windsor_DefaultInterfaces           | Small |   333.972 us |  3.0064 us |  2.8122 us |  27.3438 |  6.8359 |  507.01 KB |
| ZCrew_AsSelf                        | Small |    10.025 us |  0.0553 us |  0.0490 us |   1.9684 |  0.0458 |   36.41 KB |
| Scrutor_AsSelf                      | Small |    40.657 us |  0.1421 us |  0.1329 us |   1.7090 |  0.0610 |   31.75 KB |
| Windsor_AsSelf                      | Small |   320.363 us |  2.6010 us |  2.4330 us |  25.8789 |  7.3242 |  483.19 KB |
| ZCrew_InternalTypes_AllInterfaces   | Small |     9.337 us |  0.0512 us |  0.0478 us |   1.3123 |  0.0305 |   24.17 KB |
| Scrutor_InternalTypes_AllInterfaces | Small |    47.932 us |  0.2129 us |  0.1991 us |   2.4414 |  0.0610 |   45.44 KB |
| Windsor_InternalTypes_AllInterfaces | Small |   339.093 us |  2.0293 us |  1.8982 us |  27.8320 |  7.3242 |  511.77 KB |
| ZCrew_BasedOn_AsInterface           | Small |     2.740 us |  0.0137 us |  0.0114 us |   0.1259 |       - |    2.33 KB |
| Scrutor_BasedOn_AsInterface         | Small |     5.767 us |  0.0297 us |  0.0278 us |   0.7095 |  0.0153 |   13.07 KB |
| Windsor_BasedOn_AllInterfaces       | Small |    28.992 us |  0.2401 us |  0.2129 us |   1.6785 |  0.0305 |    30.9 KB |
| ZCrew_FirstInterface                | Small |     9.318 us |  0.0422 us |  0.0395 us |   1.1444 |  0.0153 |   21.19 KB |
| Scrutor_FirstInterface              | Small |    39.292 us |  0.1556 us |  0.1456 us |   1.2207 |       - |   22.88 KB |
| Windsor_FirstInterface              | Small |   324.283 us |  3.2775 us |  3.0657 us |  25.8789 |  7.3242 |  484.19 KB |
| ZCrew_AllInterfaces                 | Large |    90.821 us |  0.4993 us |  0.4671 us |  13.9160 |  2.4414 |  255.91 KB |
| Scrutor_AllInterfaces               | Large |   293.919 us |  1.1857 us |  1.1091 us |  17.0898 |  3.4180 |  314.68 KB |
| Windsor_AllInterfaces               | Large | 2,118.509 us |  9.2134 us |  8.6182 us | 210.9375 | 74.2188 | 3909.56 KB |
| ZCrew_DefaultInterfaces             | Large |    66.176 us |  0.5352 us |  0.5006 us |   5.9814 |  0.1221 |  111.67 KB |
| Scrutor_DefaultInterfaces           | Large |   257.118 us |  1.1051 us |  0.9797 us |   8.3008 |  0.4883 |  157.83 KB |
| Windsor_DefaultInterfaces           | Large | 2,100.112 us |  9.0790 us |  8.4925 us | 210.9375 | 74.2188 | 3886.19 KB |
| ZCrew_AsSelf                        | Large |    45.421 us |  0.3414 us |  0.3194 us |   9.8877 |  1.0376 |   182.7 KB |
| Scrutor_AsSelf                      | Large |   243.003 us |  1.2842 us |  1.2012 us |   8.0566 |  1.4648 |  148.87 KB |
| Windsor_AsSelf                      | Large | 1,948.270 us | 23.2663 us | 21.7633 us | 199.2188 | 78.1250 | 3725.75 KB |
| ZCrew_InternalTypes_AllInterfaces   | Large |   125.741 us |  1.1142 us |  0.9304 us |  20.2637 |  3.9063 |  375.08 KB |
| Scrutor_InternalTypes_AllInterfaces | Large |   386.554 us |  2.4641 us |  2.1843 us |  24.9023 |  5.8594 |  462.46 KB |
| Windsor_InternalTypes_AllInterfaces | Large | 3,142.676 us | 15.6272 us | 14.6177 us | 351.5625 |  7.8125 | 6511.51 KB |
| ZCrew_BasedOn_AsInterface           | Large |    19.381 us |  0.1378 us |  0.1289 us |   1.4343 |  0.0305 |   26.46 KB |
| Scrutor_BasedOn_AsInterface         | Large |    44.947 us |  0.3225 us |  0.3017 us |   3.7231 |  0.3662 |    69.3 KB |
| Windsor_BasedOn_AllInterfaces       | Large |   262.518 us |  1.6616 us |  1.5543 us |  11.2305 |  1.4648 |  208.64 KB |
| ZCrew_FirstInterface                | Large |    75.428 us |  0.3683 us |  0.3445 us |  10.7422 |  1.0986 |  198.43 KB |
| Scrutor_FirstInterface              | Large |   257.584 us |  1.0219 us |  0.9558 us |   8.7891 |  1.4648 |  161.61 KB |
| Windsor_FirstInterface              | Large | 1,950.639 us | 13.0768 us | 11.5923 us | 201.1719 | 70.3125 | 3718.91 KB |

