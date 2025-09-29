using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Helpers;
using TheTechIdea.Beep.Helpers.DataTypesHelpers;
using TheTechIdea.Beep.Helpers.RDBMSHelpers;

namespace TheTechIdea.Beep.Container.Shared
{
    public static class ContainerMisc
    {
        private static IBeepService BeepService;
        private static IServiceCollection Services;

        public static string BeepDataPath { get; private set; }
        public static string ContainerDataPath { get; private set; }
        private static bool mappingcreated = false;
        private static bool connectioncreated = false;
        private static bool datasourcecreated = false;

        #region "Container Methods"

        #endregion

        public static IServiceCollection CreateBeepMapping(this IBeepService beepService)
        {
            if (beepService != null)
            {
                BeepService = beepService;
                AddAllConnectionConfigurations(beepService);
                AddAllDataSourceMappings(beepService);
                AddAllDataSourceQueryConfigurations(beepService);
            }
            return Services;
        }

        /// <summary>
        /// Creates the main Beep application folder in a platform-appropriate location.
        /// </summary>
        /// <param name="beepService">Optional BeepService reference.</param>
        /// <returns>Path to the created main folder or empty string if creation fails.</returns>
        public static string CreateMainFolder(this IBeepService beepService)
        {
            return CreateMainFolder();
        }

        /// <summary>
        /// Creates the main Beep application folder in a platform-appropriate location.
        /// </summary>
        /// <returns>Path to the created main folder or empty string if creation fails.</returns>
        public static string CreateMainFolder()
        {
            try
            {
                string basePath = GetBaseFolderPath();
                string beepPath = Path.Combine(basePath, "TheTechIdea", "Beep");

                if (!Directory.Exists(beepPath))
                {
                    Directory.CreateDirectory(beepPath);
                }

                BeepDataPath = beepPath;
                return BeepDataPath;
            }
            catch (Exception ex)
            {
                // Log error or throw as appropriate for your application
                Console.WriteLine($"Failed to create main folder: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates a container folder within the main Beep folder.
        /// </summary>
        /// <param name="containername">Name of the container folder to create.</param>
        /// <returns>Path to the created container folder or empty string if creation fails.</returns>
        public static string CreateContainerfolder(string containername = "")
        {
            try
            {
                // Ensure main folder exists
                if (string.IsNullOrEmpty(BeepDataPath))
                {
                    CreateMainFolder();
                }

                if (string.IsNullOrEmpty(BeepDataPath))
                {
                    return string.Empty;
                }

                if (!string.IsNullOrEmpty(containername))
                {
                    // Sanitize folder name for cross-platform compatibility
                    string safeName = SanitizeFolderName(containername);
                    string containerPath = Path.Combine(BeepDataPath, safeName);

                    if (!Directory.Exists(containerPath))
                    {
                        Directory.CreateDirectory(containerPath);
                    }

                    ContainerDataPath = containerPath;
                    return ContainerDataPath;
                }

                return ContainerDataPath ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Log error or throw as appropriate for your application
                Console.WriteLine($"Failed to create container folder: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates an application folder within a container folder.
        /// </summary>
        /// <param name="containername">Name of the container that will hold the app folder.</param>
        /// <param name="appfolder">Name of the application folder to create.</param>
        /// <returns>Path to the created application folder or empty string if creation fails.</returns>
        public static string CreateAppfolder(string containername, string appfolder)
        {
            try
            {
                if (string.IsNullOrEmpty(containername) || string.IsNullOrEmpty(appfolder))
                {
                    return string.Empty;
                }

                // Create container first
                string containerPath = CreateContainerfolder(containername);
                if (string.IsNullOrEmpty(containerPath))
                {
                    return string.Empty;
                }

                // Sanitize folder name
                string safeName = SanitizeFolderName(appfolder);
                string appFolderPath = Path.Combine(containerPath, safeName);

                if (!Directory.Exists(appFolderPath))
                {
                    Directory.CreateDirectory(appFolderPath);
                }

                return appFolderPath;
            }
            catch (Exception ex)
            {
                // Log error or throw as appropriate for your application
                Console.WriteLine($"Failed to create app folder: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates an application folder directly within the main Beep folder.
        /// </summary>
        /// <param name="appfolder">Name of the application folder to create.</param>
        /// <returns>Path to the created application folder or empty string if creation fails.</returns>
        public static string CreateAppfolder(string appfolder)
        {
            try
            {
                if (string.IsNullOrEmpty(appfolder))
                {
                    return string.Empty;
                }

                // Ensure main folder exists
                if (string.IsNullOrEmpty(BeepDataPath))
                {
                    CreateMainFolder();
                }

                if (string.IsNullOrEmpty(BeepDataPath))
                {
                    return string.Empty;
                }

                // Sanitize folder name
                string safeName = SanitizeFolderName(appfolder);
                string appFolderPath = Path.Combine(BeepDataPath, safeName);

                if (!Directory.Exists(appFolderPath))
                {
                    Directory.CreateDirectory(appFolderPath);
                }

                return appFolderPath;
            }
            catch (Exception ex)
            {
                // Log error or throw as appropriate for your application
                Console.WriteLine($"Failed to create app folder: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the appropriate base folder path based on the current operating system.
        /// </summary>
        /// <returns>The base folder path for application data.</returns>
        private static string GetBaseFolderPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // On Windows, use CommonApplicationData (C:\ProgramData)
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // On macOS, use ~/.config or ~/Library/Application Support
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Library", "Application Support");
            }
            else
            {
                // On Linux, use ~/.config 
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config");
            }
        }

        /// <summary>
        /// Sanitizes folder names for cross-platform compatibility.
        /// </summary>
        /// <param name="folderName">The folder name to sanitize.</param>
        /// <returns>A sanitized, cross-platform safe folder name.</returns>
        private static string SanitizeFolderName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                return folderName;

            // Replace invalid characters with underscores
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                folderName = folderName.Replace(c, '_');
            }

            // Trim leading and trailing whitespace
            folderName = folderName.Trim();

            // Trim to reasonable length if needed
            if (folderName.Length > 100)
            {
                folderName = folderName.Substring(0, 100);
            }

            return folderName;
        }

        /// <summary>
        /// Creates a directory recursively, ensuring all parent directories exist.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        /// <returns>True if the directory was created successfully, false otherwise.</returns>
        private static bool CreateDirectorySafely(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return false;

                if (Directory.Exists(path))
                    return true;

                Directory.CreateDirectory(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void AddAllDataSourceQueryConfigurations(this IBeepService beepService)
        {
            if (datasourcecreated) return;
            beepService.DMEEditor.ConfigEditor.QueryList.AddRange(RDBMSHelper.CreateQuerySqlRepos());
            datasourcecreated = true;
        }

        public static void AddAllConnectionConfigurations(this IBeepService beepService)
        {
            if (connectioncreated) return;
            if (beepService.DMEEditor.ConfigEditor.DataDriversClasses == null)
            {
                beepService.DMEEditor.ConfigEditor.DataDriversClasses = new List<ConnectionDriversConfig>();
            }
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.AddRange(ConnectionHelper.GetAllConnectionConfigs());
            connectioncreated = true;
        }

        public static void AddAllDataSourceMappings(this IBeepService beepService)
        {
            if (mappingcreated) return;
            beepService.DMEEditor.ConfigEditor.DataTypesMap.AddRange(DataTypeFieldMappingHelper.GetMappings());
            mappingcreated = true;
        }
    }
}