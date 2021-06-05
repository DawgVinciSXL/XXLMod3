using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class BailUI
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.AutoRespawn, "Auto Respawn"))
            {
                Main.settings.AutoRespawn = !Main.settings.AutoRespawn;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.BetterBails, "Better Bails"))
            {
                Main.settings.BetterBails = !Main.settings.BetterBails;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.BailControls, "Bail Controls"))
            {
                Main.settings.BailControls = !Main.settings.BailControls;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.BailDownForce = RGUI.SliderFloat(Main.settings.BailDownForce, 0.5f, 100f, 2f, "Down Force (LB)");
            Main.settings.BailUpForce = RGUI.SliderFloat(Main.settings.BailUpForce, 0.5f, 100f, 5f, "Up Force (RB)");
            Main.settings.BailArmForce = RGUI.SliderFloat(Main.settings.BailArmForce, 0.2f, 10, 1f, "Arm Force (LT/RT)");
            Main.settings.BailLegForce = RGUI.SliderFloat(Main.settings.BailLegForce, 0.2f, 10, 1f, "Leg Force (L3/R3)");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.BailLookAtPlayer, "Camera Focus On Player"))
            {
                Main.settings.BailLookAtPlayer = !Main.settings.BailLookAtPlayer;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.BailRespawnAt, "Respawn At Bail"))
            {
                Main.settings.BailRespawnAt = !Main.settings.BailRespawnAt;
            }
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>BAIL SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}