using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace RocksmithToolkitLib.DLCPackage {
    public class ConfigRepository : XmlRepository<Config> {
        private static readonly Lazy<ConfigRepository> instance = new Lazy<ConfigRepository>(() => new ConfigRepository());

        private const string FILENAME = "RocksmithToolkitLib.Config.xml";

        public static ConfigRepository Instance() { return instance.Value; }

        public ConfigRepository() : base(FILENAME) { }

        public Config Select(string configKey)
        {
            if (List.OfType<Config>().Where(s => s.Key == configKey).Count() > 0)
                return List.FirstOrDefault<Config>(s => s.Key == configKey);
            else
                return List[0];
        }

        public string this[string configKey]
        {
            get {
                return List.FirstOrDefault<Config>(s => s.Key == configKey).Value;
            }
        }

        public string this[Int32 index] {
            get {
                return List[index].Value;
            }
        }
    }
}
