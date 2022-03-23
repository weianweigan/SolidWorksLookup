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
            Path.GetDirectoryName(typeof(LogExtension).Assembly.Location),
            "Log");

        internal static void Init(
            Version version, 
            SwVersion_e sldWorksVersion, 
            string userName)
        {
            ExceptionlessClient.Default.Configuration.ServerUrl = "";
            ExceptionlessClient.Default.Configuration.ApiKey = "";

            //服务信息收集配置
            ExceptionlessClient.Default.Configuration.IncludePrivateInformation = true;
            ExceptionlessClient.Default.Configuration.IncludeMachineName = true;
            ExceptionlessClient.Default.Configuration.IncludeIpAddress = true;

            //设置当前客户端版本号
            ExceptionlessClient.Default.Configuration.SetVersion(version);

            //设置本地存储日志文件夹
            if (!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);
            ExceptionlessClient.Default.Configuration.UseFolderStorage(LogFolder);

            //开启心跳追踪
            var uid = $"{Environment.UserName}@{Environment.MachineName}";
            ExceptionlessClient.Default.Configuration.SetUserIdentity(uid, userName ?? uid);
            ExceptionlessClient.Default.Configuration.UseSessions();

            ExceptionlessClient.Default.SubmitLog(
                $"启动:{sldWorksVersion}...", LogLevel.Info);

        }
    }
}
