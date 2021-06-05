using GameManagement;
using RapidGUI;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class OtherUI
    {
        public static bool showMenu;
        public static Rect rect = new Rect(1, 1, 100, 100);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.UseEscToQuit, "Esc to Quit Game"))
            {
                Main.settings.UseEscToQuit = !Main.settings.UseEscToQuit;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (GUILayout.Button(XXLController.Instance.BoardVisible ? "<b>Hide Board</b>" : "<b>Show Board</b>", GUILayout.Height(21f)))
            {
                if (XXLController.Instance.BoardVisible)
                {
                    XXLController.Instance.HideBoard();
                    return;
                }
                XXLController.Instance.ShowBoard();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (GUILayout.Button(XXLController.Instance.SkaterVisible ? "<b>Hide Skater</b>" : "<b>Show Skater</b>", GUILayout.Height(21f)))
            {
                if (XXLController.Instance.SkaterVisible)
                {
                    XXLController.Instance.HidePlayer();
                    return;
                }
                XXLController.Instance.ShowPlayer();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (GUILayout.Button("<b>Mute Environment</b>", GUILayout.Height(21f)))
            {
                XXLController.Instance.MuteEnvironment();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            Main.settings.ReplayPlaybackSpeed = RGUI.SliderFloat(Main.settings.ReplayPlaybackSpeed, 0.1f, 1f, 1f, "Replay Playback Speed");
            if (RGUI.Button(Main.settings.StartReplayAtLastFrame, "Start Replay at Last Frame"))
            {
                Main.settings.StartReplayAtLastFrame = !Main.settings.StartReplayAtLastFrame;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.SlowMotionBails, "Slow Motion Bails"))
            {
                Main.settings.SlowMotionBails = !Main.settings.SlowMotionBails;
            }
            Main.settings.SlowMotionBailSpeed = RGUI.SliderFloat(Main.settings.SlowMotionBailSpeed, 0.1f, 1f, 0.5f, "Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if(RGUI.Button(Main.settings.SlowMotionFlips, "Slow Motion Flips"))
            {
                Main.settings.SlowMotionFlips = !Main.settings.SlowMotionFlips;
            }
            Main.settings.SlowMotionFlipSpeed = RGUI.SliderFloat(Main.settings.SlowMotionFlipSpeed, 0.1f, 1f, 0.5f, "Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.SlowMotionGrabs, "Slow Motion Grabs"))
            {
                Main.settings.SlowMotionGrabs = !Main.settings.SlowMotionGrabs;
            }
            Main.settings.SlowMotionGrabSpeed = RGUI.SliderFloat(Main.settings.SlowMotionGrabSpeed, 0.1f, 1f, 0.5f, "Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.SlowMotionGrinds, "Slow Motion Grinds"))
            {
                Main.settings.SlowMotionGrinds = !Main.settings.SlowMotionGrinds;
            }
            Main.settings.SlowMotionGrindSpeed = RGUI.SliderFloat(Main.settings.SlowMotionGrindSpeed, 0.1f, 1f, 0.5f, "Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.SlowMotionManuals, "Slow Motion Manuals"))
            {
                Main.settings.SlowMotionManuals = !Main.settings.SlowMotionManuals;
            }
            Main.settings.SlowMotionManualSpeed = RGUI.SliderFloat(Main.settings.SlowMotionManualSpeed, 0.1f, 1f, 0.5f, "Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.SlowMotionPrimos, "Slow Motion Primos"))
            {
                Main.settings.SlowMotionPrimos = !Main.settings.SlowMotionPrimos;
            }
            Main.settings.SlowMotionPrimoSpeed = RGUI.SliderFloat(Main.settings.SlowMotionPrimoSpeed, 0.1f, 1f, 0.5f, "Speed");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.SlowMotionReverts, "Slow Motion Reverts"))
            {
                Main.settings.SlowMotionReverts = !Main.settings.SlowMotionReverts;
            }
            Main.settings.SlowMotionRevertSpeed = RGUI.SliderFloat(Main.settings.SlowMotionRevertSpeed, 0.1f, 1f, 0.5f, "Speed");
            GUILayout.EndVertical();
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>OTHER STUFF</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                UIController.Instance.MenuTab = Core.MenuTab.Off;
            }
            GUILayout.EndHorizontal();
        }
    }
}