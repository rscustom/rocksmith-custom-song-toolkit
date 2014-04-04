using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace RocksmithToolkitLib {
    public class ConfigRepository : XmlRepository<Config> {
        private static readonly Lazy<ConfigRepository> instance = new Lazy<ConfigRepository>(() => new ConfigRepository());

        private const string FILENAME = "RocksmithToolkitLib.Config.xml";

        public static ConfigRepository Instance() { return instance.Value; }

        public ConfigRepository() : base(FILENAME, new ConfigComparer()) { }

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
            set {
                if (Exists(configKey)) {
                    Config conf = List.FirstOrDefault<Config>(s => s.Key == configKey);
                    conf.Value = value;
                    Save();
                } else {
                    this.Add(new Config(){ Key = configKey, Value = value });
                }
            }
        }

        public string this[Int32 index] {
            get {
                return List[index].Value;
            }
        }

        public bool Exists(string configKey) {
            return List.OfType<Config>().Where(s => s.Key == configKey).Count() > 0;
        }

        public bool ValueChanged(string configKey, object value) {            
            if (Exists(configKey)) {
                Config conf = List.FirstOrDefault<Config>(s => s.Key == configKey);
                if (conf.Value.Equals(value))
                    return false;
            }

            return true;
        }

        public bool GetBoolean(string configKey) {
            return Convert.ToBoolean(List.FirstOrDefault<Config>(s => s.Key == configKey).Value);
        }

        public Int32 GetInt32(string configKey) {
            return Convert.ToInt32(List.FirstOrDefault<Config>(s => s.Key == configKey).Value);
        }

        public decimal GetDecimal(string configKey) {
            return Convert.ToDecimal(List.FirstOrDefault<Config>(s => s.Key == configKey).Value);
        }
    }

    internal class ConfigComparer : IEqualityComparer<Config>
    {
        public bool Equals(Config x, Config y)
        {
            if (x == null || y == null)
                return false;

            return (x.Key == y.Key);
        }

        public int GetHashCode(Config obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.Key.GetHashCode() ^ obj.Key.GetHashCode();
        }
    }
}
