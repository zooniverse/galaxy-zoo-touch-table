using System;
using System.Configuration;

namespace GalaxyZooTouchTable
{
    static class Config
    {
        public static string WorkflowId;

        static Config()
        {
            var appSettings = ConfigurationManager.AppSettings;
            WorkflowId = appSettings["WorkflowId"];
        }
    }
}
