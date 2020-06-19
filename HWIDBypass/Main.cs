using Harmony;
using MelonLoader;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnhollowerBaseLib;

namespace HWIDBypass
{
    public static class BuildInfo
    {
        public const string Name = "HWID Bypass"; // Name of the Mod.
        public const string Author = "404#0004"; // Author of the Mod.
        public const string Company = "I am not a company -Kappa-"; // Company that made the Mod.
        public const string Version = "1.6.0"; // Version of the Mod.
        public const string DownloadLink = "https://github.com/l-404-l/HWIDBypass/releases"; // Download Link for the Mod.
    }

    public class Main : MelonMod
    {
        public unsafe override void OnApplicationStart()
        {
            Config.LoadConfig();

            var MainHWID = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            //Credit to Dubya for finding out the old way is scuffed and Knah thinking to patch icalls rather than just methods :bigbrain:
            var mainmethod = IL2CPP.il2cpp_resolve_icall("UnityEngine.SystemInfo::GetDeviceUniqueIdentifier");
            Imports.Hook((IntPtr)(&mainmethod), AccessTools.Method(typeof(Main), "FakeDeviceID").MethodHandle.GetFunctionPointer());

            if (Config.CFG.ConsolePrint) // Idk lets you see if its working
            {
                MelonModLogger.Log("Old HWID:");
                MelonModLogger.Log(MainHWID);
                MelonModLogger.Log("New HWID:");
                MelonModLogger.Log(UnityEngine.SystemInfo.deviceUniqueIdentifier);
            }
        }

        public static IntPtr FakeDeviceID()
        {
            if (string.IsNullOrEmpty(Config.CFG.HWID))
            {
                var random = new Random();
                Config.CFG.HWID = KeyedHashAlgorithm.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}B-{1}1-C{2}-{3}A-{4}{5}-{6}{7}", new object[]
                {
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9), // this takes literally 3ms but looks like it would take forever XD
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9)
                }))).Select((byte x) =>
                {
                    return x.ToString("x2");
                }).Aggregate((string x, string y) => x + y);
                Config.SaveConfig();
            }
            if (IntPtr.Zero == Config.HWIDP)
                Config.HWIDP = new Il2CppSystem.Object(IL2CPP.ManagedStringToIl2Cpp(Config.CFG.HWID)).Pointer;
            return Config.HWIDP;
        }
    }
}
