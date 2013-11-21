using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage {
    public class TuningDefinitionRepository : XmlRepository<TuningDefinition>
    {
        private static readonly Lazy<TuningDefinitionRepository> instance = new Lazy<TuningDefinitionRepository>(() => new TuningDefinitionRepository());

        private const string FILENAME = "RocksmithToolkitLib.TuningDefinition.xml";

        public static TuningDefinitionRepository Instance() { return instance.Value; }

        public TuningDefinitionRepository() : base(FILENAME) { }

        public TuningDefinition Select(string uiName, GameVersion gameVersion)
        {
            return List.SingleOrDefault<TuningDefinition>(s => s.UIName == uiName && s.GameVersion == gameVersion);
        }

        public IEnumerable<TuningDefinition> Select(GameVersion gameVersion)
        {
            return List.OfType<TuningDefinition>().Where(s => s.GameVersion == gameVersion);
        }

        public IEnumerable<TuningDefinition> Select(TuningStrings tuningStrings)
        {
            return List.OfType<TuningDefinition>().Where(s => s.Tuning.ToArray().SequenceEqual(tuningStrings.ToArray()));
        }

        public TuningDefinition Select(TuningStrings tuningStrings, GameVersion gameVersion) {
            var tuning = List.SingleOrDefault<TuningDefinition>(s => s.Tuning.ToArray().SequenceEqual(tuningStrings.ToArray()) && s.GameVersion == gameVersion);
            return tuning;
        }

        public TuningDefinition Select(int[] tuningStrings, GameVersion gameVersion)
        {
            return List.SingleOrDefault<TuningDefinition>(s => s.Tuning.ToArray().SequenceEqual(tuningStrings) && s.GameVersion == gameVersion);
        }
    }
}
