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

        internal static void LogStart(
            Version version, 
            SwVersion_e sldWorksVersion, 
            string userName)
        {
            var configFile = Path.Combine(
            Path.GetDirectoryName(typeof(LogExtension).Assembly.Location),
            "exceptionless.txt");

            if (!File.Exists(configFile))
                return;

            var data = File.ReadAllLines(configFile);

            if (data.Length < 2)
                return;

            ExceptionlessClient.Default.Configuration.ServerUrl = data[0].Trim();
            ExceptionlessClient.Default.Configuration.ApiKey = data[1].Trim();

            //服务信息收集配置
            ExceptionlessClient.Default.Configuration.IncludePrivateInformation = true;
            ExceptionlessClient.Default.Configuration.IncludeMachineName = true;
            ExceptionlessClient.Default.Configuration.IncludeIpAddress = true;

            //设置当前客户端版本号
            ExceptionlessClient.Default.Configuration.SetVersion(version);

            //设置本地存储日志文件夹
            try
            {
                if (!Directory.Exists(LogFolder))
                    Directory.CreateDirectory(LogFolder);
                ExceptionlessClient.Default.Configuration.UseFolderStorage(LogFolder);

            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                    .AddTags("CreateDirectoy Failed")
                    .Submit();
            }

            //开启心跳追踪
            var uid = $"{Environment.UserName}@{Environment.MachineName}";
            ExceptionlessClient.Default.Configuration.SetUserIdentity(uid, userName ?? uid);
            ExceptionlessClient.Default.Configuration.UseSessions();

            ExceptionlessClient.Default.SubmitLog(
                $"启动:{sldWorksVersion}...", LogLevel.Info);

            ExceptionlessClient.Default.Startup();
        }

        internal static void LogEnded()
        {
            ExceptionlessClient.Default.SubmitLog("退出...", LogLevel.Info);
            ExceptionlessClient.Default.ProcessQueue();
        }
    }
}
