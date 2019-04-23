using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace mSettings
{
    class Program
    {
        static void Main()
        {
            Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Console.WriteLine(appConfig.HasFile);
            Console.WriteLine(appConfig.FilePath);
            appConfig.Save(ConfigurationSaveMode.Full);

            if (!appConfig.HasFile)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                stringBuilder.AppendLine("<configuration>");
                stringBuilder.AppendLine("</configuration>");

                File.WriteAllText(string.Concat(Assembly.GetEntryAssembly().Location, ".config"), stringBuilder.ToString());
            }

            if (appConfig.AppSettings.Settings.AllKeys.Contains("test"))
                appConfig.AppSettings.Settings["test"].Value = "test1";
            else
                appConfig.AppSettings.Settings.Add("test", "test");

            appConfig.Save(ConfigurationSaveMode.Full);
            Console.WriteLine(appConfig.AppSettings.Settings["test"].Value);
            
            appConfig.AppSettings.Settings["test"].Value = "test2";
            appConfig.Save(ConfigurationSaveMode.Full);
            Console.WriteLine(appConfig.AppSettings.Settings["test"].Value);
            
            
            Console.Read();
        }
    }
}
