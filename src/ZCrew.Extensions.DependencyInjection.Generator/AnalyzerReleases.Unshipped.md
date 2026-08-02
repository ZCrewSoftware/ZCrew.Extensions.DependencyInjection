; Unshipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
ZCDI001 | ZCrew.Extensions.DependencyInjection.Registration | Error | Registration key ([Keyed]/[As]) cannot be an array
ZCDI002 | ZCrew.Extensions.DependencyInjection.Registration | Error | Registration modifier requires [Service]
ZCDI003 | ZCrew.Extensions.DependencyInjection.Registration | Error | [As] service type is not assignable from the implementation
ZCDI004 | ZCrew.Extensions.DependencyInjection.Registration | Error | Conflicting lifetime attributes
