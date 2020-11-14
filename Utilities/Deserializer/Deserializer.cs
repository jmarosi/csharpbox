using System.Collections.Generic;
using System.IO;

namespace Utilities
{
    public class Deserializer<T> where T : DeserializerBase, new()
    {
        public List<T> Deserialize(string filePath, List<string> ignoredPropertyNames = null)
        {
            var objects = new List<T>();
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                streamReader.ReadLine(); // skip the 1st line
                while (streamReader.EndOfStream == false)
                {
                    var obj = new T();
                    var propertyValues = streamReader.ReadLine().Split('\t');
                    obj.AssignValues(propertyValues, ignoredPropertyNames);
                    objects.Add(obj);
                }
            }
            return objects;
        }
    }
}