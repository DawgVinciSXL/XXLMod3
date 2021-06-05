using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class PrimoUI
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.Primos, "Primos"))
            {
                Main.settings.Primos = !Main.settings.Primos;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PrimoFlipAnimationSpeed = RGUI.SliderFloat(Main.settings.PrimoFlipAnimationSpeed, 0.7f, 1.3f, 1f, "Animation Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PrimoBoardOffset = RGUI.SliderFloat(Main.settings.PrimoBoardOffset, -0.2f, 0.1f, 0f, "Board Offset");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PrimoFlipSpeed = RGUI.SliderFloat(Main.settings.PrimoFlipSpeed, 0f, 2.5f, 1f, "Flip Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PrimoFlipStrength = RGUI.SliderFloat(Main.settings.PrimoFlipStrength, 0.30f, 1f, 1f, "Flip Strength");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PrimoFriction = RGUI.SliderFloat(Main.settings.PrimoFriction, 0.1f, 1f, 0.5f, "Friction");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.PrimoLaidbackFlips, "Laidback Flips"))
            {
                Main.settings.PrimoLaidbackFlips = !Main.settings.PrimoLaidbackFlips;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PrimoPopForce = RGUI.SliderFloat(Main.settings.PrimoPopForce, 0f, 5f, 3f, "Pop Force");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.PrimoPressureFlips, "Pressure Flips"))
            {
                Main.settings.PrimoPressureFlips = !Main.settings.PrimoPressureFlips;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PrimoScoopSpeed = RGUI.SliderFloat(Main.settings.PrimoScoopSpeed, 0f, 2.5f, 1f, "Scoop Speed");
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>PRIMO SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}