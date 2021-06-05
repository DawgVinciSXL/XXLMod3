using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Windows
{
    public static class GrabSettings
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.Grabs, "Grabs"))
            {
                Main.settings.Grabs = !Main.settings.Grabs;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.GrabBoardOffset = RGUI.SliderFloat(Main.settings.GrabBoardOffset, -0.15f, 0f, 0f, "Board Offset");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.BodyflipMode = RGUI.Field(Main.settings.BodyflipMode, "Bodyflips");
            Main.settings.BodyflipSpeed = RGUI.SliderFloat(Main.settings.BodyflipSpeed, 30f, 120f, 60f, "Bodyflip Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.GrabDelay, "Grab Delay"))
            {
                Main.settings.GrabDelay = !Main.settings.GrabDelay;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.OneFootGrabMode = RGUI.Field(Main.settings.OneFootGrabMode, "One Foot Grabs");
            if(GUILayout.Button("<b>Setup Feet</b>", GUILayout.Height(21f)))
            {
                UIController.Instance.MenuTab = MenuTab.Off;
                if (!StanceUI.showMenu)
                {
                    StanceUI.showMenu = true;
                    StanceUI.StanceTab = StanceTab.GrabsOnButton;
                }
            }
            GUILayout.EndVertical();           
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>GRAB SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}