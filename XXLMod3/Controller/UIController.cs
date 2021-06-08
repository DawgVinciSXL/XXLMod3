using System;
using System.IO;
using System.Linq;
using UnityEngine;
using XXLMod3.Core;
using XXLMod3.Windows;

namespace XXLMod3.Controller
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; private set; }
        public Rect MainMenuRect;
        public Rect TabMenuRect;
        public bool setupGUI;
        public bool showMainMenu;

        private GUIStyle windowStyle;
        private Texture2D windowTex;

        public MenuTab MenuTab = MenuTab.Off;

        private void Awake() => Instance = this;

        private void Start()
        {
            MainMenuRect = new Rect(20f, Screen.currentResolution.height / 2 - 370.5f, 100f, 200f);
            TabMenuRect = new Rect(200f, Screen.currentResolution.height / 2 - 370.5f, 100f, 200f);
        }

        private void Update()
        {
            if (Input.GetKeyDown(Main.settings.XXLHotkey.keyCode))
            {
                bool flag = !showMainMenu;
                if (flag)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }

            if (Input.GetKeyDown(Main.settings.MultiplayerHotkey.keyCode))
            {
                bool flag = !MultiplayerUI.showMenu;
                if (flag)
                {
                    MultiplayerUI.Open();
                }
                else
                {
                    MultiplayerUI.Close();
                }
            }

            if (Input.GetKeyDown(Main.settings.PresetsHotkey.keyCode))
            {
                bool flag = !PresetUI.showMenu;
                if (flag)
                {
                    PresetUI.Open();
                }
                else
                {
                    PresetUI.Close();
                }
            }

            if (Input.GetKeyDown(Main.settings.StanceHotkey.keyCode))
            {
                bool flag = !StanceUI.showMenu;
                if (flag)
                {
                    StanceUI.Open();
                }
                else
                {
                    StanceUI.Close();
                }
            }
        }

        private void Open()
        {
            showMainMenu = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Close()
        {
            showMainMenu = false;
            if (MultiplayerUI.showMenu || PresetUI.showMenu || StanceUI.showMenu)
            {
                return;
            }
            Cursor.visible = false;
        }

        private void SetupGUI()
        {
            windowTex = new Texture2D(1, 1);
            var brightness = 1f;
            var alpha = 0.9f;
            windowTex.SetPixels(new[] { new Color(brightness, brightness, brightness, alpha) });
            windowTex.Apply();

            windowStyle = new GUIStyle(GUI.skin.window)
            {
            };
            windowStyle.normal.background = windowTex;
        }

        private void OnGUI()
        {
            //if (!setupGUI)
            //{
            //    SetupGUI();
            //    setupGUI = true;
            //}
            if (showMainMenu)
            {
                GUI.backgroundColor = Main.settings.BGColor;
                MainMenuRect = GUILayout.Window(9000, MainMenuRect, MainMenu, "<b>XXLMOD3</b>");

                switch (MenuTab)
                {
                    case MenuTab.General:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position,new Vector2(444f, 697f)), GeneralSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Catch:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 246f)), CatchUI.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Flips:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 398f)), FlipSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Lateflips:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 294f)), LateFlipSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Grabs:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 266f)), GrabSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Fingerflips:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 50f)), FingerFlipSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Footplants:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 217f)), FootplantSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Grinds:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(495f, 695f)), GrindSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Manuals:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 398f)), ManualSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Primos:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 381f)), PrimoUI.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Misc:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 553f)), MiscSettings.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Bail:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 336f)), BailUI.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Other:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 659f)), OtherUI.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.Fixes:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(614f, 225f)), FixUI.Window, "<b>XXLMOD3</b>");
                        break;
                    case MenuTab.LegCustomizer:
                        TabMenuRect = GUI.Window(9001, new Rect(TabMenuRect.position, new Vector2(444f, 375f)), LegCustomizer.Window, "<b>XXLMOD3</b>");
                        break;
                }
            }

            if (Windows.DebugUI.showMenu)
            {
                GUI.backgroundColor = Main.settings.BGColor;
                Windows.DebugUI.rect = GUILayout.Window(9013, Windows.DebugUI.rect, Windows.DebugUI.Window, "<b>XXLMOD3</b>");
            }

            if (MultiplayerUI.showMenu)
            {
                GUI.backgroundColor = Main.settings.BGColor;
                MultiplayerUI.Rect = GUILayout.Window(9014, MultiplayerUI.Rect, MultiplayerUI.Window, "<b>XXLMOD3</b>");
            }

            if (PresetUI.showMenu)
            {
                GUI.backgroundColor = Main.settings.BGColor;
                PresetUI.Rect = GUILayout.Window(9015, PresetUI.Rect, PresetUI.Window, "<b>XXLMOD3</b>");
            }

            if (StanceUI.showMenu)
            {
                StanceController.IsInEditMode = true;
                GUI.backgroundColor = Main.settings.BGColor;
                StanceUI.Rect = GUILayout.Window(9016, StanceUI.Rect, StanceUI.Window, "<b>XXLMOD3</b>");
            }
            else
            {
                StanceController.IsInEditMode = false;
            }
        }

        private void MainMenu(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            if (GUILayout.Button($"<b>GENERAL</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.General) ? MenuTab.Off : MenuTab.General;
            }

            if (GUILayout.Button("<b>CATCH</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Catch) ? MenuTab.Off : MenuTab.Catch;
            }

            if (GUILayout.Button("<b>FLIPS</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Flips) ? MenuTab.Off : MenuTab.Flips;
            }

            if (GUILayout.Button("<b>LATEFLIPS</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Lateflips) ? MenuTab.Off : MenuTab.Lateflips;
            }

            if (GUILayout.Button("<b>GRABS</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Grabs) ? MenuTab.Off : MenuTab.Grabs;
            }

            if (GUILayout.Button("<b>FOOTPLANTS</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Footplants) ? MenuTab.Off : MenuTab.Footplants;
            }

            if (GUILayout.Button("<b>GRINDS</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Grinds) ? MenuTab.Off : MenuTab.Grinds;
            }

            if (GUILayout.Button("<b>MANUALS</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Manuals) ? MenuTab.Off : MenuTab.Manuals;
            }

            if (GUILayout.Button("<b>PRIMO</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Primos) ? MenuTab.Off : MenuTab.Primos;
            }

            if (GUILayout.Button("<b>MISC</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Misc) ? MenuTab.Off : MenuTab.Misc;
            }

            if (GUILayout.Button("<b>BAILS</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Bail) ? MenuTab.Off : MenuTab.Bail;
            }

            if (GUILayout.Button("<b>OTHER</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Other) ? MenuTab.Off : MenuTab.Other;
            }

            if (GUILayout.Button("<b>GAME FIXES</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.Fixes) ? MenuTab.Off : MenuTab.Fixes;
            }

            if (GUILayout.Button("<b>LEGCUSTOMIZER</b>", GUILayout.Height(21f)))
            {
                MenuTab = (MenuTab == MenuTab.LegCustomizer) ? MenuTab.Off : MenuTab.LegCustomizer;
            }

            if (GUILayout.Button($"<b>STANCE ({Main.settings.StanceHotkey.keyCode.ToString()})</b>", GUILayout.Height(21f)))
            {
                bool flag = !StanceUI.showMenu;
                if (flag)
                {
                    StanceUI.Open();
                }
                else
                {
                    StanceUI.Close();
                }
            }

            if (GUILayout.Button($"<b>MULTIPLAYER ({Main.settings.MultiplayerHotkey.keyCode.ToString()})</b>", GUILayout.Height(21f)))
            {
                bool flag = !MultiplayerUI.showMenu;
                if (flag)
                {
                    MultiplayerUI.Open();
                }
                else
                {
                    MultiplayerUI.Close();
                }
            }

            if (GUILayout.Button($"<b>PRESETS ({Main.settings.PresetsHotkey.keyCode.ToString()})</b>", GUILayout.Height(21f)))
            {
                bool flag = !PresetUI.showMenu;
                if (flag)
                {
                    PresetUI.Open();
                }
                else
                {
                    PresetUI.Close();
                }
            }

            if (GUILayout.Button("<b>DEBUG</b>", GUILayout.Height(21f)))
            {
                Windows.DebugUI.showMenu = !Windows.DebugUI.showMenu;
            }
        }

        private void OnApplicationQuit()
        {
            Main.settings.Save(Main.modEntry);
            StanceController.Instance.SaveFootPositionRotation();
        }
    }
}