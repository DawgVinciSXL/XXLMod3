using ModIO.UI;
using Newtonsoft.Json;
using RapidGUI;
using System;
using System.IO;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Windows
{
    public static class StanceUI
    {
        private static string presetName = "Enter name...";
        public static string presetPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\XXLMod3\\StancePresets\\";

        public static bool showMenu;
        public static Rect Rect = new Rect(200f, Screen.currentResolution.height / 2 - 370.5f, 100f, 200f);

        public static GrabOffBoardStanceTab GrabOffBoardStanceTab = GrabOffBoardStanceTab.Simple;
        public static GrabStanceTab GrabStanceTab = GrabStanceTab.Indy;
        public static GrindStanceTab GrindStanceTab = GrindStanceTab.BSBluntslide;
        public static StanceTab StanceTab = StanceTab.Riding;

        public static void Window(int windowID)
        {
            GUI.backgroundColor = Color.black;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            Title();

            GUILayout.BeginVertical("Box");
            StanceTab = RGUI.Field(StanceTab, "Selected Stance");
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal("Box");
            Main.settings.PositionSensitivity = RGUI.SliderFloat(Main.settings.PositionSensitivity, 0.1f, 1f, 0.1f, "<b>Position Placement Sensitivity</b>");
            Main.settings.RotationSensitivity = RGUI.SliderFloat(Main.settings.RotationSensitivity, 0.1f, 1f, 0.1f, "<b>Rotation Adjustment Sensitivity</b>");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            if(StanceTab == StanceTab.Catch)
            {
                GUILayout.BeginHorizontal("Box");
                GUI.backgroundColor = Color.white;
                Main.settings.UseRandomLanding = GUILayout.Toggle(Main.settings.UseRandomLanding, "Random Landing", GUILayout.Height(21f));
                GUI.backgroundColor = Color.black;
                GUILayout.EndHorizontal();
            }

            if(StanceTab == StanceTab.GrabsOnButton)
            {
                GUILayout.BeginHorizontal("Box");
                Main.settings.OneFootGrabMode = RGUI.Field(Main.settings.OneFootGrabMode, "One Foot Grabs");
                GUI.backgroundColor = Color.black;
                GUILayout.EndHorizontal();
            }

            if(StanceTab == StanceTab.OnButton)
            {
                GUILayout.BeginHorizontal("Box");
                GUI.backgroundColor = Color.white;
                Main.settings.UseSpecialInGrindState = GUILayout.Toggle(Main.settings.UseSpecialInGrindState, "Active while Grinding", GUILayout.Height(21f));
                Main.settings.UseSpecialInReleaseState = GUILayout.Toggle(Main.settings.UseSpecialInReleaseState, "Active while Catching", GUILayout.Height(21f));
                Main.settings.UseSpecialInManualState = GUILayout.Toggle(Main.settings.UseSpecialInManualState, "Active while Manualling", GUILayout.Height(21f));
                Main.settings.UseSpecialInPrimoState = GUILayout.Toggle(Main.settings.UseSpecialInPrimoState, "Active while Primo", GUILayout.Height(21f));
                GUI.backgroundColor = Color.black;
                GUILayout.EndHorizontal();
            }

            switch (StanceTab)
            {
                case StanceTab.Riding:
                    DoStanceSettings(Main.settings.RidingStanceSettings);
                    break;
                case StanceTab.RidingSwitch:
                    DoStanceSettings(Main.settings.RidingSwitchStanceSettings);
                    break;
                case StanceTab.Setup:
                    DoStanceSettings(Main.settings.SetupDefaultStanceSettings);
                    break;
                case StanceTab.SetupNollie:
                    DoStanceSettings(Main.settings.SetupNollieStanceSettings);
                    break;
                case StanceTab.SetupSwitch:
                    DoStanceSettings(Main.settings.SetupSwitchStanceSettings);
                    break;
                case StanceTab.SetupFakie:
                    DoStanceSettings(Main.settings.SetupFakieStanceSettings);
                    break;
                case StanceTab.Primo:
                    DoStanceSettings(Main.settings.PrimoStanceSettings);
                    break;
                case StanceTab.PrimoSetup:
                    DoStanceSettings(Main.settings.PrimoSetupDefaultStanceSettings);
                    break;
                case StanceTab.PrimoSetupNollie:
                    DoStanceSettings(Main.settings.PrimoSetupNollieStanceSettings);
                    break;
                case StanceTab.InAir:
                    DoStanceSettings(Main.settings.InAirStanceSettings);
                    break;
                case StanceTab.Catch:
                    DoStanceSettings(Main.settings.ReleaseStanceSettings);
                    break;
                case StanceTab.Pushing:
                    DoStanceSettings(Main.settings.PushDefaultStanceSettings);
                    break;
                case StanceTab.PushingMongo:
                    DoStanceSettings(Main.settings.PushMongoStanceSettings);
                    break;
                case StanceTab.PushingSwich:
                    DoStanceSettings(Main.settings.PushSwitchStanceSettings);
                    break;
                case StanceTab.PushingSwitchMongo:
                    DoStanceSettings(Main.settings.PushSwitchMongoStanceSettings);
                    break;
                case StanceTab.Powerslide:
                    DoStanceSettings(Main.settings.PowerslideStanceSettings);
                    break;
                case StanceTab.OnButton:
                    DoStanceSettings(Main.settings.OnButtonStanceSettings);
                    break;
                case StanceTab.Grinding:
                    DoGrindStanceSettings();
                    break;
                case StanceTab.Grabs:
                    DoGrabStanceSettings();
                    break;
                case StanceTab.GrabsOnButton:
                    DoGrabOffBoardStanceSettings();
                    break;
                case StanceTab.Manual:
                    DoStanceSettings(Main.settings.ManualStanceSettings);
                    break;
                case StanceTab.ManualSwitch:
                    DoStanceSettings(Main.settings.ManualSwitchStanceSettings);
                    break;
                case StanceTab.NoseManual:
                    DoStanceSettings(Main.settings.NoseManualStanceSettings);
                    break;
                case StanceTab.NoseManualSwitch:
                    DoStanceSettings(Main.settings.NoseManualSwitchStanceSettings);
                    break;
            }

            DrawFootMovement();

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
            StanceController.Instance.SaveFootPositionRotation();
            showMenu = false;
            if (MultiplayerUI.showMenu || UIController.Instance.showMainMenu || PresetUI.showMenu)
            {
                return;
            }
            Cursor.visible = false;
        }

        private static void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>STANCES</b>", GUILayout.Height(21f));
            if (GUILayout.Button("<b>X</b>", GUILayout.Height(19f), GUILayout.Width(32f)))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private static void DoStanceSettings(BaseStanceSettings Stance)
        {
            if(RGUI.Button(Stance.Active, "Enabled", GUILayout.Height(21f)))
            {
                Stance.Active = !Stance.Active;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Box("<b>Left Foot</b>", GUILayout.Height(21f));
            GUILayout.Box("<b>Right Foot</b>", GUILayout.Height(21f));
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Box("<b>Transition Speed Settings</b>", GUILayout.Height(21f));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("Box");
            Stance.lfPosSpeed = RGUI.SliderFloat(Stance.lfPosSpeed, 0.1f, 5f, 1f, "Position Speed");
            Stance.lfRotSpeed = RGUI.SliderFloat(Stance.lfRotSpeed, 50f, 300f, 100f, "Rotation Speed");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.BeginVertical("Box");
            Stance.rfPosSpeed = RGUI.SliderFloat(Stance.rfPosSpeed, 0.1f, 5f, 1f, "Position Speed");
            Stance.rfRotSpeed = RGUI.SliderFloat(Stance.rfRotSpeed, 50f, 300f, 100f, "Rotation Speed");
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void DoGrindStanceSettings()
        {
            GrindStanceTab = RGUI.Field(GrindStanceTab, "<b>Selected Grind</b>");
            DoStanceSettings(GetCurrentStance());
        }

        private static void DoGrabStanceSettings()
        {
            GrabStanceTab = RGUI.Field(GrabStanceTab, "<b>Selected Grab</b>");
            DoStanceSettings(GetCurrentStance());
        }

        private static void DoGrabOffBoardStanceSettings()
        {
            GrabOffBoardStanceTab = RGUI.Field(GrabOffBoardStanceTab, "<b>Selected Grab</b>", GUILayout.Height(21f));
            if(RGUI.Button(Main.settings.UseSimpleOnButtonGrabs, "Use Simple On Button Grabs", GUILayout.Height(21f)))
            {
                Main.settings.UseSimpleOnButtonGrabs = !Main.settings.UseSimpleOnButtonGrabs;
                if (Main.settings.UseSimpleOnButtonGrabs)
                {
                    GrabOffBoardStanceTab = GrabOffBoardStanceTab.Simple;
                }
            }
            DoStanceSettings(GetCurrentStance());
        }

        private static void SaveStanceSettings(string fileName, BaseStanceSettings _stanceSettings)
        {

            SaveStance data = new SaveStance(_stanceSettings.Active, _stanceSettings.lfPosSpeed,_stanceSettings.lfRotSpeed,_stanceSettings.rfPosSpeed,_stanceSettings.rfRotSpeed,_stanceSettings.lfPos,_stanceSettings.rfPos,_stanceSettings.lfRot,_stanceSettings.rfRot,_stanceSettings.ltRot,_stanceSettings.rtRot);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            File.WriteAllText(fileName + ".json", json);
        }

        public static void LoadStanceSettings(string fileName)
        {
            string json;

            if (File.Exists(fileName))
            {
                json = File.ReadAllText(fileName);
            }
            else
            {
                return;
            }

            SaveStance data = JsonConvert.DeserializeObject<SaveStance>(json);

            BaseStanceSettings _stanceSettings = GetCurrentStance();

            _stanceSettings.Active = data.Active;

            _stanceSettings.lfPosSpeed = data.lfPosSpeed;
            _stanceSettings.lfRotSpeed = data.lfRotSpeed;
            _stanceSettings.rfPosSpeed = data.rfPosSpeed;
            _stanceSettings.rfRotSpeed = data.rfRotSpeed;

            StanceController.Instance.ActiveLeftFootTarget.transform.localPosition = data.LeftFootPosition;
            StanceController.Instance.ActiveRightFootTarget.transform.localPosition = data.RightFootPosition;

            StanceController.Instance.ActiveLeftFootRotTarget.transform.localRotation = data.LeftFootRotation;
            StanceController.Instance.ActiveRightFootRotTarget.transform.localRotation = data.RightFootRotation;

            StanceController.Instance.ActiveLeftToeRotTarget.transform.localRotation = data.LeftToeRotation;
            StanceController.Instance.ActiveRightToeRotTarget.transform.localRotation = data.RightToeRotation;

            StanceController.Instance.SaveFootPositionRotation();
        }

        private static void DrawFootMovement()
        {
            GUILayout.Box("<b>Position Placement</b>", GUILayout.Height(21f));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Left</b>"))
            {
                StanceController.Instance.ActiveLeftFootTarget.transform.Translate(Main.settings.PositionSensitivity * -0.01f, 0, 0);
            }
            if (GUILayout.RepeatButton("<b>Right</b>"))
            {
                StanceController.Instance.ActiveLeftFootTarget.transform.Translate(Main.settings.PositionSensitivity * 0.01f, 0, 0);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Up</b>"))
            {
                StanceController.Instance.ActiveLeftFootTarget.transform.Translate(0, Main.settings.PositionSensitivity * 0.01f, 0);
            }
            if (GUILayout.RepeatButton("<b>Down</b>"))
            {
                StanceController.Instance.ActiveLeftFootTarget.transform.Translate(0, Main.settings.PositionSensitivity * -0.01f, 0);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Fwd</b>"))
            {
                StanceController.Instance.ActiveLeftFootTarget.transform.Translate(0, 0, Main.settings.PositionSensitivity * 0.01f);
            }
            if (GUILayout.RepeatButton("<b>Bwd</b>"))
            {
                StanceController.Instance.ActiveLeftFootTarget.transform.Translate(0, 0, Main.settings.PositionSensitivity * -0.01f);

            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Left</b>"))
            {
                StanceController.Instance.ActiveRightFootTarget.transform.Translate(Main.settings.PositionSensitivity * -0.01f, 0, 0);
            }
            if (GUILayout.RepeatButton("<b>Right</b>"))
            {
                StanceController.Instance.ActiveRightFootTarget.transform.Translate(Main.settings.PositionSensitivity * 0.01f, 0, 0);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Up</b>"))
            {
                StanceController.Instance.ActiveRightFootTarget.transform.Translate(0, Main.settings.PositionSensitivity * 0.01f, 0);
            }
            if (GUILayout.RepeatButton("<b>Down</b>"))
            {
                StanceController.Instance.ActiveRightFootTarget.transform.Translate(0, Main.settings.PositionSensitivity * -0.01f, 0);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Fwd</b>"))
            {
                StanceController.Instance.ActiveRightFootTarget.transform.Translate(0, 0, Main.settings.PositionSensitivity * 0.01f);
            }
            if (GUILayout.RepeatButton("<b>Bwd</b>"))
            {
                StanceController.Instance.ActiveRightFootTarget.transform.Translate(0, 0, Main.settings.PositionSensitivity * -0.01f);

            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(2f);

            GUILayout.Box("<b>Rotation Adjustment</b>", GUILayout.Height(21f));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Yaw L</b>"))
            {
                StanceController.Instance.ActiveLeftFootRotTarget.transform.Rotate(Main.settings.RotationSensitivity * -1f, 0, 0, Space.Self);
            }
            if (GUILayout.RepeatButton("<b>Yaw R</b>"))
            {
                StanceController.Instance.ActiveLeftFootRotTarget.transform.Rotate(Main.settings.RotationSensitivity * 1f, 0, 0, Space.Self);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Roll L</b>"))
            {
                StanceController.Instance.ActiveLeftFootRotTarget.transform.Rotate(0, Main.settings.RotationSensitivity * 1f, 0, Space.Self);
            }
            if (GUILayout.RepeatButton("<b>Roll R</b>"))
            {
                StanceController.Instance.ActiveLeftFootRotTarget.transform.Rotate(0, Main.settings.RotationSensitivity * -1f, 0, Space.Self);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Pitch Fwd</b>"))
            {
                StanceController.Instance.ActiveLeftFootRotTarget.transform.Rotate(0, 0, Main.settings.RotationSensitivity * 1f, Space.Self);
            }
            if (GUILayout.RepeatButton("<b>Pitch Bwd</b>"))
            {
                StanceController.Instance.ActiveLeftFootRotTarget.transform.Rotate(0, 0, Main.settings.RotationSensitivity * -1f, Space.Self);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (StanceTab == StanceTab.Setup || StanceTab == StanceTab.SetupNollie || StanceTab == StanceTab.SetupSwitch || StanceTab == StanceTab.SetupFakie)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.RepeatButton("<b>Toes Down</b>"))
                {
                    StanceController.Instance.ActiveLeftToeRotTarget.transform.Rotate(0, 0, Main.settings.RotationSensitivity * 1f, Space.Self);
                }
                if (GUILayout.RepeatButton("<b>Toes Up</b>"))
                {
                    StanceController.Instance.ActiveLeftToeRotTarget.transform.Rotate(0, 0, Main.settings.RotationSensitivity * -1f, Space.Self);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Yaw L</b>"))
            {
                StanceController.Instance.ActiveRightFootRotTarget.transform.Rotate(Main.settings.RotationSensitivity * -1f, 0, 0, Space.Self);
            }
            if (GUILayout.RepeatButton("<b>Yaw R</b>"))
            {
                StanceController.Instance.ActiveRightFootRotTarget.transform.Rotate(Main.settings.RotationSensitivity * 1f, 0, 0, Space.Self);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Roll L</b>"))
            {
                StanceController.Instance.ActiveRightFootRotTarget.transform.Rotate(0, Main.settings.RotationSensitivity * 1f, 0, Space.Self);
            }
            if (GUILayout.RepeatButton("<b>Roll R</b>"))
            {
                StanceController.Instance.ActiveRightFootRotTarget.transform.Rotate(0, Main.settings.RotationSensitivity * -1f, 0, Space.Self);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.RepeatButton("<b>Pitch Fwd</b>"))
            {
                StanceController.Instance.ActiveRightFootRotTarget.transform.Rotate(0, 0, Main.settings.RotationSensitivity * 1f, Space.Self);
            }
            if (GUILayout.RepeatButton("<b>Pitch Bwd</b>"))
            {
                StanceController.Instance.ActiveRightFootRotTarget.transform.Rotate(0, 0, Main.settings.RotationSensitivity * -1f, Space.Self);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (StanceTab == StanceTab.Setup || StanceTab == StanceTab.SetupNollie || StanceTab == StanceTab.SetupSwitch || StanceTab == StanceTab.SetupFakie)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.RepeatButton("<b>Toes Down</b>"))
                {
                    StanceController.Instance.ActiveRightToeRotTarget.transform.Rotate(0, 0, Main.settings.RotationSensitivity * 1f, Space.Self);
                }
                if (GUILayout.RepeatButton("<b>Toes Up</b>"))
                {
                    StanceController.Instance.ActiveRightToeRotTarget.transform.Rotate(0, 0, Main.settings.RotationSensitivity * -1f, Space.Self);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (StanceTab != StanceTab.Setup && StanceTab != StanceTab.SetupNollie && StanceTab != StanceTab.SetupSwitch && StanceTab != StanceTab.SetupFakie)
            {
                GUILayout.Box("", GUILayout.Height(21f));
            }
            GUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("<b>Flip</b>", GUILayout.Height(21f)))
            {
                Vector3 tempLeftPos = StanceController.Instance.ActiveLeftFootTarget.transform.localPosition;
                Vector3 tempRightPos = StanceController.Instance.ActiveRightFootTarget.transform.localPosition;
                StanceController.Instance.ActiveLeftFootTarget.transform.localPosition = new Vector3(-tempRightPos.x,tempRightPos.y,tempRightPos.z);
                StanceController.Instance.ActiveRightFootTarget.transform.localPosition = new Vector3(-tempLeftPos.x, tempLeftPos.y, tempLeftPos.z);

                Quaternion tempLeftRot = StanceController.Instance.ActiveLeftFootRotTarget.transform.localRotation;
                Quaternion tempRightRot = StanceController.Instance.ActiveRightFootRotTarget.transform.localRotation;
                StanceController.Instance.ActiveLeftFootRotTarget.transform.localRotation = new Quaternion(-tempRightRot.x, -tempRightRot.y, tempRightRot.z, tempRightRot.w);
                StanceController.Instance.ActiveRightFootRotTarget.transform.localRotation = new Quaternion(-tempLeftRot.x, -tempLeftRot.y, tempLeftRot.z, tempLeftRot.w);

                Quaternion tempLeftToeRot = StanceController.Instance.ActiveLeftToeRotTarget.transform.localRotation;
                Quaternion tempRightToeRot = StanceController.Instance.ActiveRightToeRotTarget.transform.localRotation;
                StanceController.Instance.ActiveLeftToeRotTarget.transform.localRotation = new Quaternion(-tempRightToeRot.x, -tempRightToeRot.y, tempRightToeRot.z, tempRightToeRot.w);
                StanceController.Instance.ActiveRightToeRotTarget.transform.localRotation = new Quaternion(-tempLeftToeRot.x, -tempLeftToeRot.y, tempLeftToeRot.z, tempLeftToeRot.w);

                StanceController.Instance.SaveFootPositionRotation();
            }
            if(GUILayout.Button("<b>Reset Pos</b>", GUILayout.Height(21f)))
            {
                StanceController.Instance.ActiveLeftFootTarget.transform.localPosition = StanceController.Instance.DefaultStanceSettings.lfPos;
                StanceController.Instance.ActiveRightFootTarget.transform.localPosition = StanceController.Instance.DefaultStanceSettings.rfPos;
                StanceController.Instance.SaveFootPositionRotation();
            }
            if(GUILayout.Button("<b> Reset Rot</b>", GUILayout.Height(21f)))
            {
                StanceController.Instance.ActiveLeftFootRotTarget.transform.localRotation = StanceController.Instance.DefaultStanceSettings.lfRot;
                StanceController.Instance.ActiveRightFootRotTarget.transform.localRotation = StanceController.Instance.DefaultStanceSettings.rfRot;

                StanceController.Instance.ActiveLeftToeRotTarget.transform.localRotation = Quaternion.Euler(-4.65968424E-06f, 8.54689745E-07f, 262.965179f);
                StanceController.Instance.ActiveRightToeRotTarget.transform.localRotation = Quaternion.Euler(-4.65968424E-06f, 8.54689745E-07f, 262.965179f);
                StanceController.Instance.SaveFootPositionRotation();
            }
            if (GUILayout.Button("<b>Reset All</b>", GUILayout.Height(21f)))
            {
                BaseStanceSettings ActiveStance = GetCurrentStance();
                ActiveStance.lfPosSpeed = 1f;
                ActiveStance.lfRotSpeed = 100f;
                ActiveStance.rfPosSpeed = 1f;
                ActiveStance.rfRotSpeed = 100f;

                StanceController.Instance.ActiveLeftFootTarget.transform.localPosition = StanceController.Instance.DefaultStanceSettings.lfPos;
                StanceController.Instance.ActiveRightFootTarget.transform.localPosition = StanceController.Instance.DefaultStanceSettings.rfPos;

                StanceController.Instance.ActiveLeftFootRotTarget.transform.localRotation = StanceController.Instance.DefaultStanceSettings.lfRot;
                StanceController.Instance.ActiveRightFootRotTarget.transform.localRotation = StanceController.Instance.DefaultStanceSettings.rfRot;

                StanceController.Instance.ActiveLeftToeRotTarget.transform.localRotation = Quaternion.Euler(-4.65968424E-06f, 8.54689745E-07f, 262.965179f);
                StanceController.Instance.ActiveRightToeRotTarget.transform.localRotation = Quaternion.Euler(-4.65968424E-06f, 8.54689745E-07f, 262.965179f);
                StanceController.Instance.SaveFootPositionRotation();
            }
            presetName = GUILayout.TextField(presetName, GUILayout.Height(21f));
            if (GUILayout.Button("<b>Save Stance</b>", GUILayout.Height(21f), GUILayout.Width(120f)))
            {
                StanceController.Instance.SaveFootPositionRotation();
                SaveStanceSettings(presetPath + presetName, GetCurrentStance());
                StanceController.Instance.GetPresetsFromFolder();
                UISounds.Instance.PlayOneShotSelectMajor();
                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"Preset: {presetName} successfully saved!", 2f);
            }
            if(GUILayout.Button("<b>Load Stance</b>", GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
            {
                if (PresetUI.showMenu)
                {
                    PresetUI.PresetTab = PresetTab.Stances;
                    return;
                }
                PresetUI.PresetTab = PresetTab.Stances;
                PresetUI.Open();
            }
            GUILayout.EndHorizontal();
        }

        private static BaseStanceSettings GetCurrentStance()
        {
            switch (StanceTab)
            {
                case StanceTab.Riding:
                    return Main.settings.RidingStanceSettings;
                case StanceTab.RidingSwitch:
                    return Main.settings.RidingSwitchStanceSettings;
                case StanceTab.Setup:
                    return Main.settings.SetupDefaultStanceSettings;
                case StanceTab.SetupNollie:
                    return Main.settings.SetupNollieStanceSettings;
                case StanceTab.SetupSwitch:
                    return Main.settings.SetupSwitchStanceSettings;
                case StanceTab.SetupFakie:
                    return Main.settings.SetupFakieStanceSettings;
                case StanceTab.Primo:
                    return Main.settings.PrimoStanceSettings;
                case StanceTab.PrimoSetup:
                    return Main.settings.PrimoSetupDefaultStanceSettings;
                case StanceTab.PrimoSetupNollie:
                    return Main.settings.PrimoSetupNollieStanceSettings;
                case StanceTab.Catch:
                    return Main.settings.ReleaseStanceSettings;
                case StanceTab.Pushing:
                    return Main.settings.PushDefaultStanceSettings;
                case StanceTab.PushingMongo:
                    return Main.settings.PushMongoStanceSettings;
                case StanceTab.PushingSwich:
                    return Main.settings.PushSwitchStanceSettings;
                case StanceTab.PushingSwitchMongo:
                    return Main.settings.PushSwitchMongoStanceSettings;
                case StanceTab.Powerslide:
                    return Main.settings.PowerslideStanceSettings;
                case StanceTab.Grinding:
                    switch (GrindStanceTab)
                    {
                        case GrindStanceTab.BSBluntslide:
                            return Main.settings.BSBluntslideStanceSettings;
                        case GrindStanceTab.BSBoardslide:
                            return Main.settings.BSBoardslideStanceSettings;
                        case GrindStanceTab.BSCrook:
                            return Main.settings.BSCrookStanceSettings;
                        case GrindStanceTab.BSFeeble:
                            return Main.settings.BSFeebleStanceSettings;
                        case GrindStanceTab.BSFiftyFifty:
                            return Main.settings.BSFiftyFiftyStanceSettings;
                        case GrindStanceTab.BSFiveO:
                            return Main.settings.BSFiveOStanceSettings;
                        case GrindStanceTab.BSLipslide:
                            return Main.settings.BSLipslideStanceSettings;
                        case GrindStanceTab.BSLosi:
                            return Main.settings.BSLosiStanceSettings;
                        case GrindStanceTab.BSNoseblunt:
                            return Main.settings.BSNosebluntStanceSettings;
                        case GrindStanceTab.BSNosegrind:
                            return Main.settings.BSNosegrindStanceSettings;
                        case GrindStanceTab.BSNoseslide:
                            return Main.settings.BSNoseslideStanceSettings;
                        case GrindStanceTab.BSOvercrook:
                            return Main.settings.BSOvercrookStanceSettings;
                        case GrindStanceTab.BSSalad:
                            return Main.settings.BSSaladStanceSettings;
                        case GrindStanceTab.BSSmith:
                            return Main.settings.BSSmithStanceSettings;
                        case GrindStanceTab.BSSuski:
                            return Main.settings.BSSuskiStanceSettings;
                        case GrindStanceTab.BSTailslide:
                            return Main.settings.BSTailslideStanceSettings;
                        case GrindStanceTab.BSWilly:
                            return Main.settings.BSWillyStanceSettings;
                        case GrindStanceTab.FSBluntslide:
                            return Main.settings.FSBluntslideStanceSettings;
                        case GrindStanceTab.FSBoardslide:
                            return Main.settings.FSBoardslideStanceSettings;
                        case GrindStanceTab.FSCrook:
                            return Main.settings.FSCrookStanceSettings;
                        case GrindStanceTab.FSFeeble:
                            return Main.settings.FSFeebleStanceSettings;
                        case GrindStanceTab.FSFiftyFifty:
                            return Main.settings.FSFiftyFiftyStanceSettings;
                        case GrindStanceTab.FSFiveO:
                            return Main.settings.FSFiveOStanceSettings;
                        case GrindStanceTab.FSLipslide:
                            return Main.settings.FSLipslideStanceSettings;
                        case GrindStanceTab.FSLosi:
                            return Main.settings.FSLosiStanceSettings;
                        case GrindStanceTab.FSNoseblunt:
                            return Main.settings.FSNosebluntStanceSettings;
                        case GrindStanceTab.FSNosegrind:
                            return Main.settings.FSNosegrindStanceSettings;
                        case GrindStanceTab.FSNoseslide:
                            return Main.settings.FSNoseslideStanceSettings;
                        case GrindStanceTab.FSOvercrook:
                            return Main.settings.FSOvercrookStanceSettings;
                        case GrindStanceTab.FSSalad:
                            return Main.settings.FSSaladStanceSettings;
                        case GrindStanceTab.FSSmith:
                            return Main.settings.FSSmithStanceSettings;
                        case GrindStanceTab.FSSuski:
                            return Main.settings.FSSuskiStanceSettings;
                        case GrindStanceTab.FSTailslide:
                            return Main.settings.FSTailslideStanceSettings;
                        case GrindStanceTab.FSWilly:
                            return Main.settings.FSWillyStanceSettings;
                        default:
                            return Main.settings.DefaultStanceSettings;
                    }
                case StanceTab.OnButton:
                    return Main.settings.OnButtonStanceSettings;
                case StanceTab.Grabs:
                    switch (GrabStanceTab)
                    {
                        case GrabStanceTab.Indy:
                            return Main.settings.IndyStanceSettings;
                        case GrabStanceTab.Melon:
                            return Main.settings.MelonStanceSettings;
                            case GrabStanceTab.Mute:
                            return Main.settings.MuteStanceSettings;
                        case GrabStanceTab.Nosegrab:
                            return Main.settings.NosegrabStanceSettings;
                        case GrabStanceTab.Stalefish:
                            return Main.settings.StalefishStanceSettings;
                        case GrabStanceTab.Tailgrab:
                            return Main.settings.TailgrabStanceSettings;
                        default:
                            return Main.settings.DefaultStanceSettings;
                    }
                case StanceTab.GrabsOnButton:
                    switch (GrabOffBoardStanceTab)
                    {
                        case GrabOffBoardStanceTab.Simple:
                            return Main.settings.GrabsOnButtonSimpleSettings;
                        case GrabOffBoardStanceTab.Indy:
                            return Main.settings.IndyOffBoardStanceSettings;
                        case GrabOffBoardStanceTab.Melon:
                            return Main.settings.MelonOffBoardStanceSettings;
                        case GrabOffBoardStanceTab.Mute:
                            return Main.settings.MuteOffBoardStanceSettings;
                        case GrabOffBoardStanceTab.Nosegrab:
                            return Main.settings.NosegrabOffBoardStanceSettings;
                        case GrabOffBoardStanceTab.Stalefish:
                            return Main.settings.StalefishOffBoardStanceSettings;
                        case GrabOffBoardStanceTab.Tailgrab:
                            return Main.settings.TailgrabOffBoardStanceSettings;
                        default:
                            return Main.settings.DefaultStanceSettings;
                    }
                case StanceTab.Manual:
                    return Main.settings.ManualStanceSettings;
                    case StanceTab.ManualSwitch:
                    return Main.settings.ManualSwitchStanceSettings;
                case StanceTab.NoseManual:
                    return Main.settings.NoseManualStanceSettings;
                case StanceTab.NoseManualSwitch:
                    return Main.settings.NoseManualSwitchStanceSettings;
                default:
                    return Main.settings.DefaultStanceSettings;
            }
        }
    }
}