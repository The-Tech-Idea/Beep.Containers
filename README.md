# Beep Containers

Beep Containers is a dependency injection (DI) and container management library within the [BeepDM Framework](https://github.com/The-Tech-Idea/BeepDM), developed by [The-Tech-Idea](https://github.com/The-Tech-Idea). It provides a robust infrastructure for registering, managing, and utilizing services and features in BeepDM applications, built on top of Microsoft.Extensions.DependencyInjection. This project is in its early stages (Alpha) and aims to enhance the modularity and scalability of BeepDM-based solutions.

## Features
- **Service Registration**: Register BeepDM services as singletons or scoped instances using extension methods.
- **Container Management**: Create, load, and manage multiple containers with file system integration.
- **Feature Management**: Define and load features dynamically with assembly scanning.
- **Configuration Flexibility**: Support for container-specific configurations and environment settings.
- **File System Support**: Automatically create container directories and manage configuration files.
- **Assembly Loading**: Dynamically load and resolve assemblies for extensibility.

## Installation
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/The-Tech-Idea/Beep.Containers.git
   ```
2. **Open in Visual Studio**:
   - Open the solution file (`Beep.Containers.sln`) in Visual Studio 2019 or later. (Note: The `.sln` file may not yet be in the repo; use your local copy if available.)
   - Build the solution to compile the projects.
3. **Add as a NuGet Package (Optional)**:
   - If published, install via NuGet:
     ```bash
     dotnet add package TheTechIdea.Beep.Containers
     ```
4. **Integrate with BeepDM**:
   - Ensure the [BeepDM Framework](https://github.com/The-Tech-Idea/BeepDM) is referenced in your project.
   - Add `Beep.Containers` as a dependency in your BeepDM application.

## Usage
- **Register Services**:
  ```csharp
  var services = new ServiceCollection();
  services.AddBeepContainerManager(); // Registers the container manager
  var serviceProvider = services.BuildServiceProvider();
  var containerManager = serviceProvider.GetService<ICantainerManager>();
  ```
- **Create a Container**:
  ```csharp
  await containerManager.CreateContainer("my-container-guid", "user1", "user1@example.com", 1, "user-guid", "MyContainer", @"C:\CustomPath");
  ```
- **Add a Beep Container**:
  ```csharp
  services.AddBeepContainer(@"C:\BeepData", "MyContainer", BeepConfigType.Container);
  var beepContainer = serviceProvider.GetService<IBeepContainer>();
  ```
- **Load Features**:
  ```csharp
  var featureLoader = new FeatureLoader(assemblyHandler);
  featureLoader.LoadAllAssembly(); // Scans assemblies for IBeepFeature implementations
  ```

## Requirements
- [.NET Core](https://dotnet.microsoft.com/download) or [.NET Framework](https://dotnet.microsoft.com/download) (version TBD based on BeepDM requirements).
- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) NuGet package.
- [BeepDM Framework](https://github.com/The-Tech-Idea/BeepDM) (specific version TBD).

## Project Structure
The `Beep.Containers` solution contains the following namespaces and key components:

### TheTechIdea.Beep.Container.ContainerManagement
- **Type**: Class Library
- **Purpose**: Core container management and service registration logic.
- **Key Files**:
  - `RegisterContainerManagerServiceCollection.cs`: Extension methods for registering container managers and Beep containers in IServiceCollection.
  - `CantainerManager.cs`: Implements `ICantainerManager` for creating, loading, and managing Beep containers, with file system and JSON persistence.
- **Output**: Included in the main DLL (`Beep.Containers.dll`).

### TheTechIdea.Beep.Container.FeatureManagement
- **Type**: Class Library
- **Purpose**: Feature definition and dynamic loading from assemblies.
- **Key Files**:
  - `BeepFeature.cs`: Implements `IBeepFeature` for defining features with metadata and service registration.
  - `FeatureLoader.cs`: Implements `ILoaderExtention` for scanning assemblies and loading `IBeepFeature` implementations.
- **Output**: Included in the main DLL (`Beep.Containers.dll`).

### TheTechIdea.Beep.Container.Services
- **Type**: Class Library
- **Purpose**: Core Beep service implementation and utilities.
- **Key Files**:
  - `BeepService.cs`: Implements `IBeepService` for configuring and managing BeepDM components (editor, logger, etc.).
  - `ServiceHelper.cs`: Static helper for accessing services from the DI container.
- **Output**: Included in the main DLL (`Beep.Containers.dll`).

### TheTechIdea.Beep.Container
- **Type**: Class Library
- **Purpose**: Miscellaneous utilities and Beep-specific DI registration.
- **Key Files**:
  - `ContainerMisc.cs`: Static methods for creating folder structures and adding configurations/mappings.
  - `RegisterBeepinServiceCollection.cs`: Extension methods for registering Beep services in IServiceCollection.
- **Output**: Included in the main DLL (`Beep.Containers.dll`).

*(Note: The structure assumes a single DLL output; adjust if separate DLLs are intended.)*

## Contributing
We welcome contributions! Please see our [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on how to get involved.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Status
This project is in **Alpha**. Expect rapid changes as we build out functionality. Check the [Issues](https://github.com/The-Tech-Idea/Beep.Containers/issues) tab for current tasks and bugs.

## Contact
For questions or suggestions, open an issue or reach out to the maintainers at [The-Tech-Idea](https://github.com/The-Tech-Idea).
