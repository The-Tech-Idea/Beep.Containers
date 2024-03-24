using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Helpers;

namespace TheTechIdea.Beep.Container
{
    public static class ContainerMisc
    {
        private static IBeepService BeepService;
        private static IServiceCollection Services;

        public static string BeepDataPath { get; private set; }

        public static IServiceCollection CreateBeepMapping (IBeepService beepService)
        {
            if (beepService != null)
            {
                if (beepService != null)
                {
                    BeepService = beepService;
                }
                AddAllConnectionConfigurations(beepService);
                AddAllDataSourceMappings(beepService);
                AddAllDataSourceQueryConfigurations(beepService);
            }
            return Services;
        }
        public static IServiceCollection CreateMainFolder (IBeepService beepService,string containername="")
        {
            if (beepService != null)
            {
                BeepService=beepService;
            }
            Createfolder(containername);
            return Services;
        }
        private static void Createfolder(string containername="")
        {
            if (BeepService != null)
            {
                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep")))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep"));

                }
                BeepDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep");
            }
            if(!string.IsNullOrEmpty(containername))
            {
                if (!Directory.Exists(Path.Combine(BeepDataPath, containername)))
                {
                    Directory.CreateDirectory(Path.Combine(BeepDataPath, containername));

                }
            }
        }
        public static void AddAllDataSourceQueryConfigurations (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.QueryList.AddRange(RDBMSHelper.CreateQuerySqlRepos());
        }
        public static void AddAllConnectionConfigurations (IBeepService beepService)
        {
            if (beepService.DMEEditor.ConfigEditor.DataDriversClasses == null)
            {
                beepService.DMEEditor.ConfigEditor.DataDriversClasses = new List<DataManagementModels.DriversConfigurations.ConnectionDriversConfig>();
            }
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.AddRange(ConnectionHelper.GetAllConnectionConfigs());
        }
        public static void AddAllDataSourceMappings (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataTypesMap.AddRange(DataTypeFieldMappingHelper.GetMappings());
        }
        public static void CreateSnowFlakeConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateSnowFlakeConfig());
        }
        public static void CreateHadoopConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateHadoopConfig());
        }
        public static void CreateRedisConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateRedisConfig());
        }
        public static void CreateKafkaConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateKafkaConfig());
        }
        public static void CreateOPCConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateOPCConfig());
        }
        public static void CreateDB2Config (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateDB2Config());
        }
        public static void CreateCouchDBConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCouchDBConfig());
        }
        public static void CreateVistaDBConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateVistaDBConfig());
        }
        public static void CreateCouchbaseConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCouchbaseConfig());
        }
        public static void CreateFirebaseConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateFirebaseConfig());
        }
        public static void CreateRealmConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateRealmConfig());
        }
        public static void CreatePostgreConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreatePostgreConfig());
        }
        public static void CreateMongoDBConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateMongoDBConfig());
        }
        public static void CreateStackExchangeRedisConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateStackExchangeRedisConfig());
        }
        public static void CreateCouchbaseLiteConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCouchbaseLiteConfig());
        }
        public static void CreateElasticsearchConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateElasticsearchConfig());
        }
        public static void CreateSQLiteConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateSQLiteConfig());
        }
        public static void CreateRavenDBConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateRavenDBConfig());
        }
        public static void CreateCSVFileReaderConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCSVFileReaderConfig());
        }
        public static void CreateFirebirdConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateFirebirdConfig());
        }
        public static void CreateCassandraConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCassandraConfig());
        }
        public static void CreateMySqlConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateMySqlConfig());
        }
        public static void CreateMySqlConnectorConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateMySqlConnectorConfig());
        }
        public static void CreateSqlServerConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateSqlServerConfig());
        }
        public static void CreateSqlCompactConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateSqlCompactConfig());
        }
        public static void CreateDataViewConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateDataViewConfig());
        }
        public static void CreateCSVDataSourceConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCSVDataSourceConfig());
        }
        public static void CreateJsonDataSourceConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateJsonDataSourceConfig());
        }
        public static void CreateTxtXlsCSVFileSourceConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateTxtXlsCSVFileSourceConfig());
        }
        public static void CreateLiteDBDataSourceConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateLiteDBDataSourceConfig());
        }
        public static void CreateOracleConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateOracleConfig());
        }
        public static void CreateDuckDBConfig (IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateDuckDBConfig());
        }
    }
}
