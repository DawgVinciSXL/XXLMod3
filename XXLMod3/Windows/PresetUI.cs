using ModIO.UI;
using RapidGUI;
using System.IO;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Windows
{
    public class PresetUI
    {
        public static bool showMenu;
        public static Rect Rect = new Rect(1150f, Screen.currentResolution.height / 2 - 370.5f, 100f, 300f);

        public static Core.PresetTab PresetTab = Core.PresetTab.Stats;

        static Vector2 grindScrollPos = Vector2.zero;
        static Vector2 legScrollPos = Vector2.zero;
        static Vector2 stanceScrollPos = Vector2.zero;
        static Vector2 statsScrollPos = Vector2.zero;

        static string StatsPresetName = "Enter Name...";

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            PresetTab = RGUI.Field(PresetTab, "");

            switch (PresetTab)
            {
                case Core.PresetTab.Stats:
                    DrawStatPresets();
                    break;
                case Core.PresetTab.Grinds:
                    DrawGrindPresets();
                    break;
                case Core.PresetTab.Legs:
                    DrawLegPresets();
                    break;
                case Core.PresetTab.Stances:
                    DrawStancePresets();
                    break;
            }
        }

        private static void DrawStatPresets()
        {
            bool flag2 = PresetHelper.StatsPresets == null;
            if (flag2)
            {
                PresetHelper.GetPresets();
            }

            if (PresetHelper.StatsPresets.Length >= 1)
            {
                GUILayout.BeginVertical("Box");
                float num3 = (float)Mathf.Min(208, PresetHelper.StatsPresets.Length * (200 + 2 * 2) + 2 * 2);
                statsScrollPos = GUILayout.BeginScrollView(statsScrollPos, new GUILayoutOption[]
                {
                    GUILayout.Height(num3),
                    GUILayout.ExpandHeight(true)
                });
                GUILayout.BeginVertical();
                for (int i = 0; i < PresetHelper.StatsPresets.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    string presetName = Path.GetFileNameWithoutExtension(PresetHelper.StatsPresets[i]);
                    bool flag7 = GUILayout.Button("<b>" + presetName + "</b>", GUILayout.Height(21));
                    if (flag7)
                    {
                        Main.settings = Core.SaveStats.Load<Settings>(Main.modEntry, PresetHelper.StatsPresetsPath + presetName + ".xml");
                        StanceController.Instance.Initialize();
                    }
                    bool flag8 = GUILayout.Button("<b>X</b>", GUILayout.Height(21f), GUILayout.Width(30f));
                    if (flag8)
                    {
                        DeletePreset(PresetHelper.StatsPresetsPath + presetName + ".xml");
                        PresetHelper.GetPresets();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Box("<b>No Presets Found!</b>", GUILayout.Height(21f));
            }

            GUILayout.BeginHorizontal("Box");
            StatsPresetName = GUILayout.TextField(StatsPresetName, GUILayout.Height(21f));
            if (GUILayout.Button("<b>Save Settings</b>", GUILayout.Height(21f), GUILayout.Width(120f)))
            {
                StanceController.Instance.SaveFootPositionRotation();
                Core.SaveStats.Save<Settings>(Main.settings, Main.modEntry, PresetHelper.StatsPresetsPath + StatsPresetName+ ".xml");
                PresetHelper.GetPresets();
                UISounds.Instance.PlayOneShotSelectMajor();
                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"Preset: {StatsPresetName} successfully saved!", 2f);
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawGrindPresets()
        {
            bool flag2 = PresetHelper.GrindPresets == null;
            if (flag2)
            {
                PresetHelper.GetPresets();
            }

            if (PresetHelper.GrindPresets.Length >= 1)
            {
                GUILayout.BeginVertical("Box");
                float num3 = (float)Mathf.Min(208, PresetHelper.GrindPresets.Length * (200 + 2 * 2) + 2 * 2);
                grindScrollPos = GUILayout.BeginScrollView(grindScrollPos, new GUILayoutOption[]
                {
                    GUILayout.Height(num3),
                    GUILayout.ExpandHeight(true)
                });
                GUILayout.BeginVertical();
                for (int i = 0; i < PresetHelper.GrindPresets.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    string presetName = Path.GetFileNameWithoutExtension(PresetHelper.GrindPresets[i]);
                    bool flag7 = GUILayout.Button("<b>" + presetName + "</b>", GUILayout.Height(21));
                    if (flag7)
                    {
                        GrindSettings.LoadSettings(PresetHelper.GrindPresetsPath + presetName + ".json");
                    }
                    bool flag8 = GUILayout.Button("<b>X</b>", GUILayout.Height(21f), GUILayout.Width(30f));
                    if (flag8)
                    {
                        DeletePreset(PresetHelper.GrindPresetsPath + presetName + ".json");
                        PresetHelper.GetPresets();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Box("<b>No Presets Found!</b>", GUILayout.Height(21f));
            }
        }

        private static void DrawLegPresets()
        {
            bool flag2 = PresetHelper.LegPresets == null;
            if (flag2)
            {
                PresetHelper.GetPresets();
            }

            if (PresetHelper.LegPresets.Length >= 1)
            {
                GUILayout.BeginVertical("Box");
                float num3 = (float)Mathf.Min(208, PresetHelper.LegPresets.Length * (200 + 2 * 2) + 2 * 2);
                legScrollPos = GUILayout.BeginScrollView(legScrollPos, new GUILayoutOption[]
                {
                    GUILayout.Height(num3),
                    GUILayout.ExpandHeight(true)
                });
                GUILayout.BeginVertical();
                for (int i = 0; i < PresetHelper.LegPresets.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    string presetName = Path.GetFileNameWithoutExtension(PresetHelper.LegPresets[i]);
                    bool flag7 = GUILayout.Button("<b>" + presetName + "</b>", GUILayout.Height(21));
                    if (flag7)
                    {
                        LegCustomizer.LoadSettings(PresetHelper.LegPresetsPath + presetName + ".json");
                    }
                    bool flag8 = GUILayout.Button("<b>X</b>", GUILayout.Height(21f), GUILayout.Width(30f));
                    if (flag8)
                    {
                        DeletePreset(PresetHelper.LegPresetsPath + presetName + ".json");
                        PresetHelper.GetPresets();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Box("<b>No Presets Found!</b>", GUILayout.Height(21f));
            }
        }

        private static void DrawStancePresets()
        {
            bool flag2 = StanceController.Instance.StancePresets == null;
            if (flag2)
            {
                StanceController.Instance.GetPresetsFromFolder();
            }

            if (StanceController.Instance.StancePresets.Length >= 1)
            {
                GUILayout.BeginVertical("Box");
                float num3 = (float)Mathf.Min(208, StanceController.Instance.StancePresets.Length * (200 + 2 * 2) + 2 * 2);
                stanceScrollPos = GUILayout.BeginScrollView(stanceScrollPos, new GUILayoutOption[]
                {
                    GUILayout.Height(num3),
                    GUILayout.ExpandHeight(true)
                });
                GUILayout.BeginVertical();
                for (int i = 0; i < StanceController.Instance.StancePresets.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    string presetName = Path.GetFileNameWithoutExtension(StanceController.Instance.StancePresets[i]);
                    bool flag7 = GUILayout.Button("<b>" + presetName + "</b>", GUILayout.Height(21));
                    if (flag7)
                    {
                        StanceUI.LoadStanceSettings(StanceController.Instance.presetPath + presetName + ".json");
                    }
                    bool flag8 = GUILayout.Button("<b>X</b>", GUILayout.Height(21f), GUILayout.Width(30f));
                    if (flag8)
                    {
                        DeletePreset(StanceController.Instance.presetPath + presetName + ".json");
                        StanceController.Instance.GetPresetsFromFolder();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Box("<b>No Presets Found!</b>", GUILayout.Height(21f));
            }
        }

        private static void DeletePreset(string fileName)
        {
            File.Delete(fileName);
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>PRESETS</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                Close();
            }
            GUILayout.EndHorizontal();
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
            if (MultiplayerUI.showMenu || UIController.Instance.showMainMenu || StanceUI.showMenu)
            {
                return;
            }
            Cursor.visible = false;
        }
    }
}