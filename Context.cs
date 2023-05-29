using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MonitorClickerWPF
{
    public static class Context
    {
        const string file = "settings.xml";
        public static Settings Settings { get; set; } = new();

        public static void Load()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                using var steam = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);
                Settings = serializer.Deserialize(steam) as Settings;
                if (Settings == null) Settings = new();
            }
            catch { Settings = new(); }
        }

        public static void Save()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                using var steam = new FileStream(file, FileMode.Truncate);
                serializer.Serialize(steam, Settings);
            }
            catch { Settings = new(); }
        }
    }

    public class Settings
    {
        public int ClickDelay { get; set; } = 300;
        public float ClickAveryMin { get; set; } = 10;
        public BindingList<Point> Points { get; set; } = new();
    }
}
