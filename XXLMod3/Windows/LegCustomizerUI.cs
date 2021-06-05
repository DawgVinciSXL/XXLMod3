using ModIO.UI;
using Newtonsoft.Json;
using RapidGUI;
using System;
using System.IO;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Windows
{
    public static class LegCustomizer
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static string PresetPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\XXLMod3\\LegPresets\\";

        public enum LegTab
        {
            Flip,
            Steeze
        }

        public static LegTab LegTabs = LegTab.Steeze;
        private static string PresetName = "Enter Name...";

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            LegTabs = RGUI.Field(LegTabs, "Selected Legs");
            GUILayout.EndVertical();

            switch (LegTabs)
            {
                case LegTab.Flip:
                    FlipLegs();
                    break;
                case LegTab.Steeze:
                    SteezeLegs();
                    break;
            }
        }

        private static void DoLegSettings(BaseLegSettings legs)
        {
            GUILayout.Box("<b>Left Leg</b>", GUILayout.Height(21f));
            legs.LeftLegZ = RGUI.SliderFloat(legs.LeftLegZ, -0.3f, 0.3f, 0f, "Front/Back");
            legs.LeftLegY = RGUI.SliderFloat(legs.LeftLegY, -0.3f, 0.3f, 0f, "Up/Down");
            legs.LeftLegX = RGUI.SliderFloat(legs.LeftLegX, -0.3f, 0.3f, 0f, "Left/Right");

            GUILayout.Box("<b>" + "Right Leg" + "</b>", GUILayout.Height(21f));
            legs.RightLegZ = RGUI.SliderFloat(legs.RightLegZ, -0.3f, 0.3f, 0f, "Front/Back");
            legs.RightLegY = RGUI.SliderFloat(legs.RightLegY, -0.3f, 0.3f, 0f, "Up/Down");
            legs.RightLegX = RGUI.SliderFloat(legs.RightLegX, -0.3f, 0.3f, 0f, "Left/Right");

            GUILayout.BeginHorizontal("Box");
            PresetName = GUILayout.TextField(PresetName, GUILayout.Height(21f));
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("<b>Save Legs</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                SaveSettings(PresetPath + PresetName, legs);
                UIController.Instance.GetPresetsFromFolder();
                UISounds.Instance.PlayOneShotSelectMajor();
                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"Legs: {PresetName} successfully saved!", 2f);
            }
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("<b>Load Legs</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                if (PresetUI.showMenu)
                {
                    PresetUI.PresetTab = PresetTab.Legs;
                    return;
                }
                PresetUI.PresetTab = PresetTab.Legs;
                PresetUI.Open();
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("<b>Reset Legs</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                legs.LeftLegY = 0f;
                legs.LeftLegX = 0f;
                legs.LeftLegZ = 0f;

                legs.RightLegY = 0f;
                legs.RightLegX = 0f;
                legs.RightLegZ = 0f;
            }
            GUILayout.EndHorizontal();
        } 

        public static void FlipLegs()
        {
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.FlipLegs.Active, "Flip Legs"))
            {
                Main.settings.FlipLegs.Active = !Main.settings.FlipLegs.Active;
            }

            DoLegSettings(Main.settings.FlipLegs);
            GUILayout.EndVertical();
        }

        public static void SteezeLegs()
        {
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.SteezeLegs.Active, "Steeze Legs"))
            {
                Main.settings.SteezeLegs.Active = !Main.settings.SteezeLegs.Active;
            }
            DoLegSettings(Main.settings.SteezeLegs);
            GUILayout.EndVertical();
        }

        private static void SaveSettings(string fileName, BaseLegSettings Settings)
        {
            BaseLegSettings data = new BaseLegSettings(Settings.Active, Settings.LeftLegX, Settings.LeftLegY, Settings.LeftLegZ, Settings.RightLegX, Settings.RightLegY, Settings.RightLegZ);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            File.WriteAllText(fileName + ".json", json);
        }

        public static void LoadSettings(string fileName)
        {
            string json;
            if (File.Exists(fileName))
            {
                json = File.ReadAllText(fileName);
            }
            else
            {
                return;
            }

            BaseLegSettings _loadedSettings = JsonConvert.DeserializeObject<BaseLegSettings>(json);

            BaseLegSettings Settings = GetCurrentSelectedLegs();

            Settings.Active = _loadedSettings.Active;
            Settings.LeftLegY = _loadedSettings.LeftLegY;
            Settings.LeftLegX = _loadedSettings.LeftLegX;
            Settings.LeftLegZ = _loadedSettings.LeftLegZ;

            Settings.RightLegY = _loadedSettings.RightLegY;
            Settings.RightLegX = _loadedSettings.RightLegX;
            Settings.RightLegZ = _loadedSettings.RightLegZ;
        }

        private static BaseLegSettings GetCurrentSelectedLegs()
        {
            switch (LegTabs)
            {
                case LegTab.Flip:
                    return Main.settings.FlipLegs;
                case LegTab.Steeze:
                    return Main.settings.SteezeLegs;
                default:
                    return Main.settings.SteezeLegs;
            }
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>LEG CUSTOMIZER</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}