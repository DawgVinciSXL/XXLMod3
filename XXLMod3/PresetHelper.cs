using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXLMod3
{
    public static class PresetHelper
    {
        public static string GrindPresetsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\XXLMod3\\GrindPresets\\";
        public static string LegPresetsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\XXLMod3\\LegPresets\\";
        public static string StancePresetsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\XXLMod3\\StancePresets\\";
        public static string StatsPresetsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\XXLMod3\\StatsPresets\\";

        public static string[] GrindPresets;
        public static string[] LegPresets;
        public static string[] StancePresets;
        public static string[] StatsPresets;

        public static void CreateFolder()
        {
            if (!Directory.Exists(GrindPresetsPath))
            {
                Directory.CreateDirectory(GrindPresetsPath);
            }

            if (!Directory.Exists(LegPresetsPath))
            {
                Directory.CreateDirectory(LegPresetsPath);
            }

            if (!Directory.Exists(StancePresetsPath))
            {
                Directory.CreateDirectory(StancePresetsPath);
            }

            if (!Directory.Exists(StatsPresetsPath))
            {
                Directory.CreateDirectory(StatsPresetsPath);
            }
        }

        public static void GetPresets()
        {
            GrindPresets = (from file in Directory.EnumerateFiles(Path.Combine(GrindPresetsPath), "*.json", SearchOption.AllDirectories)
                            where file.Contains(".json")
                            select file).ToArray<string>();

            LegPresets = (from file in Directory.EnumerateFiles(Path.Combine(LegPresetsPath), "*.json", SearchOption.AllDirectories)
                          where file.Contains(".json")
                          select file).ToArray<string>();

            StancePresets = (from file in Directory.EnumerateFiles(Path.Combine(StancePresetsPath), "*.xml", SearchOption.AllDirectories)
                            where file.Contains(".xml")
                            select file).ToArray<string>();

            StatsPresets = (from file in Directory.EnumerateFiles(Path.Combine(StatsPresetsPath), "*.xml", SearchOption.AllDirectories)
                            where file.Contains(".xml")
                            select file).ToArray<string>();
        }
    }
}
