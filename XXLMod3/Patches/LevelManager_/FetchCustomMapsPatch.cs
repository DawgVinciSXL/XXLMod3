//using HarmonyLib;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEngine;
//using XXLMod3.Core;

//namespace XXLMod3.Patches.LevelManager_
//{
//    [HarmonyPatch(typeof(LevelManager), "FetchCustomMaps")]
//    class FetchCustomMapsPatch
//    {
//        static void Postfix(LevelManager __instance)
//        {
//            if (Main.settings.AlternativeLevelDir == "None")
//            {
//                return;
//            }
//            List<LevelInfo> modLevels = __instance.ModLevels;
//            __instance.ModLevels.AddRange(GetMapsFromAlternativePath());
//            for (int i = 0; i < modLevels.Count; i++)
//            {
//                LevelInfo oldLvl = modLevels[i];
//                if (oldLvl.isAssetBundle && !string.IsNullOrEmpty(oldLvl.path))
//                {
//                    LevelInfo levelInfo = __instance.ModLevels.FirstOrDefault((LevelInfo l) => l.path == oldLvl.path);
//                    if (levelInfo == null)
//                    {
//                        string text;
//                        __instance.AssetBundlePathToHashDict.TryRemove(oldLvl.path, out text);
//                        Debug.Log("Custom Level " + oldLvl.name + " was removed" + ((__instance.currentLevel == oldLvl) ? " while being loaded as current level" : ""));
//                    }
//                    else if (levelInfo.modioID == 0 && string.IsNullOrEmpty(levelInfo.hash) && !string.IsNullOrEmpty(oldLvl.hash))
//                    {
//                        levelInfo.hash = oldLvl.hash;
//                    }
//                }
//            }
//            __instance.CacheCustomLevels();
//            __instance.StartHashing();
//            Extentions.InvokeMethod(__instance, "UpdateHashToLevelDict");
//        }

//        public static List<LevelInfo> GetMapsFromAlternativePath()
//        {
//            List<LevelInfo> list = new List<LevelInfo>();
//            if (SaveManager.Instance.modInfoCache != null)
//            {
//                SaveManager.ModInfo[] array = (from p in SaveManager.Instance.modInfoCache.installedMods
//                                               where p.typeTag == "Map"
//                                               select p).ToArray<SaveManager.ModInfo>();
//                Debug.Log(string.Concat(new object[]
//                {
//                array.Count<SaveManager.ModInfo>(),
//                " Mods out of ",
//                SaveManager.Instance.modInfoCache.installedMods.Count,
//                " are Maps"
//                }));
//                foreach (SaveManager.ModInfo modInfo in array)
//                {
//                    LevelInfo levelInfo = SaveManager.Instance.LevelInfoForMod(modInfo);
//                    if (levelInfo != null)
//                    {
//                        list.Add(levelInfo);
//                    }
//                }
//            }
//            if (Directory.Exists(Main.settings.AlternativeLevelDir))
//            {
//                list.AddRange(from path in Directory.GetFiles(Main.settings.AlternativeLevelDir, "*", SearchOption.AllDirectories).Where(delegate (string p)
//                {
//                    string extension = Path.GetExtension(p.ToLower());
//                    return !(extension == ".dll") && !(extension == ".png") && !(extension == ".jpg") && !(extension == ".jpeg") && !(extension == ".meta");
//                })
//                              select new LevelInfo(path, true));
//            }
//            else
//            {
//                Debug.Log(Main.settings.AlternativeLevelDir + " does not exists or does not contain valid maps!");
//            }
//            return list;
//        }
//    }
//}
