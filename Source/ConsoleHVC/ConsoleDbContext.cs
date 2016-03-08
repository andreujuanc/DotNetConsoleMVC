using ConsoleHVC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleHVC
{
    public abstract class ConsoleDbContext
    {
        private string _dataFile = null;
        private bool _isConfigured = false;

        private string DataFile { get { return _dataFile; } set { _dataFile = value; LoadFile(); } }
        private Storage StoredObject { get; set; } = new Storage();

        public abstract void OnConfiguring(ConsoleDbContextOptions options);

        internal void Configure()
        {
            try
            {
                _isConfigured = true;
                var options = new ConsoleDbContextOptions();
                OnConfiguring(options);
                if (options.DataFile == null) throw new InvalidOperationException("Context Datafile cannot be null");
                DataFile = options.DataFile;
            }
            catch
            {
                _isConfigured = false;
            }
        }
        private void CheckFile()
        {
            if (!File.Exists(DataFile))
            {
                FileStream cfs = null;
                try
                {
                    cfs = File.Create(DataFile);
                    cfs.Close();
                }
                catch
                {
                    if (cfs != null) cfs.Close();
                }
            }
        }

        private void LoadFile()
        {
            CheckConfigure();
            CheckFile();
            using (var fs = File.OpenText(DataFile))
            {
                try
                {
                    StoredObject = JsonConvert.DeserializeObject<Storage>(fs.ReadToEnd());
                }
                catch
                {
                    StoredObject = new Storage();
                }
                finally
                {
                    if (StoredObject == null) StoredObject = new Storage();
                }
            }
        }

        public void SaveChanges()
        {
            CheckConfigure();
            CheckFile();
            try
            {

                var fileContent = JsonConvert.SerializeObject(StoredObject);
                File.WriteAllText(DataFile, fileContent);

            }
            finally
            {

            }
        }

        private void CheckConfigure()
        {
            if (!_isConfigured) Configure();
        }

        protected T Get<T>()
        {
            CheckConfigure();
            if (StoredObject == null) return default(T);
            var PropertyName = typeof(T).Name;
            

            if (!StoredObject.ContainsKey(PropertyName))
            {
                StoredObject.Add(PropertyName, Activator.CreateInstance<T>());
            }
            object value = StoredObject[PropertyName];
            if (value is Newtonsoft.Json.Linq.JObject)
            {
                value = ((Newtonsoft.Json.Linq.JObject)value).ToObject<T>();
            }
            StoredObject[PropertyName] = value;

            return (T)value;
        }
        protected void Set<T>(object value)
        {
            CheckConfigure();
            var PropertyName = typeof(T).Name;
            
            if (!StoredObject.ContainsKey(PropertyName))
            {
                StoredObject.Add(PropertyName, value);
            }
            else
            {
                StoredObject[PropertyName] = value;
            }
        }

        [Serializable]
        private class Storage : Dictionary<string, object>
        {
           
        }
    }
}
