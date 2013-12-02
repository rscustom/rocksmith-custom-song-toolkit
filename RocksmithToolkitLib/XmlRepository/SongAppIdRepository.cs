using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace RocksmithToolkitLib.DLCPackage {
    public class SongAppIdRepository : XmlRepository<SongAppId> {
        private static readonly Lazy<SongAppIdRepository> instance = new Lazy<SongAppIdRepository>(() => new SongAppIdRepository());

        private const string FILENAME = "RocksmithToolkitLib.SongAppId.xml";

        public static SongAppIdRepository Instance() { return instance.Value; }

        public SongAppIdRepository() : base(FILENAME) { }

        public SongAppId Select(string appId, GameVersion gameVersion)
        {
            if (List.OfType<SongAppId>().Where(s => s.AppId == appId && s.GameVersion == gameVersion).Count() > 0)
                return List.Single<SongAppId>(s => s.AppId == appId);
            else
                return List[0];
        }

        public IEnumerable<SongAppId> Select(GameVersion gameVersion)
        {
            return List.OfType<SongAppId>().Where(s => s.GameVersion == gameVersion);
        }
    }
}
