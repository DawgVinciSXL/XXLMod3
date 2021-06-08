using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using XXLMod3.Controller;

namespace XXLMod3
{
    public static class Main
    {
        public static Harmony harmonyInstance;
        public static UnityModManager.ModEntry.ModLogger logger;
        public static Settings settings;
        public static UnityModManager.ModEntry modEntry;
        public static StanceController stanceController;
        public static UIController uiController;
        public static XXLController xxlController;

        public static bool enabled;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(OnSaveGUI);
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
            Main.modEntry = modEntry;
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Box("<b>Background Color</b>", GUILayout.Height(21f));
            settings.BGColor.r = RapidGUI.RGUI.SliderFloat(settings.BGColor.r, 0f, 1f, 0f, "Red");
            settings.BGColor.g = RapidGUI.RGUI.SliderFloat(settings.BGColor.g, 0f, 1f, 0f, "Green");
            settings.BGColor.b = RapidGUI.RGUI.SliderFloat(settings.BGColor.b, 0f, 1f, 0f, "Blue");
            GUILayout.Box("<b>Hotkeys</b>", GUILayout.Height(21f));
            settings.Draw(modEntry);
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if(value == enabled)
            {
                return true;
            }

            enabled = value;

            if (enabled)
            {
                harmonyInstance = new Harmony(modEntry.Info.Id);
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                AssetBundleHelper.LoadUIBundle();
                uiController = new GameObject().AddComponent<UIController>();
                xxlController = new GameObject().AddComponent<XXLController>();
                stanceController = new GameObject().AddComponent<StanceController>();
                PresetHelper.CreateFolder();
                PresetHelper.GetPresets();
                UnityEngine.Object.DontDestroyOnLoad(stanceController);
                UnityEngine.Object.DontDestroyOnLoad(uiController);
                UnityEngine.Object.DontDestroyOnLoad(xxlController);
            }
            else
            {
                harmonyInstance.UnpatchAll(harmonyInstance.Id);
                UnityEngine.Object.Destroy(stanceController);
                UnityEngine.Object.Destroy(uiController);
                UnityEngine.Object.Destroy(xxlController);
            }
            return true;
        }
    }
}