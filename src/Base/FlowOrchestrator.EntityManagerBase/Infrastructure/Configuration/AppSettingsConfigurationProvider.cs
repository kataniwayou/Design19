using Microsoft.Extensions.Configuration;
using IConfigurationProvider = FlowOrchestrator.Common.Configuration.IConfigurationProvider;
using IConfigurationSection = FlowOrchestrator.Common.Configuration.IConfigurationSection;

namespace FlowOrchestrator.EntityManagerBase.Infrastructure.Configuration;

/// <summary>
/// AppSettings configuration provider implementation.
/// </summary>
public class AppSettingsConfigurationProvider : IConfigurationProvider
{
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsConfigurationProvider"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public AppSettingsConfigurationProvider(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets a configuration value by key.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    public T GetValue<T>(string key)
    {
        return _configuration.GetValue<T>(key)!;
    }

    /// <summary>
    /// Gets a configuration value by key, or returns the default value if the key is not found.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The configuration value, or the default value if the key is not found.</returns>
    public T GetValue<T>(string key, T defaultValue)
    {
        return _configuration.GetValue<T>(key, defaultValue)!;
    }

    /// <summary>
    /// Gets a configuration section by key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration section.</returns>
    public FlowOrchestrator.Common.Configuration.IConfigurationSection GetSection(string key)
    {
        var section = _configuration.GetSection(key);
        return new ConfigurationSection(section);
    }

    /// <summary>
    /// Binds a configuration section to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The bound object.</returns>
    public T Bind<T>(string key) where T : new()
    {
        var result = new T();
        _configuration.GetSection(key).Bind(result);
        return result;
    }

    private class ConfigurationSection : FlowOrchestrator.Common.Configuration.IConfigurationSection
    {
        private readonly Microsoft.Extensions.Configuration.IConfigurationSection _section;

        public ConfigurationSection(Microsoft.Extensions.Configuration.IConfigurationSection section)
        {
            _section = section;
        }

        public string Key => _section.Key;

        public string Path => _section.Path;

        public string? Value => _section.Value;

        public T GetValue<T>(string key)
        {
            return _section.GetValue<T>(key)!;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            return _section.GetValue<T>(key, defaultValue)!;
        }

        public FlowOrchestrator.Common.Configuration.IConfigurationSection GetSection(string key)
        {
            var section = _section.GetSection(key);
            return new ConfigurationSection(section);
        }

        public T Bind<T>() where T : new()
        {
            var result = new T();
            _section.Bind(result);
            return result;
        }
    }
}
