using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class ManualSettings
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.ManualBabyPop, "Baby Pop (L3/R3)"))
            {
                Main.settings.ManualBabyPop = !Main.settings.ManualBabyPop;
            }
            Main.settings.ManualBabyPopForce = RGUI.SliderFloat(Main.settings.ManualBabyPopForce, 0.3f, 3f,1.5f, "Baby Pop Force");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.ManualBraking, "Braking"))
            {
                Main.settings.ManualBraking = !Main.settings.ManualBraking;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.ManualCrouchMode = RGUI.Field(Main.settings.ManualCrouchMode, "Crouch Mode");
            Main.settings.ManualCrouchAmount = RGUI.SliderFloat(Main.settings.ManualCrouchAmount, 0.3f, 1f, 1.1f, "Crouch Amount");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.ManualMaxAngle = RGUI.SliderFloat(Main.settings.ManualMaxAngle, 10f, 20f, 10f, "Max Angle");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.ManualDelay, "Manual Delay"))
            {
                Main.settings.ManualDelay = !Main.settings.ManualDelay;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.ManualPopDelay, "Pop Delay"))
            {
                Main.settings.ManualPopDelay = !Main.settings.ManualPopDelay;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.ManualPopForce = RGUI.SliderFloat(Main.settings.ManualPopForce, 0f, 5f, 2.5f, "Pop Force");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.ManualOneFootMode = RGUI.Field(Main.settings.ManualOneFootMode, "One Foot Manual");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.ManualRevertSensitivity = RGUI.SliderFloat(Main.settings.ManualRevertSensitivity, 0.05f, 0.3f, 0.3f, "Revert Sensitivity");
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>MANUAL SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}