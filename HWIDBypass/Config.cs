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
        public static string MainFolder = "404Mods";
        public static string ModLocation = "hwid.json";
        public static Config CFG;

        public string HWID = null;
        public bool ConsolePrint = true;
        public static void SaveConfig()
        {
            if (CFG != null)
                File.WriteAllText(MainFolder + "//" + ModLocation, JsonConvert.SerializeObject(CFG));
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
