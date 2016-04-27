using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using RocksmithToolkitLib.Xml;


namespace RocksmithToolkitLib
{
    public class TuningDefinitionRepository : XmlRepository<TuningDefinition>
    {
        private static readonly Lazy<TuningDefinitionRepository> instance = new Lazy<TuningDefinitionRepository>(() => new TuningDefinitionRepository());

        private const string FILENAME = "RocksmithToolkitLib.TuningDefinition.xml";

        // TODO: consider changing FirstOrDefualt to SingleOrDefault so that
        // error is thrown if the same tuning appears in the database more than once

        // this load method is producing unexpected results if used to fill combobox 
        // custom tunings are being added and referenced undesirably in memory
        public static TuningDefinitionRepository Instance
        {
            get { return instance.Value; }
        }

        public TuningDefinitionRepository()
            : base(FILENAME, new TuningComparer())
        {
        }

        #region Select
        public IEnumerable<TuningDefinition> Select(GameVersion gameVersion)
        {
            return List.Where(s => s.GameVersion == gameVersion);
        }

        public IEnumerable<TuningDefinition> Select(TuningStrings tuningStrings)
        {
            return List.Where(s => s.Tuning.Equals(tuningStrings));
        }

        [Obsolete("This function is deprecated due to low accuracy. Use SelectAny() instead.", true)]
        public TuningDefinition Select(string uiName, GameVersion gameVersion)
        {
            return List.FirstOrDefault(s => s.UIName == uiName && s.GameVersion == gameVersion);
        }

        public TuningDefinition SelectAny(TuningStrings tuningStrings, GameVersion gameVersion)
        {
            var g = Select(tuningStrings, gameVersion);
            return g; //Accurate compare, no mercy for bass.
        }

        //Tuning Strings + GameVersion
        public TuningDefinition Select(TuningStrings tuningStrings, GameVersion gameVersion)
        {
            return List.FirstOrDefault(s => s.Tuning.ToArray().SequenceEqual(tuningStrings.ToArray()) && s.GameVersion == gameVersion);
        }

        public TuningDefinition Select(Int16[] tuningStrings, GameVersion gameVersion)
        {
            return List.FirstOrDefault(s => s.Tuning.ToArray().SequenceEqual(tuningStrings) && s.GameVersion == gameVersion);
        }

        [Obsolete("Depricated, please use TuningDefinition Select() funtion.", true)]
        public TuningDefinition SelectForBass(TuningStrings tuningStrings, GameVersion gameVersion)
        {
            return List.FirstOrDefault(s => s.Tuning.ToBassArray().SequenceEqual(tuningStrings.ToBassArray()) && s.GameVersion == gameVersion);
        }

        [Obsolete("Depricated, please use TuningDefinition Select() funtion.", true)]
        public TuningDefinition SelectForBass(Int16[] tuningStrings, GameVersion gameVersion)
        {
            return List.FirstOrDefault(s => s.Tuning.ToBassArray().SequenceEqual(tuningStrings) && s.GameVersion == gameVersion);
        }
        #endregion

        // LINQ method produces unexpected results on subsequent calls
        // so produce expected results the good old fashioned way
        public List<TuningDefinition> LoadTuningDefinitions(GameVersion gameVersion)
        {
            string tdFilePath = Path.Combine(Application.StartupPath, FILENAME);
            return TuningDefinition.LoadFile(tdFilePath, gameVersion);
        }

        public void SaveFile(TuningDefinition customTuningDefinition)
        {
            string tdFilePath = Path.Combine(Application.StartupPath, FILENAME);

            if (File.Exists(tdFilePath))
                File.Delete(tdFilePath);

        }

        public TuningDefinition Detect(TuningStrings tuning, GameVersion gameVersion, bool addreload = false)
        {
            var t = Instance.Select(tuning, gameVersion);
            // if unknown tuning then register it at tunung definition
            if (t == null)
            {
                t = new TuningDefinition(tuning, gameVersion);
                // only add full(guitar one) arrangement tunings to the TuningDefinitionRepository
                if (addreload)
                {
                    Add(t, true);
                }
            }
            return t;
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
            if (ReferenceEquals(obj, null))
                return 0;

            return obj.GameVersion.GetHashCode() ^ obj.Tuning.GetHashCode();
        }
    }
}


