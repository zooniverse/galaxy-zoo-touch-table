using System;
using System.Configuration;

namespace GalaxyZooTouchTable
{
    static class Config
    {
        public static string WorkflowId;
        public static string ProjectId;

        static Config()
        {
            var appSettings = ConfigurationManager.AppSettings;
            WorkflowId = appSettings["WorkflowId"];
            ProjectId = appSettings["ProjectId"];
        }
    }
}
