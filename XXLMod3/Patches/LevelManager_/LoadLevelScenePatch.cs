using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace XXLMod3.Patches.LevelManager_
{
    //Code & permission by Mcbtay
    public class LevelManagerPatch
    {
        [HarmonyPatch(typeof(LevelManager), nameof(LevelManager.LoadLevelScene))]
        public static class LoadLevelScenePatch
        {
            static void Prefix(LevelInfo level)
            {
                var path = Path.GetDirectoryName(level.path);
                var file = Path.Combine(path, level.name + ".dll");
                if (Directory.Exists(path) && File.Exists(file))
                {
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => !x.IsDynamic && Path.GetFileName(x.Location).StartsWith(level.name));
                    if (assembly == null)
                    {
                        Assembly.LoadFile(file);
                    }
                }
            }
        }
    }
}