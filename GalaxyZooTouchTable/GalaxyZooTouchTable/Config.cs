using System;
using System.Configuration;

namespace GalaxyZooTouchTable
{
    static class Config
    {
        public static string WorkflowId;
        public static string ProjectId;
        public static string CaesarHost;
        public static string DatabaseName;

        static Config()
        {
            var appSettings = ConfigurationManager.AppSettings;
            WorkflowId = appSettings["WorkflowId"];
            ProjectId = appSettings["ProjectId"];
            CaesarHost = appSettings["CaesarHost"];
            DatabaseName = appSettings["DatabaseName"];
        }
    }
}
