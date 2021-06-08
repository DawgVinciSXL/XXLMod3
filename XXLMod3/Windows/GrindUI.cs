using RapidGUI;
using System;
using System.IO;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;
using Newtonsoft.Json;
using ModIO.UI;

namespace XXLMod3.Windows
{
    public class GrindSettings
    {
        private static string presetName = "Enter name...";
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.Grinds, "Grinds"))
            {
                Main.settings.Grinds = !Main.settings.Grinds;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.AdvancedGrinding, "Advanced Grinds"))
            {
                Main.settings.AdvancedGrinding = !Main.settings.AdvancedGrinding;
            }
            if (!Main.settings.AdvancedGrinding)
            {
                GUILayout.Box("");
                GetSettings(Main.settings._generalGrindSettings);
            }
            else
            {
                Main.settings.AdvancedGrinds = RGUI.Field(Main.settings.AdvancedGrinds, "Selected Grind");
                switch (Main.settings.AdvancedGrinds)
                {
                    case AdvancedGrinds.Bluntslide:
                        GetSettings(Main.settings._bluntSlideSettings);
                        break;
                    case AdvancedGrinds.Boardslide:
                        GetSettings(Main.settings._boardSlideSettings);
                        break;
                    case AdvancedGrinds.Crook:
                        GetSettings(Main.settings._crookSettings);
                        break;
                    case AdvancedGrinds.Feeble:
                        GetSettings(Main.settings._feebleSettings);
                        break;
                    case AdvancedGrinds.FiftyFifty:
                        GetSettings(Main.settings._fiftyFiftySettings);
                        break;
                    case AdvancedGrinds.FiveO:
                        GetSettings(Main.settings._fiveOSettings);
                        break;
                    case AdvancedGrinds.Lipslide:
                        GetSettings(Main.settings._lipslideSettings);
                        break;
                    case AdvancedGrinds.Losi:
                        GetSettings(Main.settings._losiSettings);
                        break;
                    case AdvancedGrinds.Noseblunt:
                        GetSettings(Main.settings._nosebluntSettings);
                        break;
                    case AdvancedGrinds.Nosegrind:
                        GetSettings(Main.settings._nosegrindSettings);
                        break;
                    case AdvancedGrinds.Noseslide:
                        GetSettings(Main.settings._noseslideSettings);
                        break;
                    case AdvancedGrinds.Overcrook:
                        GetSettings(Main.settings._overcrookSettings);
                        break;
                    case AdvancedGrinds.Salad:
                        GetSettings(Main.settings._saladSettings);
                        break;
                    case AdvancedGrinds.Smith:
                        GetSettings(Main.settings._smithSettings);
                        break;
                    case AdvancedGrinds.Suski:
                        GetSettings(Main.settings._suskiSettings);
                        break;
                    case AdvancedGrinds.Tailslide:
                        GetSettings(Main.settings._tailslideSettings);
                        break;
                    case AdvancedGrinds.Willy:
                        GetSettings(Main.settings._willySettings);
                        break;
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.GrindBumpDelay, "Bump to Grind Delay"))
            {
                Main.settings.GrindBumpDelay = !Main.settings.GrindBumpDelay;
            }
            if (RGUI.Button(Main.settings.InfiniteStallTime, "Infinite Stall Time"))
            {
                Main.settings.InfiniteStallTime = !Main.settings.InfiniteStallTime;
            }
            Main.settings.InstantStallMode = RGUI.Field(Main.settings.InstantStallMode, "Instant Stall");
            Main.settings.GrindOneFootMode = RGUI.Field(Main.settings.GrindOneFootMode, "One Foot Grinds");
            Main.settings.PopOutDirection = RGUI.Field(Main.settings.PopOutDirection, "Pop Out Direction");
            Main.settings.GrindTurnSpeed = RGUI.SliderFloat(Main.settings.GrindTurnSpeed, 0.5f, 5f, 1f, "Turn Speed");

            GUILayout.EndVertical();
        }

        private static void GetSettings(BaseGrindSettings GrindSettings)
        {
            GUILayout.BeginHorizontal();
            GrindSettings.AnimationSpeed = RGUI.SliderFloat(GrindSettings.AnimationSpeed, 0.01f, 2f, 1f, "Animation Speed");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.AnimationSpeed = GrindSettings.AnimationSpeed;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (RGUI.Button(GrindSettings.BumpOut, "Bump Out"))
            {
                GrindSettings.BumpOut = !GrindSettings.BumpOut;
            }
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.BumpOut = GrindSettings.BumpOut;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.BumpOutPopForce = RGUI.SliderFloat(GrindSettings.BumpOutPopForce, 0f, 5f, 1.25f, "Bump Out Pop Force");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.BumpOutPopForce = GrindSettings.BumpOutPopForce;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.BumpOutSidewayForce = RGUI.SliderFloat(GrindSettings.BumpOutSidewayForce, 0f, 5f, 1.2f, "Bump Out Sideway Force");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.BumpOutSidewayForce = GrindSettings.BumpOutSidewayForce;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.CrouchMode = RGUI.Field(GrindSettings.CrouchMode, "Crouch Mode");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.CrouchMode = GrindSettings.CrouchMode;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.CrouchAmount = RGUI.SliderFloat(GrindSettings.CrouchAmount, 0.3f, 1f, 0.95f, "Crouch Amount");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.CrouchAmount = GrindSettings.CrouchAmount;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.Friction = RGUI.SliderFloat(GrindSettings.Friction, 0.1f, 1f, 0.25f, "Friction");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.Friction = GrindSettings.Friction;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.MaxAngleModifier = RGUI.SliderFloat(GrindSettings.MaxAngleModifier, 0f, 35f, 15f, "Max Angle");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.MaxAngleModifier = GrindSettings.MaxAngleModifier;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.PopForce = RGUI.SliderFloat(GrindSettings.PopForce, 0f, 5f, 2f, "Pop Force");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.PopForce = GrindSettings.PopForce;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (RGUI.Button(GrindSettings.PopOut, "Pop Out"))
            {
                GrindSettings.PopOut = !GrindSettings.PopOut;
            }
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.PopOut = GrindSettings.PopOut;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.PopThreshold = RGUI.SliderFloat(GrindSettings.PopThreshold, 15f, 30f, 15f, "Pop Threshold");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.PopThreshold = GrindSettings.PopThreshold;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.SidewayPopForce = RGUI.SliderFloat(GrindSettings.SidewayPopForce, 0f, 5f, 0.75f, "Sideway Pop Force");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.SidewayPopForce = GrindSettings.SidewayPopForce;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (RGUI.Button(GrindSettings.Stabilizer, "Stabilizer"))
            {
                GrindSettings.Stabilizer = !GrindSettings.Stabilizer;
            }
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.Stabilizer = GrindSettings.Stabilizer;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GrindSettings.TorqueModifier = RGUI.SliderFloat(GrindSettings.TorqueModifier, 22f, 88f, 44f, "Torque Force");
            if (GUILayout.Button("<b>Link</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                foreach (BaseGrindSettings settings in XXLController.Instance.GrindSettingObjects)
                {
                    settings.TorqueModifier = GrindSettings.TorqueModifier;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            presetName = GUILayout.TextField(presetName, GUILayout.Height(21f));
            if (GUILayout.Button("<b>Save Settings</b>", GUILayout.Height(21f), GUILayout.Width(120f)))
            {
                SaveSettings(PresetHelper.GrindPresetsPath + presetName, GrindSettings);
                PresetHelper.GetPresets();
                UISounds.Instance.PlayOneShotSelectMajor();
                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"Preset: {presetName} successfully saved!", 2f);

            }
            if (GUILayout.Button("<b>Load Settings</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                if (PresetUI.showMenu)
                {
                    PresetUI.PresetTab = PresetTab.Grinds;
                    return;
                }
                PresetUI.PresetTab = PresetTab.Grinds;
                PresetUI.Open();
            }
            GUILayout.EndHorizontal();
        }

        private static void SaveSettings(string fileName, BaseGrindSettings Settings)
        {
            CustomGrindSettings data = new CustomGrindSettings(Settings.AnimationSpeed, Settings.BumpOut, Settings.BumpOutPopForce, Settings.BumpOutSidewayForce, Settings.CrouchMode, Settings.CrouchAmount, Settings.Friction, Settings.MaxAngleModifier,Settings.PopForce,Settings.PopOut,Settings.PopThreshold,Settings.SidewayPopForce,Settings.Stabilizer,Settings.TorqueModifier);
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

            CustomGrindSettings _loadedSettings = JsonConvert.DeserializeObject<CustomGrindSettings>(json);

            BaseGrindSettings Settings = GetCurrentSelectedGrind();

            Settings.AnimationSpeed = _loadedSettings.AnimationSpeed;
            Settings.BumpOut = _loadedSettings.BumpOut;
            Settings.BumpOutPopForce = _loadedSettings.BumpOutPopForce;
            Settings.BumpOutSidewayForce = _loadedSettings.BumpOutSidewayForce;
            Settings.CrouchMode = _loadedSettings.CrouchMode;
            Settings.CrouchAmount = _loadedSettings.CrouchAmount;
            Settings.Friction = _loadedSettings.Friction;
            Settings.MaxAngleModifier = _loadedSettings.MaxAngleModifier;
            Settings.PopForce = _loadedSettings.PopForce;
            Settings.PopOut = _loadedSettings.PopOut;
            Settings.PopThreshold = _loadedSettings.PopThreshold;
            Settings.SidewayPopForce = _loadedSettings.SidewayPopForce;
            Settings.Stabilizer = _loadedSettings.Stabilizer;
            Settings.TorqueModifier = _loadedSettings.TorqueModifier;
        }

        private static BaseGrindSettings GetCurrentSelectedGrind()
        {
            if (!Main.settings.AdvancedGrinding)
            {
                return Main.settings._generalGrindSettings;
            }

            switch (Main.settings.AdvancedGrinds)
            {
                case AdvancedGrinds.Bluntslide:
                    return Main.settings._bluntSlideSettings;
                case AdvancedGrinds.Boardslide:
                    return Main.settings._boardSlideSettings;
                case AdvancedGrinds.Crook:
                    return Main.settings._crookSettings;
                case AdvancedGrinds.Feeble:
                    return Main.settings._feebleSettings;
                case AdvancedGrinds.FiftyFifty:
                    return Main.settings._fiftyFiftySettings;
                case AdvancedGrinds.FiveO:
                    return Main.settings._fiveOSettings;
                case AdvancedGrinds.Lipslide:
                    return Main.settings._lipslideSettings;
                case AdvancedGrinds.Losi:
                    return Main.settings._losiSettings;
                case AdvancedGrinds.Noseblunt:
                    return Main.settings._nosebluntSettings;
                case AdvancedGrinds.Nosegrind:
                    return Main.settings._nosegrindSettings;
                case AdvancedGrinds.Noseslide:
                    return Main.settings._noseslideSettings;
                case AdvancedGrinds.Overcrook:
                    return Main.settings._overcrookSettings;
                case AdvancedGrinds.Salad:
                    return Main.settings._saladSettings;
                case AdvancedGrinds.Smith:
                    return Main.settings._smithSettings;
                case AdvancedGrinds.Suski:
                    return Main.settings._suskiSettings;
                case AdvancedGrinds.Tailslide:
                    return Main.settings._tailslideSettings;
                case AdvancedGrinds.Willy:
                    return Main.settings._willySettings;
                default:
                    return Main.settings._generalGrindSettings;
            }
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>GRIND SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}