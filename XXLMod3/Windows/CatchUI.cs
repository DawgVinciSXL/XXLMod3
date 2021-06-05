using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class CatchUI
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            Main.settings.CatchBoardRotateSpeed = RGUI.SliderFloat(Main.settings.CatchBoardRotateSpeed, 15f, 120f, 57.29578f, "Board Rotate Speed (Both Feet Down)");
            if (RGUI.Button(Main.settings.CatchCorrection, "Catch Correction"))
            {
                Main.settings.CatchCorrection = !Main.settings.CatchCorrection;
            }
            Main.settings.CatchCorrectionSpeed = RGUI.SliderFloat(Main.settings.CatchCorrectionSpeed, 0f, 60f, 0f, "Catch Correction Speed");
            Main.settings.CatchMode = RGUI.Field(Main.settings.CatchMode, "Catch Mode");
            if(RGUI.Button(Main.settings.RealisticDrops, "Realistic Drops"))
            {
                Main.settings.RealisticDrops = !Main.settings.RealisticDrops;
            }
            //if (RGUI.Button(Main.settings.CatchSmooth, "Smooth Catch"))
            //{
            //    Main.settings.CatchSmooth = !Main.settings.CatchSmooth;
            //}
            if (RGUI.Button(Main.settings.StompCatch, "Stomp Catch (L3&R3)"))
            {
                Main.settings.StompCatch = !Main.settings.StompCatch;
            }
            Main.settings.StompCatchForce = RGUI.SliderFloat(Main.settings.StompCatchForce, 0.1f, 5f, 1f, "Stomp Force");
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>CATCH SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}