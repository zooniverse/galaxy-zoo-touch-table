using System;
using System.Configuration;

namespace GalaxyZooTouchTable
{
    static class Config
    {
        public static string WorkflowId;
        public static string ProjectId;
        public static string CaesarHost;

        static Config()
        {
            var appSettings = ConfigurationManager.AppSettings;
            WorkflowId = appSettings["WorkflowId"];
            ProjectId = appSettings["ProjectId"];
            CaesarHost = appSettings["CaesarHost"];
        }
    }
}
