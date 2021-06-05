using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class FingerFlipSettings
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            Main.settings.FingerFlipBoardOffset = RGUI.SliderFloat(Main.settings.FingerFlipBoardOffset, -0.2f, 0.3f, 0f, "Board Offset");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.FingerFlipSpeed = RGUI.SliderFloat(Main.settings.FingerFlipSpeed, 0f, 2.5f, 1f, "Flip Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.FingerScoopSpeed = RGUI.SliderFloat(Main.settings.FingerScoopSpeed, 0f, 2.5f, 1f, "Scoop Speed");
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>FINGERFLIP SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}