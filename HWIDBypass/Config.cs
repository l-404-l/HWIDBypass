using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWIDBypass
{
    public class Config
    {
        public static string MainFolder = "404Mods/HWIDBypass";
        public static string ModLocation = "hwid.json";
        public static Config CFG;
        public static IntPtr HWIDP = IntPtr.Zero;

        public string HWID;
        public bool ConsolePrint = true;
        public static void SaveConfig()
        {
            if (CFG != null)
                File.WriteAllText(MainFolder + "//" + ModLocation, JsonConvert.SerializeObject(CFG,Formatting.Indented));
        }
        public static Config LoadConfig() 
        {
            Directory.CreateDirectory(MainFolder);

            if (!File.Exists(MainFolder + "//" + ModLocation))
            {
                File.Create(MainFolder + "//" + ModLocation).Close();
                CFG = new Config();
                SaveConfig();
            }
            else
            {
                CFG = JsonConvert.DeserializeObject<Config>(File.ReadAllText(MainFolder + "//" + ModLocation));
                if (CFG == null)
                    CFG = new Config();
                SaveConfig();
            }
            return CFG;
        }
    }
}
