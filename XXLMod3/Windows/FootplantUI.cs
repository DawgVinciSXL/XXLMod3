using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Windows
{
    public static class FootplantSettings
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.Footplants, "Footplants"))
            {
                Main.settings.Footplants = !Main.settings.Footplants;
                if (Main.settings.Footplants)
                {
                    if(Main.settings.OneFootGrabMode == OneFootGrabMode.Off)
                    {
                        Main.settings.OneFootGrabMode = OneFootGrabMode.Buttons;
                    }
                }
            }
            Main.settings.FootplantBoardOffset = RGUI.SliderFloat(Main.settings.FootplantBoardOffset, -0.2f, 0f, 0f, "Board Offset");
            Main.settings.FootplantForwardForce = RGUI.SliderFloat(Main.settings.FootplantForwardForce, -5f, 5f, 2f, "Forward Force");
            Main.settings.FootplantJumpForce = RGUI.SliderFloat(Main.settings.FootplantJumpForce, 0f, 5f, 2f, "Jump Force");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.OneFootGrabMode = RGUI.Field(Main.settings.OneFootGrabMode, "One Foot Grabs");
            if (GUILayout.Button("<b>Setup Feet</b>", GUILayout.Height(21f)))
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
            GUILayout.Label("<b>FOOTPLANT SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}