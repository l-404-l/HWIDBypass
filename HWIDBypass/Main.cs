using Harmony;
using MelonLoader;
using NET_SDK;
using NET_SDK.Harmony;
using NET_SDK.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace HWIDBypass
{
    public static class BuildInfo
    {
        public const string Name = "HWID Bypass"; // Name of the Mod.
        public const string Author = "404#0004"; // Author of the Mod.
        public const string Company = "I am not a company -Kappa-"; // Company that made the Mod.
        public const string Version = "1.5.0"; // Version of the Mod.
        public const string DownloadLink = "https://github.com/l-404-l/HWIDBypass/releases"; // Download Link for the Mod.
    }

    public class Main : MelonMod
    {
        public static Instance H = Manager.CreateInstance("404BanBypass");
        public static ProcessFix ProcessCall;
        public static MethodInfo Hook = typeof(Imports).GetMethod("Hook", BindingFlags.NonPublic | BindingFlags.Static);
        public delegate void ProcessFix(IntPtr instance, IntPtr parm2);

        public static void HookMethod(IntPtr TM, IntPtr NM)=>
            Hook.Invoke(null, new object[] { TM, NM });
        public unsafe override void OnApplicationStart()
        {
            Config.LoadConfig();

            IL2CPP_Class API2 = NET_SDK.SDK.GetClass("VRC.Core", "API");
            IL2CPP_Class Amp = NET_SDK.SDK.GetClass("AmplitudeSDKWrapper", "AmplitudeWrapper");
            IL2CPP_Class WS = NET_SDK.SDK.GetClass("Transmtn", "WebsocketPipeline"); //Thanks Zoey#9420 for pointing out Websockets
            var oldhwid = VRC.Core.API.DeviceID;

            H.Patch(API2.GetProperty("DeviceID").GetGetMethod(), AccessTools.Method(typeof(Main), "DeviceID"));
            H.Patch(Amp.GetMethod("InitializeDeviceId"), AccessTools.Method(typeof(Main), "DeviceID1"));
            H.Patch(Amp.GetMethods(x => x.Name == "LogEvent" && x.GetParameterCount() == 4 && NET_SDK.IL2CPP.il2cpp_type_get_name(x.GetParameters()[2].Ptr).Equals("System.Int64")).First(), AccessTools.Method(typeof(Main), "LogEvent"));
            var original = *(IntPtr*)WS.GetMethod("ProcessPipe").Ptr;
            HookMethod((IntPtr)(&original), Marshal.GetFunctionPointerForDelegate(new Action<IntPtr,IntPtr>(ProcessPipeFix)));
            ProcessCall = Marshal.GetDelegateForFunctionPointer<ProcessFix>(original);

            if (Config.CFG.ConsolePrint)
            {
                MelonModLogger.Log("Old HWID:");
                MelonModLogger.Log(oldhwid);
                MelonModLogger.Log("New HWID:");
                MelonModLogger.Log(VRC.Core.API.DeviceID);
            }
        }

        public static void ProcessPipeFix(IntPtr instance, IntPtr parm2) //Thanks knah#6508 for helping me with ProcessPipe being aids.
        {
            if (instance != IntPtr.Zero)
            {
                var ok = new Transmtn.WebsocketPipeline(instance);
                ok._macAddress = GetDevice();
                ProcessCall(ok.Pointer, parm2);
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
