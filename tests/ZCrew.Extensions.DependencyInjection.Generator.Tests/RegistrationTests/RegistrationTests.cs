using Microsoft.CodeAnalysis.Testing;
using ZCrew.Extensions.DependencyInjection.Generator.Tests.TestHelpers;
using ZCrew.Extensions.CodeAnalysis.CSharp.Testing;

namespace ZCrew.Extensions.DependencyInjection.Generator.Tests.RegistrationTests;

public class RegistrationTests
{
    private static readonly TestPath TestCases = TestPath.ForCaller() / "TestCases";

    private static readonly RoslynTestBuilder<DefaultVerifier> ServiceBaseline =
        GeneratorTest.CreateBaseline<ServiceRegistrationSourceGenerator>();

    [Theory]
    [InlineData("ServiceScenarios.json")]
    [InlineData("ServiceEmpty.json")]
    public async Task Service_WithTestCase_ShouldGenerateEntryPoint(string testDescriptor)
    {
        // Arrange
        var testCase = await JsonTestCase.FromJsonFileAsync(
            TestCases / testDescriptor,
            TestContext.Current.CancellationToken
        );

        // Act
        var test = await ServiceBaseline.BuildAsync(testCase, TestContext.Current.CancellationToken);

        // Assert
        await test.RunAsync(TestContext.Current.CancellationToken);
    }
}
