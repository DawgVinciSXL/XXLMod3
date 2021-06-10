using System;
using System.IO;
using System.Linq;

namespace XXLMod3
{
    public static class PresetHelper
    {
        public static string XXL3Path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/SkaterXL/XXLMod3/";
        public static string GrindPresetsPath = XXL3Path + "GrindPresets/";
        public static string LegPresetsPath = XXL3Path + "LegPresets/";
        public static string StancePresetsPath = XXL3Path + "StancePresets/";
        public static string StatsPresetsPath = XXL3Path + "StatsPresets/";

        public static string[] GrindPresets;
        public static string[] LegPresets;
        public static string[] StancePresets;
        public static string[] StatsPresets;

        public static void CreateFolder()
        {
            if (!Directory.Exists(GrindPresetsPath)) Directory.CreateDirectory(GrindPresetsPath);

            if (!Directory.Exists(LegPresetsPath)) Directory.CreateDirectory(LegPresetsPath);

            if (!Directory.Exists(StancePresetsPath)) Directory.CreateDirectory(StancePresetsPath);

            if (!Directory.Exists(StatsPresetsPath)) Directory.CreateDirectory(StatsPresetsPath);
        }

        public static void GetPresets()
        {
            GrindPresets = (from file in Directory.EnumerateFiles(Path.Combine(GrindPresetsPath), "*.json", SearchOption.AllDirectories)
                            where file.Contains(".json")
                            select file).ToArray();

            LegPresets = (from file in Directory.EnumerateFiles(Path.Combine(LegPresetsPath), "*.json", SearchOption.AllDirectories)
                          where file.Contains(".json")
                          select file).ToArray();

            StancePresets = (from file in Directory.EnumerateFiles(Path.Combine(StancePresetsPath), "*.json", SearchOption.AllDirectories)
                            where file.Contains(".json")
                            select file).ToArray();

            StatsPresets = (from file in Directory.EnumerateFiles(Path.Combine(StatsPresetsPath), "*.xml", SearchOption.AllDirectories)
                            where file.Contains(".xml")
                            select file).ToArray();
        }

        public static void DeletePreset(string filePath)
        {
            File.Delete(filePath);
        }
    }
}