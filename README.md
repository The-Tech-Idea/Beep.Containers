# Beep.Containers

Beep.Containers is a modular extension library for the [BeepDM Framework](https://github.com/The-Tech-Idea/BeepDM), built by [The-Tech-Idea](https://github.com/The-Tech-Idea). It offers flexible dependency injection, robust container management, feature/plugin loading, and integrated file system and configuration support for BeepDM-powered .NET applications.

---

## Table of Contents

- [Overview](#overview)
- [Integration with BeepDM](#integration-with-beepdm)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [BeepService](#beepservice-in-depth)
- [ContainerMisc](#containermisc-in-depth)
- [Advanced Topics](#advanced-topics)
- [Related Projects](#related-projects)
- [License](#license)

---

## Overview

Beep.Containers provides a foundation for building modular, containerized, and feature-rich data management platforms on top of BeepDM. It standardizes DI registration, enables dynamic feature loading, automates environment setup, and simplifies multi-container orchestration.

---

## Integration with BeepDM

**Beep.Containers is based on [BeepDM](https://github.com/The-Tech-Idea/BeepDM).**  
BeepDM is a core .NET library for orchestrating connections to multiple data sources, handling configuration, and offering extensible management of data-driven applications.  
Beep.Containers extends this by offering advanced container management, feature/plugin discovery, and robust integration patterns for scalable, modular application design.

---

## Features

- **Service Registration**: Register `IBeepService` and `ICantainerManager` using either Microsoft.Extensions.DependencyInjection or Autofac.
- **Container Management**: Manage lists of `IBeepContainer` instances with JSON-based persistence.
- **Feature/Plugin Loading**: Scan assemblies for `IBeepFeature` implementations and dynamically load plugins at runtime.
- **File System Integration**: Create main and container-specific directories, extract ZIPs, and manage environment files.
- **Configuration Support**: Configure Beep services with directory paths, container names, and custom config types.
- **Environment Management**: Save/load environment setups as JSON, supporting container isolation.
- **Error Handling**: Consistent, structured error reporting and logging.
- **Extensible**: Easily add new data sources, features, or management patterns.

---

## Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/The-Tech-Idea/Beep.Containers.git
   ```
2. **Build in Visual Studio**:
   - Open the solution in Visual Studio 2019 or later.
   - Build the project to generate the DLL.

---

## Usage

### Service Registration

**Using Microsoft.Extensions.DependencyInjection:**
```csharp
var services = new ServiceCollection();
services.AddBeepContainerManager();
var provider = services.BuildServiceProvider();
var beepService = provider.GetService<IBeepService>();
```

**Using Autofac:**
```csharp
var builder = new ContainerBuilder();
BeepServicesRegisterAutFac.RegisterServices(builder); // Registers IBeepService
var container = builder.Build();
var beepService = container.Resolve<IBeepService>();
```

### Container Setup and Feature Loading

```csharp
beepService.Configure(@"C:\BeepData", "MyContainer", BeepConfigType.Container, true);
beepService.LoadAssemblies(); // Synchronous plugin/feature loading
await beepService.LoadAssembliesAsync(new Progress<PassedArgs>(args => Console.WriteLine(args.ParameterString1))); // Async with progress
beepService.LoadEnvironments();
```

### Environment and Container Management

```csharp
// Create required directories
ContainerMisc.CreateMainFolder();
ContainerMisc.CreateContainerfolder("MyContainer");

// Load configurations into the service context
ContainerMisc.AddAllConnectionConfigurations(beepService);
ContainerMisc.AddAllDataSourceMappings(beepService);
ContainerMisc.AddAllDataSourceQueryConfigurations(beepService);

// Save or load your environment
beepService.SaveEnvironments();
beepService.LoadEnvironments();
```

---

## BeepService In-Depth

**BeepService** is the main orchestrator, managing BeepDM core objects and all containers’ lifecycles.

### Responsibilities

- **DI Integration:** Registers all core BeepDM services (`IDMEEditor`, `IConfigEditor`, `IDMLogger`, `IAssemblyHandler`, etc.) as singleton or scoped, both for Microsoft DI and Autofac.
- **Configuration:** Initializes BeepDM with paths, names, and config types.
- **Component Access:** Exposes all main BeepDM components for application-wide use.
- **Feature/Plugin Management:** Dynamically loads assemblies and plugins at runtime.
- **Environment Management:** Loads/saves environment definitions as JSON.
- **Error Handling:** Structured error objects/logging and fallback state on failure.

### Key Methods

- `Configure(...)`: Sets up all components and registers services.
- `LoadServicesSingleton()` / `LoadServicesScoped()`: Register core components for DI.
- `LoadAssemblies()` / `LoadAssembliesAsync()`: Dynamic plugin/feature loading.
- `LoadEnvironments()` / `SaveEnvironments()`: Environment persistence.
- Error objects: `DMEEditor.ErrorObject`, `lg.WriteLog(...)`, etc.

### Example

```csharp
beepService.Configure(@"C:\Data", "Finance", BeepConfigType.Container, true);
beepService.LoadAssemblies();
if (beepService.DMEEditor.ErrorObject.Flag == Errors.Failed)
    Console.WriteLine("Initialization failed: " + beepService.DMEEditor.ErrorObject.Message);
```

---

## ContainerMisc In-Depth

**ContainerMisc** is a static utility class for all file system, mapping, and configuration tasks required by BeepService and containers.

### Key Functions

- `CreateMainFolder()`: Ensures main data directory exists, returns path.
- `CreateContainerfolder(string name)`: Ensures per-container folder exists.
- `AddAllConnectionConfigurations(IBeepService)`: Loads connection configs.
- `AddAllDataSourceMappings(IBeepService)`: Loads datasource mappings.
- `AddAllDataSourceQueryConfigurations(IBeepService)`: Loads query configs.

### Example

```csharp
string mainPath = ContainerMisc.CreateMainFolder();
string containerPath = ContainerMisc.CreateContainerfolder("Analysis");
ContainerMisc.AddAllConnectionConfigurations(beepService);
```

---

## Advanced Topics

### Plugin (Assembly) Loading

```csharp
// Synchronous loading
beepService.LoadAssemblies();

// Asynchronous loading with progress reporting
await beepService.LoadAssembliesAsync(new Progress<PassedArgs>(args => Console.WriteLine(args.ParameterString1)));

// Direct, custom plugin load
beepService.LLoader.LoadSingleAssembly("MyPlugin.dll");
```

### Error Handling

```csharp
try
{
    beepService.Configure(@"C:\InvalidPath", "BadContainer", BeepConfigType.Container);
}
catch (Exception ex)
{
    var error = beepService.DMEEditor.ErrorObject;
    Console.WriteLine("Configuration error: " + error.Message);
    if (error.Ex != null) Console.WriteLine(error.Ex.ToString());
}

if (beepService.DMEEditor.ErrorObject.Flag == Errors.Failed)
{
    // Handle error
}
beepService.lg.WriteLog("A custom log message");
```

### Integration with Other DI Frameworks

- **Microsoft.Extensions.DependencyInjection:** See above (default).
- **Autofac:** See above (advanced scenarios).
- **Other frameworks:** Register BeepDM components (`IDMEEditor`, `IConfigEditor`, etc.) using your framework’s registration syntax.

---

## Related Projects

- [BeepDM (Core Data Management Library)](https://github.com/The-Tech-Idea/BeepDM)
- [Beep.Containers (this library)](https://github.com/The-Tech-Idea/Beep.Containers)

---

## License

MIT

---

*For more information, see the [BeepDM documentation](https://github.com/The-Tech-Idea/BeepDM) and the source code in this repository.*
