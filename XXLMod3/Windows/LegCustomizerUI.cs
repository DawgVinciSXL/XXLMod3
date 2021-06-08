using ModIO.UI;
using Newtonsoft.Json;
using RapidGUI;
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

        public enum LegTab
        {
            Flip,
            Steeze
        }

        public enum FlipLegs
        {
            Ollie,
            Nollie,
            Switch,
            Fakie
        }

        public enum SteezeLegs
        {
            Ollie,
            Nollie,
            Switch,
            Fakie
        }

        public static FlipLegs FlipTab = FlipLegs.Ollie;
        public static LegTab LegTabs = LegTab.Steeze;
        public static SteezeLegs SteezeTab = SteezeLegs.Ollie;
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
                    FlipLegsTab();
                    break;
                case LegTab.Steeze:
                    SteezeLegsTab();
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
                SaveSettings(PresetHelper.LegPresetsPath + PresetName, legs);
                PresetHelper.GetPresets();
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

        public static void FlipLegsTab()
        {
            GUILayout.BeginVertical("Box");
            FlipTab = RGUI.Field(FlipTab, "Select PopType");
            DrawLegSettings(GetCurrentFlipLegs());
            GUILayout.EndVertical();
        }

        public static void SteezeLegsTab()
        {
            GUILayout.BeginVertical("Box");
            SteezeTab = RGUI.Field(SteezeTab, "Select PopType");
            DrawLegSettings(GetCurrentSteezeLegs());
            GUILayout.EndVertical();
        }

        public static void DrawLegSettings(CustomLegSettings legSettings)
        {
            if (RGUI.Button(legSettings.Active, "Steeze Legs"))
            {
                legSettings.Active = !legSettings.Active;
            }
            DoLegSettings(legSettings);
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
                    return Main.settings.OllieFlipLegs;
                case LegTab.Steeze:
                    return Main.settings.OllieSteezeLegs;
                default:
                    return Main.settings.OllieSteezeLegs;
            }
        }

        public static CustomLegSettings GetCurrentSteezeLegs()
        {
            switch (SteezeTab)
            {
                case SteezeLegs.Ollie:
                    return Main.settings.OllieSteezeLegs;
                case SteezeLegs.Nollie:
                    return Main.settings.NollieSteezeLegs;
                case SteezeLegs.Switch:
                    return Main.settings.SwitchSteezeLegs;
                case SteezeLegs.Fakie:
                    return Main.settings.FakieSteezeLegs;
                default:
                    return Main.settings.DefaultSteezeLegs;
            }
        }

        public static CustomLegSettings GetCurrentFlipLegs()
        {
            switch (FlipTab)
            {
                case FlipLegs.Ollie:
                    return Main.settings.OllieFlipLegs;
                case FlipLegs.Nollie:
                    return Main.settings.NollieFlipLegs;
                case FlipLegs.Switch:
                    return Main.settings.SwitchFlipLegs;
                case FlipLegs.Fakie:
                    return Main.settings.FakieFlipLegs;
                default:
                    return Main.settings.DefaultFlipLegs;
            }
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>LEG CUSTOMIZER</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}