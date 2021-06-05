using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class LateFlipSettings
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.Lateflips, "Lateflips (EXPERIMENTAL)"))
            {
                Main.settings.Lateflips = !Main.settings.Lateflips;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.LateFlipAnimationSpeed = RGUI.SliderFloat(Main.settings.LateFlipAnimationSpeed, 0.7f, 1.3f, 1f, "Animation Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.LateFlipBoardOffset = RGUI.SliderFloat(Main.settings.LateFlipBoardOffset, -0.2f, 0.3f, 0f, "Board Offset");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.LateFlipSpeed = RGUI.SliderFloat(Main.settings.LateFlipSpeed, 0f, 2.5f, 1f, "Flip Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.LateFlipStrength = RGUI.SliderFloat(Main.settings.LateFlipStrength, 0.30f, 1f, 1f, "Flip Strength");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.LateFlipLaidbackFlips, "Laidback Flips"))
            {
                Main.settings.LateFlipLaidbackFlips = !Main.settings.LateFlipLaidbackFlips;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.LateScoopSpeed = RGUI.SliderFloat(Main.settings.LateScoopSpeed, 0f, 2.5f, 1f, "Scoop Speed");
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>LATEFLIP SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}