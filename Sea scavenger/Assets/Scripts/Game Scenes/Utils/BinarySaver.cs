using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

///Original by Neodrop. neodrop@unity3d.ru
public static class BinarySaver
{
    public static void Save(object obj, string fileName)
    {
        Save(obj, fileName, null);
    }

    public static void Save(object obj, string fileName, SurrogateSelector surrogateSelector)
    {
        using (FileStream fs = new FileStream(fileName, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            if (surrogateSelector != null)
                formatter.SurrogateSelector = surrogateSelector;

            formatter.Serialize(fs, obj);
        }
    }

    public static object Load(string fileName, SurrogateSelector surrogateSelector = null)
    {
        if (!File.Exists(fileName))
            return null;

        using (FileStream fs = new FileStream(fileName, FileMode.Open))
        {
            object obj = null;
            BinaryFormatter formatter = new BinaryFormatter();

            if (surrogateSelector != null)
                formatter.SurrogateSelector = surrogateSelector;

            obj = formatter.Deserialize(fs);

            return obj;
        }
    }

    public static T Load<T>(string fileName, SurrogateSelector surrogateSelector = null)
    {
        return (T)Load(fileName, surrogateSelector);
    }
}