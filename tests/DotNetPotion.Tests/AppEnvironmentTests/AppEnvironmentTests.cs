using DotNetPotion.AppEnvironmentPack;

namespace DotNetPotion.Tests.AppEnvironmentTests;

public class AppEnvironmentTests
{
    private const string _environmentVariableName = "ASPNETCORE_ENVIRONMENT";

    [Theory]
    [InlineData("Development", "DEVELOPMENT", true, false, false, false)]
    [InlineData("Testing", "TESTING", false, true, false, false)]
    [InlineData("Staging", "STAGING", false, false, true, false)]
    [InlineData("Production", "PRODUCTION", false, false, false, true)]
    [InlineData("CustomEnv", "CUSTOMENV", false, false, false, false)]
    public void Environment_Input_ShouldResolveCorrectly(
        string environmentName,
        string expectedUppercaseName,
        bool isDevelopment,
        bool isTesting,
        bool isStaging,
        bool isProduction)
    {
        // Arrange
        SetEnvironment(environmentName);

        // Act & Assert
        Assert.Equal(expectedUppercaseName, AppEnvironment.EnvironmentName);
        Assert.Equal(isDevelopment, AppEnvironment.IsDevelopment);
        Assert.Equal(isTesting, AppEnvironment.IsTesting);
        Assert.Equal(isStaging, AppEnvironment.IsStaging);
        Assert.Equal(isProduction, AppEnvironment.IsProduction);
    }

    private static void SetEnvironment(string environmentName)
    {
        Environment.SetEnvironmentVariable(_environmentVariableName, environmentName);
        AppEnvironment.ResolveEnvironment();
    }
}