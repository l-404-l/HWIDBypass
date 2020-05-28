using Harmony;
using MelonLoader;
using NET_SDK.Harmony;
using NET_SDK.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HWIDBypass
{
    public static class BuildInfo
    {
        public const string Name = "HWID Bypass"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "404#0004"; // Author of the Mod.  (Set as null if none)
        public const string Company = "I am not a company -Kappa-"; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.4.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        public static Instance H = Manager.CreateInstance("404BanBypass");
        public override void OnApplicationStart()
        {
            Config.LoadConfig();

            IL2CPP_Class API = NET_SDK.SDK.GetClass("VRC.Core", "API");
            IL2CPP_Class Amp = NET_SDK.SDK.GetClass("AmplitudeSDKWrapper", "AmplitudeWrapper");
            var oldhwid = VRC.Core.API.DeviceID;

            H.Patch(API.GetProperty("DeviceID").GetGetMethod(), AccessTools.Method(typeof(Main), "DeviceID"));
            H.Patch(Amp.GetMethod("InitializeDeviceId"), AccessTools.Method(typeof(Main), "DeviceID1"));
            H.Patch(Amp.GetMethods(x => x.Name == "LogEvent" && x.GetParameterCount() == 4 && NET_SDK.IL2CPP.il2cpp_type_get_name(x.GetParameters()[2].Ptr).Equals("System.Int64")).First(), AccessTools.Method(typeof(Main), "LogEvent"));

            if (Config.CFG.ConsolePrint)
            {
                MelonModLogger.Log("Old HWID:");
                MelonModLogger.Log(oldhwid);
                MelonModLogger.Log("New HWID:");
                MelonModLogger.Log(VRC.Core.API.DeviceID);
            }
        }

        public string DeviceID() =>
            GetDevice();
        public static string DeviceID1(IntPtr instance) =>
            GetDevice();

        public static void LogEvent(IntPtr instance, string var, IDictionary<string, object> var2, long var3, IntPtr var4) { }

        private static string GetDevice()
        {
            if (string.IsNullOrEmpty(Config.CFG.HWID))
            {
                Config.CFG.HWID = KeyedHashAlgorithm.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}B-{1}1-C{2}-{3}A-{4}{5}-{6}{7}", new object[]
                {
                        new Random().Next(1, 9),
                        new Random().Next(1, 9),
                        new Random().Next(1, 9),
                        new Random().Next(1, 9),
                        new Random().Next(1, 9),
                        new Random().Next(1, 9),
                        new Random().Next(1, 9),
                        new Random().Next(1, 9)
                }))).Select((byte x) =>
                {
                    byte b = x;
                    return b.ToString("x2");
                }).Aggregate((string x, string y) => x + y);
                Config.SaveConfig();

            }
            return Config.CFG.HWID;
        }

    }
}
