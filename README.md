# Beep Containers

Beep Containers is a library within the [BeepDM Framework](https://github.com/The-Tech-Idea/BeepDM), developed by [The-Tech-Idea](https://github.com/The-Tech-Idea). It provides dependency injection (DI) and container management capabilities using Microsoft.Extensions.DependencyInjection, enabling service registration and feature loading for BeepDM applications.

## Features
- **Service Registration**: Register `IBeepService` and `ICantainerManager` in an IServiceCollection.
- **Container Management**: Manage a list of `IBeepContainer` instances with JSON persistence.
- **Feature Loading**: Scan assemblies for `IBeepFeature` implementations.
- **File System Integration**: Create container directories and extract ZIP files for setup.
- **Configuration Support**: Configure Beep services with directory paths and container names.

## Installation
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/The-Tech-Idea/Beep.Containers.git
   ```
2. **Build in Visual Studio**:
   - Open the solution in Visual Studio 2019 or later.
   - Build the project to generate the DLL.

## Usage
- **Register Container Manager**:
  ```csharp
  var services = new ServiceCollection();
  services.AddBeepContainerManager();
  var provider = services.BuildServiceProvider();
  var manager = provider.GetService<ICantainerManager>();
  ```
- **Add a Beep Container**:
  ```csharp
  services.AddBeepContainer(@"C:\BeepData", "MyContainer", BeepConfigType.Container);
  var beepService = provider.GetService<IBeepService>();
  ```
- **Create a Container**:
  ```csharp
  await manager.CreateContainer("guid123", "user", "user@example.com", 1, "user-guid", "MyContainer");
  ```
- **Load Features**:
  ```csharp
  var loader = new FeatureLoader(assemblyHandler);
  loader.Scan();
  ```

## IBeepService
`IBeepService` is a core interface implemented by `BeepService`, serving as the backbone for configuring and managing BeepDM components within a containerized environment. It integrates with the DI container to provide access to essential services and handles configuration loading, making it a critical component for initializing BeepDM applications.

### Key Capabilities
- **Configuration**:
  - Initializes BeepDM components with a directory path, container name, and configuration type (`BeepConfigType`).
  - Example:
    ```csharp
    beepService.Configure(@"C:\BeepData", "MyContainer", BeepConfigType.Container, true);
    ```
- **Service Registration**:
  - Registers BeepDM services (e.g., `IDMEEditor`, `IConfigEditor`, `IDMLogger`) as singletons or scoped instances.
  - Supports both singleton and scoped lifetimes:
    ```csharp
    Services.AddSingleton<IDMEEditor>(DMEEditor); // Singleton
    Services.AddScoped<IDMEEditor, DMEEditor>();  // Scoped
    ```
- **Configuration Loading**:
  - Loads connection configurations, data source mappings, and query configurations via `ContainerMisc`:
    ```csharp
    ContainerMisc.AddAllConnectionConfigurations(beepService);
    ContainerMisc.AddAllDataSourceMappings(beepService);
    ContainerMisc.AddAllDataSourceQueryConfigurations(beepService);
    ```
- **Assembly Loading**:
  - Loads assemblies synchronously or asynchronously with progress reporting:
    ```csharp
    beepService.LoadAssemblies(); // Synchronous
    await beepService.LoadAssembliesAsync(new Progress<PassedArgs>(args => Console.WriteLine(args.ParameterString1))); // Asynchronous
    ```
- **Environment Management**:
  - Loads and saves environment configurations from/to JSON files in an "Environments" subdirectory:
    ```csharp
    beepService.LoadEnvironments();
    beepService.SaveEnvironments();
    ```
- **File System Setup**:
  - Creates a main Beep folder (`%ProgramData%\TheTechIdea\Beep`) and container-specific subfolders:
    ```csharp
    ContainerMisc.CreateMainFolder(beepService);
    ContainerMisc.CreateContainerfolder("MyContainer");
    ```
- **Component Access**:
  - Provides properties for accessing BeepDM components:
    - `IDMEEditor DMEEditor`: Editor instance.
    - `IConfigEditor Config_editor`: Configuration manager.
    - `IDMLogger lg`: Logger instance.
    - `IAssemblyHandler LLoader`: Assembly loader.

### Implementation Details
- **Constructor**: Accepts an `IServiceCollection` for DI integration:
  ```csharp
  public BeepService(IServiceCollection services)
  ```
- **Design-Time Support**: Configures for design-time use in a designer context:
  ```csharp
  if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
  {
      ConfigureForDesignTime();
  }
  ```
- **Disposal**: Implements `IDisposable` to clean up resources:
  ```csharp
  public void Dispose()
  {
      Dispose(true);
      GC.SuppressFinalize(this);
  }
  ```

### Example Application Usage
Below is an example from a WinForms application demonstrating `IBeepService` integration with both Microsoft.Extensions.DependencyInjection and Autofac:

#### Using Microsoft.Extensions.DependencyInjection
```csharp
static void Main()
{
    ApplicationConfiguration.Initialize();
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    HostApplicationBuilder builder = Host.CreateApplicationBuilder();
    BeepServicesRegister.RegisterServices(builder); // Registers IBeepService
    using IHost host = builder.Build();
    BeepServicesRegister.ConfigureServices(host);

    var appManager = host.Services.GetService<IAppManager>();
    appManager.ConfigureAppManager(config =>
    {
        config.Title = "Beep Data Management Platform";
        config.Theme = EnumBeepThemes.CandyTheme;
        config.IconUrl = "simpleinfoapps.ico";
        config.LogoUrl = "simpleinfoapps.svg";
        config.HomePageName = "Form1";
    });

    BeepAppServices.visManager = appManager;
    BeepAppServices.beepService = host.Services.GetService<IBeepService>();
    BeepAppServices.StartLoading(new string[] { "BeepEnterprize", "TheTechIdea", "Beep" });
    BeepAppServices.RegisterRoutes();
    BeepServicesRegister.ShowHome();
    BeepServicesRegister.DisposeServices(host.Services);
}
```

#### Using Autofac
```csharp
static void StartAppUsingAutoFac()
{
    var builder = new ContainerBuilder();
    BeepServicesRegisterAutFac.RegisterServices(builder); // Registers IBeepService
    RegisterBeepWinformServices.RegisterControlManager(builder);
    var container = builder.Build();

    BeepServicesRegisterAutFac.ConfigureServices(container);
    BeepAppServices.visManager = BeepServicesRegisterAutFac.AppManager;
    BeepAppServices.beepService = BeepServicesRegisterAutFac.beepService;

    BeepServicesRegisterAutFac.AppManager.Title = "Beep Data Management Platform";
    BeepServicesRegisterAutFac.AppManager.Theme = EnumBeepThemes.DefaultTheme;
    BeepServicesRegisterAutFac.AppManager.IconUrl = "simpleinfoapps.ico";
    BeepServicesRegisterAutFac.AppManager.LogoUrl = "simpleinfoapps.svg";
    BeepServicesRegisterAutFac.AppManager.HomePageName = "MainForm";

    BeepAppServices.StartLoading(new string[] { "BeepEnterprize", "TheTechIdea", "Beep" });
    BeepAppServices.RegisterRoutes();
    BeepServicesRegisterAutFac.ShowHome();
    BeepServicesRegisterAutFac.DisposeServices();
}
```

In both examples, `IBeepService` is resolved from the DI container and used to initialize the BeepDM environment, demonstrating its role as a foundational service.

## Project Structure
Based on the provided files, the key components are:

### TheTechIdea.Beep.Container.ContainerManagement
- **Key Files**:
  - `RegisterContainerManagerServiceCollection.cs`: Registers `ICantainerManager` and `IBeepContainer`.
  - `CantainerManager.cs`: Manages a list of containers with JSON persistence.

### TheTechIdea.Beep.Container.FeatureManagement
- **Key Files**:
  - `BeepFeature.cs`: Defines a feature with metadata.
  - `FeatureLoader.cs`: Scans assemblies for `IBeepFeature` types.

### TheTechIdea.Beep.Container.Services
- **Key Files**:
  - `BeepService.cs`: Implements `IBeepService` for BeepDM component management.
  - `ServiceHelper.cs`: Provides static
