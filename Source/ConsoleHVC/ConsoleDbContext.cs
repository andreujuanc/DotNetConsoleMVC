using ConsoleHVC;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleHVC
{
    public abstract class ConsoleDbContext
    {
        private string _dataFile = null;
        private bool _isConfigured = false;
        private string DataFile { get { return _dataFile; } set { _dataFile = value; LoadFile(); } }
        private dynamic StoredObject { get; set; }


        private void LoadFile()
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
                    if(cfs!=null) cfs.Close();
                }   
            }
            using (var fs = File.OpenRead(DataFile))
            {
                XmlSerializer ser = null;
                try
                {
                    ser = new System.Xml.Serialization.XmlSerializer(typeof(ExpandoObject));
                    StoredObject = ser.Deserialize(fs) as ExpandoObject;
                }
                catch
                {
                    StoredObject = new ExpandoObject();
                }
                finally
                {
                    ser = null;
                }
            }
        }

        internal void Configure()
        {
            var options = new ConsoleDbContextOptions();
            OnConfiguring(options);
            if (options.DataFile == null) throw new InvalidOperationException("Context Datafile cannot be null");
            DataFile = options.DataFile;
            _isConfigured = true;
        }
        public abstract void OnConfiguring(ConsoleDbContextOptions options);
        public void SaveChanges()
        {
            
        }

        protected T Get<T>()
        {
            if (!_isConfigured) Configure();
            var PropertyName = typeof(T).Name;
            var tmp = StoredObject as IDictionary<String, object>;
            //var property = StoredObject.GetType().GetProperty(typeof(T).Name);

            if (!tmp.ContainsKey(PropertyName))
            {
                tmp.Add(PropertyName, Activator.CreateInstance<T>());
                //StoredObject = tmp;
                //property = StoredObject.GetType().GetProperty(typeof(T).Name);
            }
            //StoredObject.GetType().GetProperties
            //return (T)property.GetValue(StoredObject);
            return (T)tmp[PropertyName];
        }
        private class ConsoleDbDataStorage 
        {

        }
    }
}
