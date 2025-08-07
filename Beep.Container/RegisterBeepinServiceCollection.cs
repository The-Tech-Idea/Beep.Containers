﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Container.Shared;
using TheTechIdea.Beep.Container.Model;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Container
{
    /// <summary>
    /// Modern, thread-safe, and robust service registration extensions for Beep framework.
    /// Implements .NET 8/9 best practices with comprehensive error handling and validation.
    /// </summary>
    public static class BeepServiceRegistration
    {
        #region Private Fields
        private static readonly object _lockObject = new object();
        private static volatile bool _isInitialized = false;
        private static volatile bool _isMappingCreated = false;
        private static IBeepService _cachedBeepService;
        private static string _beepDataPath;
        private static IServiceCollection _currentServices;
        #endregion

        #region Core Registration Methods

        /// <summary>
        /// Registers Beep services with comprehensive configuration and error handling.
        /// This is the primary registration method with full feature support.
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <param name="configure">Configuration action for Beep service options.</param>
        /// <returns>The configured IBeepService instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when services or configure is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when services are already registered or configuration fails.</exception>
        public static IBeepService AddBeepServices(this IServiceCollection services, 
            Action<BeepServiceOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var options = new BeepServiceOptions();
            configure(options);
            options.Validate();

            return RegisterBeepServicesInternal(services, options);
        }

        /// <summary>
        /// Registers Beep services with simple parameters (backward compatibility).
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <param name="directoryPath">Directory path for Beep data storage.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="configType">Type of configuration.</param>
        /// <param name="addAsSingleton">Whether to register as singleton (default: true).</param>
        /// <returns>The configured IBeepService instance.</returns>
        public static IBeepService Register(this IServiceCollection services, 
            string directoryPath, 
            string containerName, 
            BeepConfigType configType, 
            bool addAsSingleton = true)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));

            var options = new BeepServiceOptions
            {
                DirectoryPath = directoryPath,
                ContainerName = containerName,
                ConfigType = configType,
                ServiceLifetime = addAsSingleton ? ServiceLifetime.Singleton : ServiceLifetime.Scoped,
                EnableAutoMapping = true,
                EnableAssemblyLoading = true
            };

            return RegisterBeepServicesInternal(services, options);
        }

        /// <summary>
        /// Registers Beep services as scoped services (for web applications).
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection RegisterScoped(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            lock (_lockObject)
            {
                if (_isInitialized)
                {
                    throw new InvalidOperationException("Beep services have already been registered. " +
                        "Multiple registrations are not supported.");
                }

                services.AddScoped<IBeepService>(serviceProvider =>
                {
                    if (_cachedBeepService != null)
                        return _cachedBeepService;

                    return new BeepService(services);
                });

                _isInitialized = true;
                _currentServices = services;
                return services;
            }
        }

        #endregion

        #region Mapping and Configuration Methods

        /// <summary>
        /// Creates mappings for the Beep service with error handling and logging.
        /// </summary>
        /// <param name="beepService">The Beep service instance.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection CreateMapping(this IBeepService beepService)
        {
            if (beepService == null)
                throw new ArgumentNullException(nameof(beepService));

            lock (_lockObject)
            {
                if (_isMappingCreated)
                {
                    // Already created, return early
                    return GetCurrentServices();
                }

                try
                {
                    ContainerMisc.AddAllConnectionConfigurations(beepService);
                    ContainerMisc.AddAllDataSourceMappings(beepService);
                    ContainerMisc.AddAllDataSourceQueryConfigurations(beepService);
                    
                    _isMappingCreated = true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to create Beep mappings.", ex);
                }

                return GetCurrentServices();
            }
        }

        /// <summary>
        /// Creates mappings asynchronously with progress reporting and cancellation support.
        /// </summary>
        /// <param name="beepService">The Beep service instance.</param>
        /// <param name="progress">Optional progress reporter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task representing the async operation.</returns>
        public static async Task CreateMappingAsync(this IBeepService beepService,
            IProgress<PassedArgs> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (beepService == null)
                throw new ArgumentNullException(nameof(beepService));

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                progress?.Report(new PassedArgs { 
                    Messege = "Creating connection configurations...",
                    EventType = "Progress"
                });
                
                ContainerMisc.AddAllConnectionConfigurations(beepService);
                
                cancellationToken.ThrowIfCancellationRequested();
                
                progress?.Report(new PassedArgs { 
                    Messege = "Creating data source mappings...",
                    EventType = "Progress"
                });
                
                ContainerMisc.AddAllDataSourceMappings(beepService);
                
                cancellationToken.ThrowIfCancellationRequested();
                
                progress?.Report(new PassedArgs { 
                    Messege = "Creating query configurations...",
                    EventType = "Progress"
                });
                
                ContainerMisc.AddAllDataSourceQueryConfigurations(beepService);
                
                progress?.Report(new PassedArgs { 
                    Messege = "Mapping creation completed successfully.",
                    EventType = "Completed"
                });
                
            }, cancellationToken);

            lock (_lockObject)
            {
                _isMappingCreated = true;
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets or creates the main Beep data folder.
        /// </summary>
        /// <returns>Path to the main Beep data folder.</returns>
        public static string GetMainFolder()
        {
            if (string.IsNullOrEmpty(_beepDataPath))
            {
                _beepDataPath = ContainerMisc.CreateMainFolder();
            }
            return _beepDataPath;
        }

        /// <summary>
        /// Gets the configured Beep service instance from IDMEEditor.
        /// This method provides backward compatibility with existing code.
        /// </summary>
        /// <param name="dmeEditor">The DME Editor instance.</param>
        /// <returns>The configured IBeepService instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when Beep services are not properly registered.</exception>
        public static IBeepService GetBeepService(this IDMEEditor dmeEditor)
        {
            if (_cachedBeepService == null)
            {
                throw new InvalidOperationException("Beep services have not been registered. " +
                    "Call Register() or AddBeepServices() first.");
            }
            return _cachedBeepService;
        }

        /// <summary>
        /// Gets the Beep service instance from the service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The configured IBeepService instance.</returns>
        public static IBeepService GetBeepService(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            return serviceProvider.GetRequiredService<IBeepService>();
        }

        /// <summary>
        /// Validates the current Beep service configuration.
        /// </summary>
        /// <returns>True if configuration is valid; otherwise, false.</returns>
        public static bool ValidateConfiguration()
        {
            return _cachedBeepService?.ValidateConfiguration() ?? false;
        }

        /// <summary>
        /// Gets configuration summary for debugging purposes.
        /// </summary>
        /// <returns>Configuration summary string.</returns>
        public static string GetConfigurationSummary()
        {
            if (_cachedBeepService == null)
                return "Beep services not initialized.";

            return $"Container: {_cachedBeepService.Containername}, " +
                   $"Type: {_cachedBeepService.ConfigureationType}, " +
                   $"Directory: {_cachedBeepService.BeepDirectory}, " +
                   $"Initialized: {_isInitialized}, " +
                   $"Mapping Created: {_isMappingCreated}";
        }

        /// <summary>
        /// Resets the registration state (primarily for testing purposes).
        /// </summary>
        /// <param name="force">Whether to force reset even if services are running.</param>
        public static void ResetRegistrationState(bool force = false)
        {
            if (!force && (_cachedBeepService?.DMEEditor != null))
            {
                throw new InvalidOperationException("Cannot reset while services are active. " +
                    "Use force=true to override.");
            }

            lock (_lockObject)
            {
                _isInitialized = false;
                _isMappingCreated = false;
                _cachedBeepService = null;
                _beepDataPath = null;
                _currentServices = null;
            }
        }

        #endregion

        #region Private Implementation Methods

        /// <summary>
        /// Internal method for registering Beep services with comprehensive error handling.
        /// </summary>
        private static IBeepService RegisterBeepServicesInternal(IServiceCollection services, BeepServiceOptions options)
        {
            lock (_lockObject)
            {
                if (_isInitialized)
                {
                    if (_cachedBeepService != null)
                        return _cachedBeepService;
                    
                    throw new InvalidOperationException("Beep services registration is in progress or failed. " +
                        "Multiple concurrent registrations are not supported.");
                }

                try
                {
                    _currentServices = services;

                    // Create and configure BeepService
                    _cachedBeepService = CreateBeepServiceInstance(services, options);

                    // Register based on specified lifetime
                    RegisterServiceWithLifetime(services, _cachedBeepService, options.ServiceLifetime);

                    // Initialize folder structure
                    _beepDataPath = ContainerMisc.CreateMainFolder();

                    // Create mappings if enabled
                    if (options.EnableAutoMapping)
                    {
                        BeepServiceRegistration.CreateMapping(_cachedBeepService);
                    }

                    _isInitialized = true;
                    return _cachedBeepService;
                }
                catch (Exception ex)
                {
                    // Reset state on failure
                    _isInitialized = false;
                    _cachedBeepService = null;
                    throw new InvalidOperationException("Failed to register Beep services.", ex);
                }
            }
        }

        /// <summary>
        /// Creates a BeepService instance with proper dependency injection.
        /// </summary>
        private static IBeepService CreateBeepServiceInstance(IServiceCollection services, BeepServiceOptions options)
        {
            var beepService = new BeepService(services);
            beepService.Configure(
                options.DirectoryPath,
                options.ContainerName,
                options.ConfigType,
                options.ServiceLifetime == ServiceLifetime.Singleton);

            return beepService;
        }

        /// <summary>
        /// Registers the service with the specified lifetime.
        /// </summary>
        private static void RegisterServiceWithLifetime(IServiceCollection services, IBeepService beepService, ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IBeepService>(beepService);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<IBeepService>(_ => beepService);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<IBeepService>(_ => beepService);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unsupported service lifetime.");
            }
        }

        /// <summary>
        /// Gets the current service collection.
        /// </summary>
        private static IServiceCollection GetCurrentServices()
        {
            return _currentServices ?? new ServiceCollection();
        }

        /// <summary>
        /// Extension method to check if a service is already registered.
        /// </summary>
        private static bool HasService<T>(this IServiceCollection services)
        {
            return services.Any(x => x.ServiceType == typeof(T));
        }

        #endregion
    }

    #region Configuration Classes

    /// <summary>
    /// Configuration options for Beep services with comprehensive validation.
    /// </summary>
    public class BeepServiceOptions
    {
        /// <summary>
        /// Gets or sets the directory path for Beep data storage.
        /// </summary>
        public string DirectoryPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        public string ContainerName { get; set; } = "DefaultContainer";

        /// <summary>
        /// Gets or sets the configuration type.
        /// </summary>
        public BeepConfigType ConfigType { get; set; } = BeepConfigType.Application;

        /// <summary>
        /// Gets or sets the service lifetime for dependency injection.
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Singleton;

        /// <summary>
        /// Gets or sets whether to automatically create mappings during initialization.
        /// </summary>
        public bool EnableAutoMapping { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to automatically load assemblies during initialization.
        /// </summary>
        public bool EnableAssemblyLoading { get; set; } = true;

        /// <summary>
        /// Gets or sets the timeout for initialization operations.
        /// </summary>
        public TimeSpan InitializationTimeout { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Gets or sets whether to validate configuration during startup.
        /// </summary>
        public bool EnableConfigurationValidation { get; set; } = true;

        /// <summary>
        /// Gets or sets additional configuration properties for extensibility.
        /// </summary>
        public Dictionary<string, object> AdditionalProperties { get; set; } = new();

        /// <summary>
        /// Validates the configuration options and throws descriptive exceptions for invalid configurations.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when configuration is invalid.</exception>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(DirectoryPath))
                throw new ArgumentException("DirectoryPath cannot be null or empty.", nameof(DirectoryPath));

            if (string.IsNullOrWhiteSpace(ContainerName))
                throw new ArgumentException("ContainerName cannot be null or empty.", nameof(ContainerName));

            if (InitializationTimeout <= TimeSpan.Zero)
                throw new ArgumentException("InitializationTimeout must be positive.", nameof(InitializationTimeout));

            if (!Enum.IsDefined(typeof(BeepConfigType), ConfigType))
                throw new ArgumentException("Invalid ConfigType value.", nameof(ConfigType));

            if (!Enum.IsDefined(typeof(ServiceLifetime), ServiceLifetime))
                throw new ArgumentException("Invalid ServiceLifetime value.", nameof(ServiceLifetime));
        }
    }

    #endregion

    #region Extension Methods

    /// <summary>
    /// Extension methods for IBeepService to provide additional functionality.
    /// </summary>
    public static class BeepServiceExtensions
    {
        /// <summary>
        /// Validates the BeepService configuration.
        /// </summary>
        /// <param name="beepService">The BeepService instance.</param>
        /// <returns>True if configuration is valid; otherwise, false.</returns>
        public static bool ValidateConfiguration(this IBeepService beepService)
        {
            if (beepService == null) return false;

            return !string.IsNullOrWhiteSpace(beepService.Containername) &&
                   !string.IsNullOrWhiteSpace(beepService.BeepDirectory) &&
                   beepService.Config_editor != null;
        }

        /// <summary>
        /// Gets configuration summary for the BeepService.
        /// </summary>
        /// <param name="beepService">The BeepService instance.</param>
        /// <returns>Configuration summary as a formatted string.</returns>
        public static string GetConfigurationSummary(this IBeepService beepService)
        {
            if (beepService == null) return "BeepService is null";

            return $"Container: {beepService.Containername}, " +
                   $"Type: {beepService.ConfigureationType}, " +
                   $"Directory: {beepService.BeepDirectory}";
        }
    }

    #endregion

    #region Legacy Compatibility

    /// <summary>
    /// Legacy RegisterBeep class for backward compatibility.
    /// This class maintains the exact same interface as the original but delegates to the new implementation.
    /// </summary>
    public static class RegisterBeep
    {
        private static IServiceCollection _services;

        /// <summary>
        /// Gets the current service collection for backward compatibility.
        /// </summary>
        public static IServiceCollection Services 
        { 
            get => _services ?? new ServiceCollection();
            private set => _services = value;
        }

        /// <summary>
        /// Registers Beep services with the original interface for backward compatibility.
        /// </summary>
        public static IBeepService Register(this IServiceCollection services, string directorypath, string containername, BeepConfigType configType, bool AddasSingleton = true)
        {
            Services = services;
            var beepService = BeepServiceRegistration.Register(services, directorypath, containername, configType, AddasSingleton);
            BeepServiceRegistration.CreateMapping(beepService);
            return beepService;
        }

        /// <summary>
        /// Registers scoped services for backward compatibility.
        /// </summary>
        public static IServiceCollection RegisterScoped(this IServiceCollection services)
        {
            Services = services;
            return BeepServiceRegistration.RegisterScoped(services);
        }

        /// <summary>
        /// Creates mappings for backward compatibility.
        /// </summary>
        public static IServiceCollection CreateMapping(this IBeepService beepService)
        {
            return BeepServiceRegistration.CreateMapping(beepService);
        }

        /// <summary>
        /// Gets the main folder for backward compatibility.
        /// </summary>
        public static string GetMainFolder()
        {
            return BeepServiceRegistration.GetMainFolder();
        }

        /// <summary>
        /// Gets the Beep service for backward compatibility.
        /// </summary>
        public static IBeepService GetBeepService(this IDMEEditor dmeEditor)
        {
            return BeepServiceRegistration.GetBeepService(dmeEditor);
        }
    }

    #endregion
}
