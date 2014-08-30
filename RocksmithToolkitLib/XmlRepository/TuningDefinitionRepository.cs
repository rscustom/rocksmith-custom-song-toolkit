using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib {
    public class TuningDefinitionRepository : XmlRepository<TuningDefinition>
    {
        private static readonly Lazy<TuningDefinitionRepository> instance = new Lazy<TuningDefinitionRepository>(() => new TuningDefinitionRepository());

        private const string FILENAME = "RocksmithToolkitLib.TuningDefinition.xml";

        public static TuningDefinitionRepository Instance() { return instance.Value; }

        public TuningDefinitionRepository() : base(FILENAME, new TuningComparer()) { }

        public IEnumerable<TuningDefinition> Select(GameVersion gameVersion)
        {
            return List.OfType<TuningDefinition>().Where(s => s.GameVersion == gameVersion);
        }

        public IEnumerable<TuningDefinition> Select(TuningStrings tuningStrings)
        {
            return List.OfType<TuningDefinition>().Where(s => s.Tuning.ToArray().SequenceEqual(tuningStrings.ToArray()));
        }

        [Obsolete("This function is deprecated due to low accuracy. Use SelectAny() instead.", true)]
        public TuningDefinition Select(string uiName, GameVersion gameVersion)
        {
            return List.FirstOrDefault<TuningDefinition>(s => s.UIName == uiName && s.GameVersion == gameVersion);
        }

        public TuningDefinition SelectAny(TuningStrings tuningStrings, GameVersion gameVersion)
        {
        	var g = Select(tuningStrings,gameVersion);
        	var b = SelectForBass(tuningStrings,gameVersion);
        	if (ReferenceEquals(g, b)) return g;
        	else return b;
        }
        //Tuning Strings + GameVersion
        public TuningDefinition Select(TuningStrings tuningStrings, GameVersion gameVersion)
        {
            return List.FirstOrDefault<TuningDefinition>(s => s.Tuning.ToArray().SequenceEqual(tuningStrings.ToArray()) && s.GameVersion == gameVersion);
        }

        public TuningDefinition SelectForBass(TuningStrings tuningStrings, GameVersion gameVersion)
        {
            return List.FirstOrDefault<TuningDefinition>(s => s.Tuning.ToBassArray().SequenceEqual(tuningStrings.ToBassArray()) && s.GameVersion == gameVersion);
        }

        public TuningDefinition Select(int[] tuningStrings, GameVersion gameVersion)
        {
            return List.FirstOrDefault<TuningDefinition>(s => s.Tuning.ToArray().SequenceEqual(tuningStrings) && s.GameVersion == gameVersion);
        }

        public TuningDefinition SelectForBass(int[] tuningStrings, GameVersion gameVersion)
        {
            return List.FirstOrDefault<TuningDefinition>(s => s.Tuning.ToBassArray().SequenceEqual(tuningStrings) && s.GameVersion == gameVersion);
        }
    }

    internal class TuningComparer : IEqualityComparer<TuningDefinition>
    {
        public bool Equals(TuningDefinition x, TuningDefinition y)
        {
            if (x == null || y == null)
                return false;

            return (x.GameVersion == y.GameVersion && x.Tuning == y.Tuning);
        }

        public int GetHashCode(TuningDefinition obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.GameVersion.GetHashCode() ^ obj.Tuning.GetHashCode();
        }
    }
}
