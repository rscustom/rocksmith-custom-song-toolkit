using System;
using System.Collections.Generic;
using System.Linq;

namespace RocksmithToolkitLib.XmlRepository {
    public class SongAppIdRepository : XmlRepository<SongAppId> {
        private static readonly Lazy<SongAppIdRepository> instance = new Lazy<SongAppIdRepository>(() => new SongAppIdRepository());

        private const string FILENAME = "RocksmithToolkitLib.SongAppId.xml";

        public static SongAppIdRepository Instance() { return instance.Value; }

        public SongAppIdRepository() : base(FILENAME, new AppIdComparer()) { }

        public SongAppIdRepository Load(string appidXml)
        {
            if (!string.IsNullOrEmpty(appidXml))
            {
                FileName = appidXml;
                Load();
            }
            return this;
        }

        public SongAppId Select(string appId, GameVersion gameVersion)
        {
            if (List.Any(s => s.AppId == appId && s.GameVersion == gameVersion))
                return List.FirstOrDefault<SongAppId>(s => s.AppId == appId);
            return List[0];
        }

        public IEnumerable<SongAppId> Select(GameVersion gameVersion)
        {
            return List.Where(s => s.GameVersion == gameVersion).OrderBy(s => s.Name);
        }
    }

    internal class AppIdComparer : IEqualityComparer<SongAppId>
    {
        public bool Equals(SongAppId x, SongAppId y)
        {
            if (x == null || y == null)
                return false;

            return (x.GameVersion == y.GameVersion && x.AppId == y.AppId);
        }

        public int GetHashCode(SongAppId obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.GameVersion.GetHashCode() ^ obj.GameVersion.GetHashCode() + obj.AppId.GetHashCode();
        }
    }
}
