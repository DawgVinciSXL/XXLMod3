using static UnityModManagerNet.UnityModManager;
using System.IO;
using System.Xml.Serialization;
using System;

namespace XXLMod3.Core
{
    public class SaveStats : UnityModManagerNet.UnityModManager.ModSettings
    {
        public void Save(ModEntry modEntry, string filepath)
        {
            Save(this, modEntry, filepath);
        }

        public static void Save<T>(T data, ModEntry modEntry, string filepath) where T : ModSettings, new()
        {
            Save<T>(data, modEntry, filepath, null);
        }

        public static void Save<T>(T data, ModEntry modEntry, string filepath, XmlAttributeOverrides attributes) where T : ModSettings, new()
        {
            try
            {
                using (var writer = new StreamWriter(filepath))
                {
                    var serializer = new XmlSerializer(typeof(T), attributes);
                    serializer.Serialize(writer, data);
                }
            }
            catch (Exception e)
            {
                modEntry.Logger.Error($"Can't save {filepath}.");
                modEntry.Logger.LogException(e);
            }
        }

        public static T Load<T>(ModEntry modEntry, string filepath) where T : ModSettings, new()
        {
            var t = new T();
            if (File.Exists(filepath))
            {
                try
                {
                    using (var stream = File.OpenRead(filepath))
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        var result = (T)serializer.Deserialize(stream);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    UnityModManagerNet.UnityModManager.Logger.Log($"Can't read {filepath}.");
                    UnityModManagerNet.UnityModManager.Logger.LogException(e);
                }
            }

            return t;
        }

        public static T Load<T>(ModEntry modEntry, string filepath, XmlAttributeOverrides attributes) where T : ModSettings, new()
        {
            var t = new T();
            filepath = Main.modEntry.Path + "test.xml";
            if (File.Exists(filepath))
            {
                try
                {
                    using (var stream = File.OpenRead(filepath))
                    {
                        var serializer = new XmlSerializer(typeof(T), attributes);
                        var result = (T)serializer.Deserialize(stream);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    UnityModManagerNet.UnityModManager.Logger.Log($"Can't read {filepath}.");
                    UnityModManagerNet.UnityModManager.Logger.LogException(e);
                }
            }

            return t;
        }
    }
}
