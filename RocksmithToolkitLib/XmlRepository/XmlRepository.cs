using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace RocksmithToolkitLib.DLCPackage {
    public abstract class XmlRepository<T> {
        #region Events
        
        public delegate void OnSavingHandler();
        public delegate void OnSavedHandler();
        
        public event OnSavingHandler OnSaving;
        public event OnSavedHandler OnSaved;
        
        #endregion
        /// <summary>
        /// Repository file name i.e.: RocksmithToolkitLib.SongAppId.xml
        /// </summary>
        protected string FileName;

        public string FilePath {
            get {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileName);
            }
        }

        /// <summary>
        /// List of objects from *.xml file
        /// </summary>
        public List<T> List { get; set; }

        protected XmlRepository(string fileName) {
            FileName = fileName;
            List = Activator.CreateInstance<List<T>>();
            Load();
        }

        /// <summary>
        /// Persist object list into *.xml file
        /// </summary>
        public void Save() {
            if (OnSaving != null)
                this.OnSaving.Invoke();

            lock (List) {
                using (FileStream writer = File.Create(FilePath)) {
                    XmlSerializer serializer = new XmlSerializer(List.GetType());
                    serializer.Serialize(writer, List);
                }
            }

            if (OnSaved != null)
                this.OnSaved.Invoke();
        }

        /// <summary>
        /// Add item into object list
        /// </summary>
        /// <param name="value">object</param>
        public void Add(T value) {
            List.Add(value);
            Save();
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
    }
}
