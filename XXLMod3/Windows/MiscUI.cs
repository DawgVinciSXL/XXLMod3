using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class MiscSettings
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.AutoBanklean, "Auto Banklean"))
            {
                Main.settings.AutoBanklean = !Main.settings.AutoBanklean;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.AutoBoardRotationSnap, "Auto Rotation Snap"))
            {
                Main.settings.AutoBoardRotationSnap = !Main.settings.AutoBoardRotationSnap;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.AutoPump, "Auto Pump"))
            {
                Main.settings.AutoPump = !Main.settings.AutoPump;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.AutoRevert, "Auto Revert"))
            {
                Main.settings.AutoRevert = !Main.settings.AutoRevert;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            GUILayout.Box("<b>XLAnimationModifier By Tave</b>", GUILayout.Height(21f));
            if (RGUI.Button(Main.settings.FixedRevertAnimation, "Cancel Revert Animation"))
            {
                Main.settings.FixedRevertAnimation = !Main.settings.FixedRevertAnimation;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.FlipsAfterPop, "Flips After Pop"))
            {
                Main.settings.FlipsAfterPop = !Main.settings.FlipsAfterPop;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.HippieOllie, "Hippie Ollies (B)"))
            {
                Main.settings.HippieOllie = !Main.settings.HippieOllie;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.LockTurningWhileWindUp, "Lock Turning While Windup"))
            {
                Main.settings.LockTurningWhileWindUp = !Main.settings.LockTurningWhileWindUp;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PowerslideCrouchAmount = RGUI.SliderFloat(Main.settings.PowerslideCrouchAmount, 0.3f, 1f, 0.94196f, "Powerslide Crouch Amount");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.PushCrouchAmount = RGUI.SliderFloat(Main.settings.PushCrouchAmount, 0.3f, 1f, 1.04196f, "Push Crouch Amount");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.RandomImpactAnimations, "Random Impact Animation"))
            {
                Main.settings.RandomImpactAnimations = !Main.settings.RandomImpactAnimations;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.ReducedStickTurning, "Reduce Stick Turning"))
            {
                Main.settings.ReducedStickTurning = !Main.settings.ReducedStickTurning;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.ShuvMidFlip, "Shuv Mid-Flip"))
            {
                Main.settings.ShuvMidFlip = !Main.settings.ShuvMidFlip;
            }
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>MISC SETTINGS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}