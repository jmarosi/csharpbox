using System.Collections.Generic;

namespace Utilities
{
    public abstract class DeserializerBase // REV: Inkább DeserializableBase a felhasználás alapján
    {
        public virtual void AssignValues(string[] propertyValues, List<string> ignoredProperties = null)
        {
            var properties = GetType().GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                if (ignoredProperties == null || ignoredProperties.Contains(properties[i].Name) == false)
                {
                    var type = properties[i].PropertyType.Name;
                    switch (type)  // REV: Értem a switch-et, de én inkább if-else és typeof kombóval mennék; ez a stringes dolog nagyon hamar el lesz írva 
                    {
                        case "Int32":
                            properties[i].SetValue(this, int.Parse(propertyValues[i]));
                            break;
                        case "UInt32":
                            properties[i].SetValue(this, uint.Parse(propertyValues[i]));
                            break;
                        case "UInt16":
                            properties[i].SetValue(this, ushort.Parse(propertyValues[i]));
                            break;
                        case "Double":
                            properties[i].SetValue(this, double.Parse(propertyValues[i]));
                            break;
                        default:
                            properties[i].SetValue(this, propertyValues[i]);
                            break;
                    }
                }
            }
        }
    }
}