using GameManagement;
using HarmonyLib;
using ReplayEditor;
using RootMotion.FinalIK;
using SkaterXL.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Core;

namespace XXLMod3.Controller
{
    public class XXLController : MonoBehaviour
    {
        public static XXLController Instance { get; private set; }
        public bool CanPrimoCatch;
        public bool FlipDetected;
        public float GrindPopOutSidewayForce = 0.75f;
        public bool GrindStabilizer;
        public bool IsFingerFlip;
        public bool IsFootplant;
        public bool IsInHardcorePop;
        public bool IsLateFlip;
        public bool IsPrimoFlip;
        public bool LeftPop;
        public bool RightPop;
        public static CurrentState CurrentState;
        public static PhysicMaterial GrindPhysicsMaterial;
        public static PhysicMaterial PrimoPhysicsMaterial;
        public static ManualStance ManualType;

        public static Core.PopType PopType;

        public static Transform LeftFoot;
        public static Transform RightFoot;

        public static Quaternion LeftFootRot;
        public static Quaternion RightFootRot;

        public static Vector3 LeftFootPos;
        public static Vector3 RightFootPos;

        public static Vector3 BailPosition;

        private AudioSource[] DeckAudioSources;
        private AudioSource[] RagdollAudioSources;

        private bool CanSlowMo;

        public bool BoardVisible = true;
        public bool SkaterVisible = true;

        public List<BaseGrindSettings> GrindSettingObjects;

        public FullBodyBipedIK bipedIK;

        private void Awake() => Instance = this;

        private void Start()
        {
            StartCoroutine(Initialize());

            LeftFoot = PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.LeftFoot);
            RightFoot = PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.RightFoot);
            bipedIK = PlayerController.Instance.skaterController.finalIk;

            GrindSettingObjects = new List<BaseGrindSettings>()
            {
                Main.settings._bluntSlideSettings,
                Main.settings._boardSlideSettings,
                Main.settings._crookSettings,
                Main.settings._feebleSettings,
                Main.settings._fiftyFiftySettings,
                Main.settings._fiveOSettings,
                Main.settings._lipslideSettings,
                Main.settings._losiSettings,
                Main.settings._nosebluntSettings,
                Main.settings._nosegrindSettings,
                Main.settings._noseslideSettings,
                Main.settings._overcrookSettings,
                Main.settings._saladSettings,
                Main.settings._smithSettings,
                Main.settings._suskiSettings,
                Main.settings._tailslideSettings,
                Main.settings._willySettings
            };

            GrindPhysicsMaterial = new PhysicMaterial();
            PrimoPhysicsMaterial = new PhysicMaterial();

            DeckAudioSources = (Traverse.Create(DeckSounds.Instance).Field("_allSources").GetValue() as AudioSource[]);
            RagdollAudioSources = SoundManager.Instance.ragdollSounds.GetAllSources();
        }

        private IEnumerator Initialize()
        {
            yield return new WaitWhile(() => !PlayerController.Instance.inputController.controlsActive);

            InjectCustomPlayerStates();
        }

        private void InjectCustomPlayerStates()
        {
            PlayerController.Instance.playerSM.StartSM();
            PlayerController.Instance.respawn.DoRespawn();
        }

        private void Update()
        {
            if (CurrentState == CurrentState.Pop || CurrentState == CurrentState.InAir || CurrentState == CurrentState.Released)
            {
                InputController.Instance.TriggerMultiplier = Main.settings.InAirTurnSpeed;
            }

            //if (Input.GetKeyDown(KeyCode.N))
            //{
            //    GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    gameObject.AddComponent<BoxCollider>();
            //    Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            //    rb.interpolation = RigidbodyInterpolation.Interpolate;
            //    gameObject.AddComponent<ObjectTracker>();
            //    gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("HDRP/Lit");
            //    gameObject.transform.position = PlayerController.Instance.skaterController.skaterTargetTransform.position + PlayerController.Instance.skaterController.skaterTransform.forward * 3f + PlayerController.Instance.skaterController.skaterTargetTransform.up * 1f;
            //    gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            //}

            if (FlipDetected && Main.settings.SlowMotionFlips)
            {
                if (CanSlowMo)
                {
                    Time.timeScale = 0.25f;
                    Time.fixedDeltaTime = Time.timeScale * 0.00833f;

                    for (int i = 0; i < DeckAudioSources.Length; i++)
                    {
                        DeckAudioSources[i].pitch = Time.timeScale;
                    }
                    CanSlowMo = false;
                }
            }
            else
            {
                if (!CanSlowMo)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.00833f;

                    for (int i = 0; i < DeckAudioSources.Length; i++)
                    {
                        DeckAudioSources[i].pitch = Time.timeScale;
                    }
                    CanSlowMo = true;
                }
            }
        }

        public void ActivateSlowMotion(bool canSlowMo, float speed)
        {
            if (canSlowMo)
            {
                Time.timeScale = speed;
                Time.fixedDeltaTime = Time.timeScale * 0.00833f;

                for (int i = 0; i < DeckAudioSources.Length; i++)
                {
                    DeckAudioSources[i].pitch = Time.timeScale;
                }

                for (int i = 0; i < RagdollAudioSources.Length; i++)
                {
                    RagdollAudioSources[i].pitch = Time.timeScale;
                }
            }
        }

        public void ResetTime(bool wasSlowMoActive)
        {
            if (wasSlowMoActive)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.00833f;

                for (int i = 0; i < DeckAudioSources.Length; i++)
                {
                    DeckAudioSources[i].pitch = Time.timeScale;
                }

                for (int i = 0; i < RagdollAudioSources.Length; i++)
                {
                    RagdollAudioSources[i].pitch = Time.timeScale;
                }
            }
        }

        public void HideBoard()
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PlayState) || GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState))
            {
                BoardVisible = false;
                CharacterCustomizer cc = PlayerController.Instance.characterCustomizer;
                cc.DeckParent.gameObject.SetActive(false);
                cc.TruckBaseParents[0].gameObject.SetActive(false);
                cc.TruckBaseParents[1].gameObject.SetActive(false);
                cc.TruckHangerParents[0].gameObject.SetActive(false);
                cc.TruckHangerParents[1].gameObject.SetActive(false);
                cc.WheelParents[0].gameObject.SetActive(false);
                cc.WheelParents[1].gameObject.SetActive(false);
                cc.WheelParents[2].gameObject.SetActive(false);
                cc.WheelParents[3].gameObject.SetActive(false);

                CharacterCustomizer rcc = ReplayEditorController.Instance.playbackController.characterCustomizer;
                rcc.DeckParent.gameObject.SetActive(false);
                rcc.TruckBaseParents[0].gameObject.SetActive(false);
                rcc.TruckBaseParents[1].gameObject.SetActive(false);
                rcc.TruckHangerParents[0].gameObject.SetActive(false);
                rcc.TruckHangerParents[1].gameObject.SetActive(false);
                rcc.WheelParents[0].gameObject.SetActive(false);
                rcc.WheelParents[1].gameObject.SetActive(false);
                rcc.WheelParents[2].gameObject.SetActive(false);
                rcc.WheelParents[3].gameObject.SetActive(false);
            }
        }

        public void ShowBoard()
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PlayState) || GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState))
            {
                BoardVisible = true;
                CharacterCustomizer cc = PlayerController.Instance.characterCustomizer;
                cc.DeckParent.gameObject.SetActive(true);
                cc.TruckBaseParents[0].gameObject.SetActive(true);
                cc.TruckBaseParents[1].gameObject.SetActive(true);
                cc.TruckHangerParents[0].gameObject.SetActive(true);
                cc.TruckHangerParents[1].gameObject.SetActive(true);
                cc.WheelParents[0].gameObject.SetActive(true);
                cc.WheelParents[1].gameObject.SetActive(true);
                cc.WheelParents[2].gameObject.SetActive(true);
                cc.WheelParents[3].gameObject.SetActive(true);

                CharacterCustomizer rcc = ReplayEditorController.Instance.playbackController.characterCustomizer;
                rcc.DeckParent.gameObject.SetActive(true);
                rcc.TruckBaseParents[0].gameObject.SetActive(true);
                rcc.TruckBaseParents[1].gameObject.SetActive(true);
                rcc.TruckHangerParents[0].gameObject.SetActive(true);
                rcc.TruckHangerParents[1].gameObject.SetActive(true);
                rcc.WheelParents[0].gameObject.SetActive(true);
                rcc.WheelParents[1].gameObject.SetActive(true);
                rcc.WheelParents[2].gameObject.SetActive(true);
                rcc.WheelParents[3].gameObject.SetActive(true);
            }
        }

        public void HidePlayer()
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PlayState) || GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState))
            {
                SkaterVisible = false;
                CharacterCustomizer cc = PlayerController.Instance.characterCustomizer;
                cc.ClothingParent.gameObject.SetActive(false);

                CharacterCustomizer rcc = ReplayEditorController.Instance.playbackController.characterCustomizer;
                rcc.ClothingParent.gameObject.SetActive(false);
            }
        }

        public void ShowPlayer()
        {
            SkaterVisible = true;
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PlayState) || GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState))
            {
                CharacterCustomizer cc = PlayerController.Instance.characterCustomizer;
                cc.ClothingParent.gameObject.SetActive(true);

                CharacterCustomizer rcc = ReplayEditorController.Instance.playbackController.characterCustomizer;
                rcc.ClothingParent.gameObject.SetActive(true);
            }
        }

        public void MuteEnvironment()
        {
            AudioSource[] allSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource aS in allSources)
            {
                aS.volume = 0f;
            }
            foreach (AudioSource deckSounds in DeckAudioSources)
            {
                deckSounds.volume = 1f;
            }
            foreach(AudioSource ragdollSounds in RagdollAudioSources)
            {
                ragdollSounds.volume = 1f;
            }
        }

        private void FixedUpdate()
        {
            if(CurrentState == CurrentState.Grabs)
            {
                switch (Main.settings.BodyflipMode)
                {
                    case BodyflipMode.LS:
                        if (PlayerController.Instance.inputController.LeftStick.rawInput.pos.x >= 0.1f || PlayerController.Instance.inputController.LeftStick.rawInput.pos.x <= -0.1f || PlayerController.Instance.inputController.LeftStick.rawInput.pos.y <= -0.1f || PlayerController.Instance.inputController.LeftStick.rawInput.pos.y >= 0.1f)
                        {
                            if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Target)
                            {
                                PlayerController.Instance.SetTargetToMaster();
                                PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = new Vector3(0, 0, 0);
                            }
                            PlayerController.Instance.boardController.currentRotationTarget = PlayerController.Instance.skaterController.skaterTransform.rotation;
                            if (PlayerController.Instance.inputController.LeftStick.rawInput.pos.x != 0)
                            {
                                PlayerController.Instance.skaterController.skaterTransform.Rotate(0, PlayerController.Instance.inputController.LeftStick.rawInput.pos.x * 5 * Time.deltaTime * Main.settings.BodyflipSpeed, 0, Space.Self);
                            }
                            if (PlayerController.Instance.inputController.LeftStick.rawInput.pos.y != 0)
                            {
                                PlayerController.Instance.skaterController.skaterTransform.Rotate(PlayerController.Instance.inputController.LeftStick.rawInput.pos.y * 5 * Time.deltaTime * Main.settings.BodyflipSpeed, 0, 0, Space.Self);
                            }
                        }
                        break;
                    case BodyflipMode.RS:
                        if (PlayerController.Instance.inputController.RightStick.rawInput.pos.x >= 0.1f || PlayerController.Instance.inputController.RightStick.rawInput.pos.x <= -0.1f || PlayerController.Instance.inputController.RightStick.rawInput.pos.y <= -0.1f || PlayerController.Instance.inputController.RightStick.rawInput.pos.y >= 0.1f)
                        {
                            if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Target)
                            {
                                PlayerController.Instance.SetTargetToMaster();
                                PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = new Vector3(0, 0, 0);
                            }
                            PlayerController.Instance.boardController.currentRotationTarget = PlayerController.Instance.skaterController.skaterRigidbody.rotation;
                            if (PlayerController.Instance.inputController.RightStick.rawInput.pos.x != 0)
                            {
                                PlayerController.Instance.skaterController.skaterTransform.Rotate(0, -PlayerController.Instance.inputController.RightStick.rawInput.pos.x * 5 * Time.deltaTime * Main.settings.BodyflipSpeed, 0, Space.Self);
                            }
                            if (PlayerController.Instance.inputController.RightStick.rawInput.pos.y != 0)
                            {
                                PlayerController.Instance.skaterController.skaterTransform.Rotate(PlayerController.Instance.inputController.RightStick.rawInput.pos.y * 5 * Time.deltaTime * Main.settings.BodyflipSpeed, 0, 0, Space.Self);
                            }
                        }
                        break;
                    case BodyflipMode.Off:
                        return;
                }
            }
        }

        private void LateUpdate()
        {
            MultiplayerManager.PopupDuration = Main.settings.PopUpMessagesTimer;
            Physics.gravity = new Vector3(0, Main.settings.Gravity, 0);
            PlayerController.Instance.skaterController.pushForce = Main.settings.PushForce;
            PlayerController.Instance.topSpeed = Main.settings.TopSpeed;
            PlayerController.Instance.skaterController.skaterRigidbody.maxAngularVelocity = Main.settings.MaxAngularVelocity;

            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState))
            {
                Traverse.Create(ReplayEditorController.Instance).Field("playbackSpeed").SetValue(Main.settings.ReplayPlaybackSpeed);
            }
        }

        public void ResetFlips()
        {
            IsFingerFlip = false;
            IsLateFlip = false;
            IsPrimoFlip = false;
        }

        public void SetMuscleWeight()
        {
            PlayerController.Instance.respawn.puppetMaster.muscleWeight = 0f;
            PlayerController.Instance.respawn.puppetMaster.muscleSpring = 0f;
            PlayerController.Instance.respawn.puppetMaster.muscleDamper = 0f;
        }

        public void ResetMuscleWeight()
        {
            PlayerController.Instance.respawn.puppetMaster.muscleWeight = 1f;
            PlayerController.Instance.respawn.puppetMaster.muscleSpring = 200f;
            PlayerController.Instance.respawn.puppetMaster.muscleDamper = 100f;
        }

        public void PressureFlip()
        {
            if (Main.settings.PressureFlips || (IsPrimoFlip && Main.settings.PrimoPressureFlips))
            {
                if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                {
                    if (PlayerController.Instance.inputController.LeftStick.rawInput.pos.x >= 0.1f)
                    {
                        if (PlayerController.Instance.boardController.IsBoardBackwards)
                        {
                            PlayerController.Instance.SetRightIKLerpTarget(1f, 0.5f);
                            PlayerController.Instance.SetLeftIKLerpTarget(0.5f, 0f);
                            PlayerController.Instance.boardController.thirdVel = -40f;
                            return;
                        }
                        PlayerController.Instance.SetRightIKLerpTarget(1f, 0.5f);
                        PlayerController.Instance.SetLeftIKLerpTarget(0.5f, 0f);
                        PlayerController.Instance.boardController.thirdVel = 40f;
                    }
                    if (PlayerController.Instance.inputController.LeftStick.rawInput.pos.x <= -0.1f)
                    {
                        if (PlayerController.Instance.boardController.IsBoardBackwards)
                        {
                            PlayerController.Instance.SetRightIKLerpTarget(0.8f, 1f);
                            PlayerController.Instance.SetLeftIKLerpTarget(0f, 1f);
                            PlayerController.Instance.boardController.thirdVel = 40f;
                            return;
                        }
                        PlayerController.Instance.SetRightIKLerpTarget(0.8f, 1f);
                        PlayerController.Instance.SetLeftIKLerpTarget(0f, 1f);
                        PlayerController.Instance.boardController.thirdVel = -40f;
                    }
                }
                if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                {
                    if (PlayerController.Instance.inputController.RightStick.rawInput.pos.x >= 0.1f)
                    {
                        if (PlayerController.Instance.boardController.IsBoardBackwards)
                        {
                            PlayerController.Instance.SetRightIKLerpTarget(0f, 1f);
                            PlayerController.Instance.SetLeftIKLerpTarget(0.8f, 1f);
                            PlayerController.Instance.boardController.thirdVel = -40f;
                            return;
                        }
                        PlayerController.Instance.SetRightIKLerpTarget(0f, 1f);
                        PlayerController.Instance.SetLeftIKLerpTarget(0.8f, 1f);
                        PlayerController.Instance.boardController.thirdVel = 40f;
                    }
                    if (PlayerController.Instance.inputController.RightStick.rawInput.pos.x <= -0.1f)
                    {
                        if (PlayerController.Instance.boardController.IsBoardBackwards)
                        {
                            PlayerController.Instance.SetRightIKLerpTarget(0f, 1f);
                            PlayerController.Instance.SetLeftIKLerpTarget(1f, 0.5f);
                            PlayerController.Instance.boardController.thirdVel = 40f;
                            return;
                        }
                        PlayerController.Instance.SetRightIKLerpTarget(0f, 1f);
                        PlayerController.Instance.SetLeftIKLerpTarget(0.8f, 1f);
                        PlayerController.Instance.boardController.thirdVel = -40f;
                    }
                }
            }
        }

        public void VerticalFlip()
        {
            if (Main.settings.VerticalFlips)
            {
                if (SettingsManager.Instance.stance == Stance.Goofy)
                {
                    if (PlayerController.Instance.inputController.player.GetButton("RB") && !PlayerController.Instance.inputController.player.GetButton("LB"))
                    {
                        if (PlayerController.Instance.boardController.IsBoardBackwards)
                        {
                            PlayerController.Instance.boardController.firstVel = Main.settings.Verticality;
                            return;
                        }
                        PlayerController.Instance.boardController.firstVel = -Main.settings.Verticality;
                    }
                    else if (PlayerController.Instance.inputController.player.GetButton("LB") && !PlayerController.Instance.inputController.player.GetButton("RB"))
                    {
                        if (PlayerController.Instance.boardController.IsBoardBackwards)
                        {
                            PlayerController.Instance.boardController.firstVel = -Main.settings.Verticality;
                            return;
                        }
                        PlayerController.Instance.boardController.firstVel = Main.settings.Verticality;
                    }
                }
                else
                {
                    if (PlayerController.Instance.inputController.player.GetButton("LB") && !PlayerController.Instance.inputController.player.GetButton("RB"))
                    {
                        if (PlayerController.Instance.boardController.IsBoardBackwards)
                        {
                            PlayerController.Instance.boardController.firstVel = -Main.settings.Verticality;
                            return;
                        }
                        PlayerController.Instance.boardController.firstVel = Main.settings.Verticality;
                    }
                    else if (PlayerController.Instance.inputController.player.GetButton("RB") && !PlayerController.Instance.inputController.player.GetButton("LB"))
                    {
                        if (PlayerController.Instance.boardController.IsBoardBackwards)
                        {
                            PlayerController.Instance.boardController.firstVel = Main.settings.Verticality;
                            return;
                        }
                        PlayerController.Instance.boardController.firstVel = -Main.settings.Verticality;
                    }
                }
            }
        }
    }
}