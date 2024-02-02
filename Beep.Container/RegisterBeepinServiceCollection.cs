using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Util;
using TheTechIdea.Beep.Helpers;

namespace TheTechIdea.Beep.Container
{
    public static class RegisterBeepinServiceCollection
    {
        public static IServiceCollection Services { get; private set; }
      
     
        public static IServiceCollection RegisterBeep(this IServiceCollection services, string directorypath, string containername, BeepConfigType configType)
        {
            Services = services;
            IBeepService beepService = new   BeepService(services,directorypath, containername, configType);
            Services.AddSingleton<IBeepService>(beepService);
            return Services;
        }
        public static void AddAllDataSourceQueryConfigurations(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.QueryList.AddRange(RDBMSHelper.CreateQuerySqlRepos());
        }
        public static void AddAllConnectionConfigurations(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.AddRange(ConnectionHelper.GetAllConnectionConfigs());
        }
        public static void AddAllDataSourceMappings(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataTypesMap.AddRange(DataTypeFieldMappingHelper.GetMappings());
        }
        public static void CreateSnowFlakeConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateSnowFlakeConfig());
        }
        public static void CreateHadoopConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateHadoopConfig());
        }
        public static  void CreateRedisConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateRedisConfig());
        }
        public static  void CreateKafkaConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateKafkaConfig());
        }
        public static  void CreateOPCConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateOPCConfig());
        }
        public static  void CreateDB2Config(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateDB2Config());
        }
        public static  void CreateCouchDBConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCouchDBConfig());
        }
        public static  void CreateVistaDBConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateVistaDBConfig());
        }
        public static  void CreateCouchbaseConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCouchbaseConfig());
        }
        public static  void CreateFirebaseConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateFirebaseConfig());
        }
        public static  void CreateRealmConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateRealmConfig());
        }
        public static  void CreatePostgreConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreatePostgreConfig());
        }
        public static  void CreateMongoDBConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateMongoDBConfig());
        }
        public static  void CreateStackExchangeRedisConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateStackExchangeRedisConfig());
        }
        public static  void CreateCouchbaseLiteConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCouchbaseLiteConfig());
        }
        public static  void CreateElasticsearchConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateElasticsearchConfig());
        }
        public static  void CreateSQLiteConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateSQLiteConfig());
        }
        public static  void CreateRavenDBConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateRavenDBConfig());
        }
        public static  void CreateCSVFileReaderConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCSVFileReaderConfig());
        }
        public static  void CreateFirebirdConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateFirebirdConfig());
        }
        public static  void CreateCassandraConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCassandraConfig());
        }
        public static  void CreateMySqlConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateMySqlConfig());
        }
        public static  void CreateMySqlConnectorConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateMySqlConnectorConfig());
        }
        public static  void CreateSqlServerConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateSqlServerConfig());
        }
        public static  void CreateSqlCompactConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateSqlCompactConfig());
        }
        public static  void CreateDataViewConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateDataViewConfig());
        }
        public static  void CreateCSVDataSourceConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateCSVDataSourceConfig());
        }
        public static  void CreateJsonDataSourceConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateJsonDataSourceConfig());
        }
        public static  void CreateTxtXlsCSVFileSourceConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateTxtXlsCSVFileSourceConfig());
        }
        public static  void CreateLiteDBDataSourceConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateLiteDBDataSourceConfig());
        }
        public static  void CreateOracleConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateOracleConfig());
        }
        public static  void CreateDuckDBConfig(this IBeepService beepService)
        {
            beepService.DMEEditor.ConfigEditor.DataDriversClasses.Add(ConnectionHelper.CreateDuckDBConfig());
        }
       
    }
}
