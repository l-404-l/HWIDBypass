using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanBypass
{
    public class Config
    {
        public static string MainFolder = "404Mods";
        public static string ModLocation = "hwid.yml";
        public static Config CFG;

        public string HWID = null;
        public bool ConsolePrint = true;
        public static void SaveConfig()
        {
            var Yml = new Serializer();
            if (CFG != null)
                File.WriteAllText(MainFolder + "//" + ModLocation, Yml.Serialize(CFG));
        }
        public static Config LoadConfig()
        {
            var Yml = new Serializer();
            Directory.CreateDirectory(MainFolder);

            if (!File.Exists(MainFolder + "//" + ModLocation))
            {
                File.Create(MainFolder + "//" + ModLocation).Close();
                CFG = new Config();
                SaveConfig();
            }
            else
            {
                CFG = Yml.Deserialize<Config>(File.ReadAllText(MainFolder + "//" + ModLocation));
                if (CFG == null)
                    CFG = new Config();
                SaveConfig();
            }
            return CFG;
        }
    }
}
