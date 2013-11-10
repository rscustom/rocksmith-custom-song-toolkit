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
            if (List.OfType<TuningDefinition>().Where(s => s.UIName == uiName && s.GameVersion == gameVersion).Count() > 0)
                return List.Single<TuningDefinition>(s => s.UIName == uiName);
            else
                return List[0];
        }

        public IEnumerable<TuningDefinition> Select(GameVersion gameVersion)
        {
            return List.OfType<TuningDefinition>().Where(s => s.GameVersion == gameVersion);
        }

        public IEnumerable<TuningDefinition> Select(TuningStrings tuningStrings)
        {
            return List.OfType<TuningDefinition>().Where(s => s.Tuning == tuningStrings);
        }

        public IEnumerable<TuningDefinition> Select(int[] tuningStrings)
        {
            return List.OfType<TuningDefinition>().Where(s => s.Tuning.ToArray().SequenceEqual(tuningStrings));
        }
    }
}
