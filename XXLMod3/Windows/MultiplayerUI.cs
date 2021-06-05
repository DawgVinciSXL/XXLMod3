using GameManagement;
using Photon.Pun;
using RapidGUI;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public static class MultiplayerUI
    {
        public static bool showMenu;
        public static Rect Rect = new Rect(670f, Screen.currentResolution.height / 2 - 370.5f, 100f, 200f);

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            Main.settings.PopUpMessagesTimer = RGUI.SliderFloat(Main.settings.PopUpMessagesTimer, 0.3f, 3f, 3f, "Message Duration");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            if (PhotonNetwork.InRoom)
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label($"<b>Current Lobby: {PhotonNetwork.CurrentRoom.Name}</b>");
                GUILayout.Label($"<b>Ping: {PhotonNetwork.GetPing()}</b>");
                if (GUILayout.Button("<b>SPECTATE</b>", GUILayout.Height(21f)))
                {
                    GameStateMachine.Instance.RequestTransitionTo(typeof(SpectatorState), false, null);
                    showMenu = !showMenu;
                }
                GUILayout.EndHorizontal();
                foreach (KeyValuePair<int, NetworkPlayerController> player in MultiplayerManager.Instance.networkPlayers)
                {
                    GUILayout.BeginHorizontal();
                    if (!player.Value.photonView.AmOwner)
                    {
                        GUILayout.Label("<b>" + player.Value.NickName + "</b>", GUILayout.Width(200f));
                        if (GUILayout.Button("<b>Teleport</b>"))
                        {
                            PlayerController.Instance.respawn.SetSpawnPos(player.Value.GetSkateboard().position, Quaternion.identity);
                            PlayerController.Instance.respawn.DoRespawn();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("<b>Join Lobby for More Options</b>");
            }
            GUILayout.EndVertical();
        }

        public static void Open()
        {
            showMenu = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void Close()
        {
            showMenu = false;
            if (StanceUI.showMenu || UIController.Instance.showMainMenu || PresetUI.showMenu)
            {
                return;
            }
            Cursor.visible = false;
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>MULTIPLAYER UTILS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}