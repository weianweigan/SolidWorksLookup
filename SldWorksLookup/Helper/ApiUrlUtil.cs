using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.SolidWorks.Enums;

namespace SldWorksLookup.Helper
{
    public static class ApiUrlUtil
    {
        public static SwVersion_e SwVersion { get; private set; }
        public static string SwVersionStr { get; private set; }

        static string baseUrl;

        public static void Init(SwVersion_e swVersion)
        {
            SwVersion = swVersion;
            var versionString = SwVersion.ToString();
            SwVersionStr = versionString.Substring(2, versionString.Length - 2);

            baseUrl = $"http://help.solidworks.com/{SwVersionStr}/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.";
        }

        public static string GetProperty(string interfaceStr,string name)
        {
            return baseUrl + $"{interfaceStr}~{name}.html";
        }

        public static string GetMethod(string interfaceStr,string name)
        {
            return baseUrl + $"{interfaceStr}~{name}.html";
        }

        public static string GetInterface(string interfaceStr)
        {
            return baseUrl + $"{interfaceStr}.html";
        }
    }
}
