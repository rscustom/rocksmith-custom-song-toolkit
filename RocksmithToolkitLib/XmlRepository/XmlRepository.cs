using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib {
    public abstract class XmlRepository<T> {
        #region Events

        public delegate void OnSavingHandler();
        public delegate void OnSavedHandler();

        public event OnSavingHandler OnSaving;
        public event OnSavedHandler OnSaved;

        #endregion

        #region Fields

        /// <summary>
        /// Repository file name i.e.: RocksmithToolkitLib.SongAppId.xml
        /// </summary>
        string _fileName;
        protected string FileName {
            set{
                _fileName = value;
                FilePath = Path.Combine(Application.StartupPath, value);
            }
            get { return _fileName; }
        }

        /// <summary>
        /// Comparer to be used on Merge by different types
        /// </summary>
        protected IEqualityComparer<T> Comparer;

        public string FilePath;

        /// <summary>                                                  (
        /// List of objects from *.xml file
        /// </summary>
        public List<T> List;

        #endregion

        protected XmlRepository(string fileName, IEqualityComparer<T> comparer) {
            FileName = fileName;
            Comparer = comparer;
            List = Activator.CreateInstance<List<T>>();
            Load();
        }

        /// <summary>
        /// Persist object list into *.xml file
        /// </summary>
        public void Save(bool reload = false) {
            if (OnSaving != null)
                OnSaving.Invoke();

            lock (List) {
                using (FileStream writer = File.Create(FilePath)) {
                    var serializer = new XmlSerializer(List.GetType());
                    serializer.Serialize(writer, List);
                }
            }

            if (OnSaved != null)
                OnSaved.Invoke();

            if (reload)
                Load();
        }

        /// <summary>
        /// Add item into object list
        /// </summary>
        /// <param name="value">object</param>
        /// <param name = "reload"></param>
        public void Add(T value, bool reload = false) {
            List.Add(value);
            Save(reload);
        }

        /// <summary>
        /// Refresh list from *.xml file
        /// </summary>
        public void Load() {
            if (File.Exists(FilePath)) {
                lock (List) {
                    using (var reader = new StreamReader(FilePath))
                    {
                        var serializer = new XmlSerializer(typeof(List<T>));
                        List = (List<T>)serializer.Deserialize(reader);
                    }
                }
            } else {
                List = new List<T>();
                Save();
            }
        }

        /// <summary>
        /// Merge two xml repositories into one
        /// </summary>
        /// <param name="sourceFile">XML source file</param>
        /// <param name="destinationFile">XML destination file</param>
        public void Merge(string sourceFile, string destinationFile)
        {
            // Load source repository
            FileName = sourceFile;
            List = Activator.CreateInstance<List<T>>();
            Load();
            var sourceRepoList = GeneralExtensions.Copy<List<T>>(List);

            // Load destination repository
            FileName = destinationFile;
            List = Activator.CreateInstance<List<T>>();
            Load();

            // Merge source to destination
            List = List.Union<T>(sourceRepoList, Comparer).ToList<T>();

            // Save
            Save();
        }
    }
}
