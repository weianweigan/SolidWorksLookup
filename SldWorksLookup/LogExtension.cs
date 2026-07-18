using System;
using System.Diagnostics;
using System.IO;
using Exceptionless;
using Exceptionless.Logging;
using Xarial.XCad.SolidWorks.Enums;

namespace SldWorksLookup
{
    internal static class LogExtension
    {
        public static readonly string LogFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SldWorksLookup",
            "Log");

        public static ExceptionlessClient Client { get; private set; }

        internal static void LogStart(
            Version version,
            SwVersion_e sldWorksVersion,
            string userName)
        {
            try
            {
                var configFile = Path.Combine(
                    Path.GetDirectoryName(typeof(LogExtension).Assembly.Location),
                    "exceptionless.txt");

                string serverUrl;
                string apiKey;
                if (!TryReadConfiguration(configFile, out serverUrl, out apiKey))
                    return;

                Client = new ExceptionlessClient(c =>
                {
                    c.ServerUrl = serverUrl;
                    c.ApiKey = apiKey;
                    c.SetVersion(version);
                });

                Client.Configuration.IncludePrivateInformation = true;
                Client.Configuration.IncludeMachineName = true;
                Client.Configuration.IncludeIpAddress = true;

                try
                {
                    if (!Directory.Exists(LogFolder))
                        Directory.CreateDirectory(LogFolder);
                    Client.Configuration.UseFolderStorage(LogFolder);
                }
                catch (Exception ex)
                {
                    ex.ToExceptionless(Client)
                        .AddTags("CreateDirectoy Failed")
                        .Submit();
                }

                var uid = $"{Environment.UserName}@{Environment.MachineName}";
                Client.Configuration.SetUserIdentity(uid, userName ?? uid);
                Client.Configuration.UseSessions();

                Client.SubmitLog(
                    $"启动:{sldWorksVersion}...", LogLevel.Info);

                Client.Startup();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        internal static bool TryReadConfiguration(string path, out string serverUrl, out string apiKey)
        {
            serverUrl = null;
            apiKey = null;

            try
            {
                if (!File.Exists(path))
                    return false;

                var data = File.ReadAllLines(path);
                if (data.Length < 2)
                    return false;

                serverUrl = data[0].Trim();
                apiKey = data[1].Trim();

                if (string.IsNullOrWhiteSpace(serverUrl) || string.IsNullOrWhiteSpace(apiKey))
                {
                    serverUrl = null;
                    apiKey = null;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                serverUrl = null;
                apiKey = null;
                return false;
            }
        }

        internal static void LogEnded()
        {
            Client?.SubmitLog("退出...", LogLevel.Info);
            Client?.ProcessQueue();
        }
    }
}
