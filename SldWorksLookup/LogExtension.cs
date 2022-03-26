using System;
using System.Collections.Generic;
using System.Linq;
using Exceptionless;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xarial.XCad.SolidWorks.Enums;
using Exceptionless.Logging;

namespace SldWorksLookup
{
    internal static class LogExtension
    {
        public static readonly string LogFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"SldWorksLookup",
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

                if (!File.Exists(configFile))
                    return;

                var data = File.ReadAllLines(configFile);

                if (data.Length < 2)
                    return;

                Client = new ExceptionlessClient(c =>
                {
                    c.ServerUrl = data[0].Trim();
                    c.ApiKey = data[1].Trim();
                    c.SetVersion(version);
                });

                //服务信息收集配置
                Client.Configuration.IncludePrivateInformation = true;
                Client.Configuration.IncludeMachineName = true;
                Client.Configuration.IncludeIpAddress = true;
                
                //设置本地存储日志文件夹
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

                //开启心跳追踪
                var uid = $"{Environment.UserName}@{Environment.MachineName}";
                Client.Configuration.SetUserIdentity(uid, userName ?? uid);
                Client.Configuration.UseSessions();

                Client.SubmitLog(
                    $"启动:{sldWorksVersion}...", LogLevel.Info);

                Client.Startup();
            }
            catch (Exception ex)
            {

            }
        }

        internal static void LogEnded()
        {
            Client?.SubmitLog("退出...", LogLevel.Info);
            Client?.ProcessQueue();
        }
    }
}
