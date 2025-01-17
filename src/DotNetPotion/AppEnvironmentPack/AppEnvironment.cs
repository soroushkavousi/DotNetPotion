using System;

namespace DotNetPotion.AppEnvironmentPack
{
    /// <summary>
    ///     Provides utility methods for identifying the current application environment.
    /// </summary>
    public static class AppEnvironment
    {
        static AppEnvironment()
        {
            ResolveEnvironment();
        }

        /// <summary>
        ///     Gets the current environment name in uppercase.
        /// </summary>
        public static string EnvironmentName { get; private set; }

        /// <summary>
        ///     Indicates whether the current environment is Development.
        /// </summary>
        public static bool IsDevelopment { get; private set; }

        /// <summary>
        ///     Indicates whether the current environment is Testing.
        /// </summary>
        public static bool IsTesting { get; private set; }

        /// <summary>
        ///     Indicates whether the current environment is Staging.
        /// </summary>
        public static bool IsStaging { get; private set; }

        /// <summary>
        ///     Indicates whether the current environment is Production.
        /// </summary>
        public static bool IsProduction { get; private set; }

        /// <summary>
        ///     Checks if the current environment matches the specified environment name.
        /// </summary>
        /// <param name="environmentName">The name of the environment to compare.</param>
        /// <returns>True if the current environment matches; otherwise, false.</returns>
        public static bool IsEnvironment(string environmentName)
        {
            if (string.IsNullOrWhiteSpace(environmentName))
                throw new ArgumentException("Environment name cannot be null or empty.", nameof(environmentName));

            return string.Equals(EnvironmentName, environmentName, StringComparison.OrdinalIgnoreCase);
        }

        public static void ResolveEnvironment()
        {
            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToUpperInvariant() ?? string.Empty;
            IsDevelopment = EnvironmentName == "DEVELOPMENT";
            IsTesting = EnvironmentName == "TESTING";
            IsStaging = EnvironmentName == "STAGING";
            IsProduction = EnvironmentName == "PRODUCTION";
        }
    }
}