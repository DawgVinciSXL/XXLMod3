using RapidGUI;
using RootMotion.Dynamics;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class DebugUI
    {
       static float test = 1f;
        public static float test2 = 200f;
        static float test3 = 100f;
        public static bool showMenu;
        public static Rect rect = new Rect(20f, Screen.currentResolution.height - 370.5f, 300, 100);
        static RaycastHit _hit;
        static RaycastHit _hit2;

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            GUILayout.BeginVertical("Box");
            GUILayout.Label($"<b>Current State: {XXLController.CurrentState.ToString()}</b>");
            GUILayout.EndVertical();
            if (GUILayout.Button("Map Muscle Positions"))
            {
                foreach (Muscle muscle in PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles)
                {
                    muscle.props.mapPosition = true;
                }
            }
            if (GUILayout.Button("Unmap Muscle Positions"))
            {
                foreach (Muscle muscle in PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles)
                {
                    muscle.props.mapPosition = false;
                }
            }

            test2 = RGUI.SliderFloat(DebugUI.test2, 0f, 1, 1, "Muscle Spring");
            test3 = RGUI.SliderFloat(DebugUI.test3, 0f, 1, 1, "Muscle Damper");
            if (GUILayout.Button("Override Values"))
            {
                PlayerController.Instance.respawn.puppetMaster.muscleWeight = DebugUI.test;
                PlayerController.Instance.respawn.puppetMaster.muscleSpring = DebugUI.test2;
                PlayerController.Instance.respawn.puppetMaster.muscleDamper = DebugUI.test3;
            }
            //foreach(Collider col in PlayerController.Instance.boardController.boardColliders)
            //{
            //    GUILayout.BeginHorizontal();
            //    if (GUILayout.Button("Size"))
            //    {
            //        col.gameObject.transform.localScale = new Vector3(test, test2, test3);
            //    }
            //    GUILayout.Label(col.name);
            //    GUILayout.Label(col.gameObject.transform.localScale.ToString());
            //    GUILayout.EndHorizontal();
            //}
            GUILayout.Label("X: " + UIController.Instance.TabMenuRect.size.x.ToString() + " Y: " + UIController.Instance.TabMenuRect.size.y.ToString());
            //GUILayout.Label("X: " + UIController.Instance.MainMenuRect.size.x.ToString() + " Y: " + UIController.Instance.MainMenuRect.size.y.ToString());
            GUILayout.Label("X: " + PresetUI.Rect.size.x.ToString() + " Y: " + PresetUI.Rect.size.y.ToString());
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>DEBUGGING</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                showMenu = !showMenu;
            }
            GUILayout.EndHorizontal();
        }
    }
}