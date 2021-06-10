using GameManagement;
using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class PowerslideUI
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            Main.settings.PowerslideCrouchAmount = RGUI.SliderFloat(Main.settings.PowerslideCrouchAmount, 0.3f, 1f, 0.94196f, "Crouch Amount");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PowerslideFriction = RGUI.SliderFloat(Main.settings.PowerslideFriction, 0f, 1f, 1f, "Friction");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.PowerslidePopOut, "Pop Out (L3/R3)"))
            {
                Main.settings.PowerslidePopOut = !Main.settings.PowerslidePopOut;
            }
            Main.settings.PowerslidePopForce = RGUI.SliderFloat(Main.settings.PowerslidePopForce, 0.3f, 5f, 2.5f, "Pop Force");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            GUILayout.Box("<b>XLAnimationModifier By Tave</b>", GUILayout.Height(21f));
            if (RGUI.Button(Main.settings.RevertAnimationCancel, "Revert Animation Cancel"))
            {
                Main.settings.RevertAnimationCancel = !Main.settings.RevertAnimationCancel;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.RevertSpeed = RGUI.SliderFloat(Main.settings.RevertSpeed, 0.3f, 2f, 1f, "Revert Speed");
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>POWERSLIDE SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}