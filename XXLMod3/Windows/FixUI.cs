using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class FixUI
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.GrabFix, "Grab Fix"))
            {
                Main.settings.GrabFix = !Main.settings.GrabFix;
            }
            GUILayout.Box("<b>Avoids sinking into the ground when approaching last second grabs.</b>", GUILayout.Height(21f));
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.ManualFix, "Low Pop/Manual Fix"))
            {
                Main.settings.ManualFix = !Main.settings.ManualFix;
            }
            GUILayout.Box("<b>Fix for high gravity/low pops into manual or onto high obstacles.</b>", GUILayout.Height(21f));
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.ShuvLegFix, "Shuv Leg Fix"))
            {
                Main.settings.ShuvLegFix = !Main.settings.ShuvLegFix;
            }
            GUILayout.Box("<b>Fixes the leg going wild on fast shuv-its. Most noticeable when goofy. Decreases scoop a bit.</b>", GUILayout.Height(21f));
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>GAME FIXES</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}