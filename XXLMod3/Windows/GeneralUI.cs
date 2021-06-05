using ModIO.UI;
using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Windows
{
    public static class GeneralSettings
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);
        private static string StatsPresetName = "Enter Name...";

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            Main.settings.Gravity = RGUI.SliderFloat(Main.settings.Gravity, -30f, 0f, -9.807f, "Gravity");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.IndividualPopForce, "Individual Pop Force"))
            {
                Main.settings.IndividualPopForce = !Main.settings.IndividualPopForce;
            }
            Main.settings.DefaultPopForce = RGUI.SliderFloat(Main.settings.DefaultPopForce, 0.5f, 5f, 3f, "Default Force");
            Main.settings.NolliePopForce = RGUI.SliderFloat(Main.settings.NolliePopForce, 0.5f, 5f, 3f, "Nollie Force");
            Main.settings.SwitchPopForce = RGUI.SliderFloat(Main.settings.SwitchPopForce, 0.5f, 5f, 3f, "Switch Force");
            Main.settings.FakiePopForce = RGUI.SliderFloat(Main.settings.FakiePopForce, 0.5f, 5f, 3f, "Fakie Force");
            Main.settings.HighPopForceMult = RGUI.SliderFloat(Main.settings.HighPopForceMult, 0.1f, 2f, 0.5f, "High Pop Mult");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.AdvancedPop = RGUI.Field(Main.settings.AdvancedPop, "Advanced Pop");
            Main.settings.ForwardPopForce = RGUI.SliderFloat(Main.settings.ForwardPopForce, 0.5f, 500f, 100f, "Forward Force");
            Main.settings.SidewayPopForce = RGUI.SliderFloat(Main.settings.SidewayPopForce, 0.5f, 500f, 100f, "Sideway Force");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.BabyPop, "Baby Pop"))
            {
                Main.settings.BabyPop = !Main.settings.BabyPop;
            }
            Main.settings.BabyPopForceMult = RGUI.SliderFloat(Main.settings.BabyPopForceMult, 0.25f, 0.9f, 0.6f, "Baby Pop Force");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.DecoupledMode = RGUI.Field(Main.settings.DecoupledMode, "Decoupled Board");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.InAirTurnSpeed = RGUI.SliderFloat(Main.settings.InAirTurnSpeed, 0.5f, 5f, 1f, "In Air Turn Speed");
            Main.settings.MaxAngularVelocity = RGUI.SliderFloat(Main.settings.MaxAngularVelocity, 3.5f, 14f, 7f, "Max Angular Velocity");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.PopDelay, "Pop Delay"))
            {
                Main.settings.PopDelay = !Main.settings.PopDelay;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PumpForceMult = RGUI.SliderFloat(Main.settings.PumpForceMult, 0.1f, 5f, 1f, "Pump Force Mult");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.FirstPushForce = RGUI.SliderFloat(Main.settings.FirstPushForce, 0.1f, 2f, 0.5f, "First Push Mult");
            Main.settings.PushForce = RGUI.SliderFloat(Main.settings.PushForce, 0.1f, 16f, 8f, "Push Force");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.RevertSpeed = RGUI.SliderFloat(Main.settings.RevertSpeed, 0.3f, 2f, 1f, "Revert Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.TopSpeed = RGUI.SliderFloat(Main.settings.TopSpeed, 0.1f, 16f, 8f, "Top Speed");
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal("Box");
            StatsPresetName = GUILayout.TextField(StatsPresetName, GUILayout.Height(21f));
            if (GUILayout.Button("<b>Save Stats</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                StanceController.Instance.SaveFootPositionRotation();
                SaveStats.Save<Settings>(Main.settings, Main.modEntry, UIController.Instance.StatsPresetPath + StatsPresetName + ".xml");
                UIController.Instance.GetPresetsFromFolder();
                UISounds.Instance.PlayOneShotSelectMajor();
                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"Preset: {StatsPresetName} successfully saved!", 2f);
            }
            if(GUILayout.Button("<b>Load Stats</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                if (PresetUI.showMenu)
                {
                    PresetUI.PresetTab = PresetTab.Stats;
                    return;
                }
                PresetUI.PresetTab = PresetTab.Stats;
                PresetUI.Open();
            }
            GUILayout.EndHorizontal();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>GENERAL SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}