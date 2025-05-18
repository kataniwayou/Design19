namespace FlowOrchestrator.Common.Configuration;

/// <summary>
/// Defines the contract for a configuration provider.
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// Gets a configuration value by key.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    T GetValue<T>(string key);

    /// <summary>
    /// Gets a configuration value by key, or returns the default value if the key is not found.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The configuration value, or the default value if the key is not found.</returns>
    T GetValue<T>(string key, T defaultValue);

    /// <summary>
    /// Gets a configuration section by key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration section.</returns>
    IConfigurationSection GetSection(string key);

    /// <summary>
    /// Binds a configuration section to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The bound object.</returns>
    T Bind<T>(string key) where T : new();
}

/// <summary>
/// Defines the contract for a configuration section.
/// </summary>
public interface IConfigurationSection
{
    /// <summary>
    /// Gets the key of the configuration section.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the path of the configuration section.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets the value of the configuration section.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Gets a configuration value by key.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    T GetValue<T>(string key);

    /// <summary>
    /// Gets a configuration value by key, or returns the default value if the key is not found.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The configuration value, or the default value if the key is not found.</returns>
    T GetValue<T>(string key, T defaultValue);

    /// <summary>
    /// Gets a configuration section by key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration section.</returns>
    IConfigurationSection GetSection(string key);

    /// <summary>
    /// Binds the configuration section to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>The bound object.</returns>
    T Bind<T>() where T : new();
}
