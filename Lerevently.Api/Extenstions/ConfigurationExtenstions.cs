namespace Lerevently.Api.Extenstions;

internal static class ConfigurationExtenstions
{
    internal static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder, string[] modules)
    {
        foreach (var module in modules)
        {
            configurationBuilder.AddJsonFile($"modules.{module}.json", optional: false, reloadOnChange: true);
            configurationBuilder.AddJsonFile($"modules.{module}.Development.json", optional: false, reloadOnChange: true);
        }
    }
}