using System.IO;
using System.Xml.Serialization;
//using Newtonsoft.Json;

namespace AutoCADNetLoadManager
{
    public class XmlUtils
    {
        public static void SerializeToXml(object item, string targetFile)
        {
            string path = Path.GetDirectoryName(targetFile);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var serializer = new XmlSerializer(item.GetType());
            using (var sw = new StreamWriter(targetFile))
            {
                serializer.Serialize(sw, item);
            }

            //string jsonDatas = ToJson(datas);
            //string path = Path.GetDirectoryName(DataPath);
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}
            //using (StreamWriter sw = new StreamWriter(DataPath))
            //{
            //    sw.WriteLine(jsonDatas);
            //    sw.Close();
            //    sw.Dispose();
            //}
        }

        public static T DeserializeFromXml<T>(string sourceFile)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(sourceFile))
            {
                var data = serializer.Deserialize(reader);
                return (T)data;
            }
        }

        //private string ToJson(object obj)
        //{
        //    lock (obj)
        //    {
        //        try
        //        {
        //            return JsonConvert.SerializeObject(obj);
        //        }
        //        catch
        //        {
        //        }
        //        return null;
        //    }
        //}

        // Datas datas = JsonConvert.DeserializeObject<Datas>(jsonDatas);
    }
}
