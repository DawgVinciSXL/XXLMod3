using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class FlipSettings
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            Main.settings.FlipAnimationSpeed = RGUI.SliderFloat(Main.settings.FlipAnimationSpeed, 0.7f, 1.3f, 1f, "Animation Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.FlipBoardOffset = RGUI.SliderFloat(Main.settings.FlipBoardOffset, -0.2f, 0.3f, 0f, "Board Offset");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.FlipSpeed = RGUI.SliderFloat(Main.settings.FlipSpeed, 0f, 2.5f, 1f, "Flip Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.FlipStrength = RGUI.SliderFloat(Main.settings.FlipStrength, 0.60f, 1f, 1f, "Flip Strength");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.LaidbackFlips, "Laidback Flips"))
            {
                Main.settings.LaidbackFlips = !Main.settings.LaidbackFlips;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PopKickLeft = RGUI.SliderFloat(Main.settings.PopKickLeft, -5f, 5f, -0.7f, "Pop Kick Left");
            Main.settings.PopKickRight = RGUI.SliderFloat(Main.settings.PopKickRight, -5f, 5f, 0.7f, "Pop Kick Right");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.PressureFlips, "Pressure Flips"))
            {
                Main.settings.PressureFlips = !Main.settings.PressureFlips;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.ScoopSpeed = RGUI.SliderFloat(Main.settings.ScoopSpeed, 0f, 2.5f, 1f, "Scoop Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.VerticalFlips, "Vertical Flips"))
            {
                Main.settings.VerticalFlips = !Main.settings.VerticalFlips;
            }
            Main.settings.Verticality = RGUI.SliderFloat(Main.settings.Verticality, -2f, 2f, 1f, "Verticality");
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>FLIP SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}