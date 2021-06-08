using System;
using UnityEngine;
using UnityModManagerNet;
using XXLMod3.Core;

namespace XXLMod3
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw(DrawType.KeyBinding)] public KeyBinding XXLHotkey = new KeyBinding { keyCode = KeyCode.F7 };
        [Draw(DrawType.KeyBinding)] public KeyBinding StanceHotkey = new KeyBinding { keyCode = KeyCode.U };
        [Draw(DrawType.KeyBinding)] public KeyBinding PresetsHotkey = new KeyBinding { keyCode = KeyCode.P };
        [Draw(DrawType.KeyBinding)] public KeyBinding MultiplayerHotkey = new KeyBinding { keyCode = KeyCode.M };

        public Color BGColor = new Color(0f,0f,0f);

        #region GeneralSettings
        public float Gravity = -9.807f;
        public AdvancedPop AdvancedPop = AdvancedPop.Off;
        public float ForwardPopForce = 100f;
        public float SidewayPopForce = 100f;
        public bool IndividualPopForce = false;
        public float DefaultPopForce = 3f;
        public float NolliePopForce = 3f;
        public float SwitchPopForce = 3f;
        public float FakiePopForce = 3f;
        public float HighPopForceMult = 0.5f;
        public bool BabyPop = false;
        public float BabyPopForceMult = 0.6f;
        public float InAirTurnSpeed = 1f;
        public float MaxAngularVelocity = 7f;
        public bool PopDelay = true;
        public float PumpForceMult = 1.0f;
        public float FirstPushForce = 0.5f;
        public float PushForce = 8f;
        public float RevertSpeed = 1f;
        public float TopSpeed = 8f;
        public DecoupledMode DecoupledMode = DecoupledMode.Off;
        #endregion

        #region CatchSettings
        public float CatchBoardRotateSpeed = 57.29578f;
        public bool CatchCorrection = true;
        public float CatchCorrectionSpeed = 0f;
        public CatchMode CatchMode = CatchMode.Auto;
        public bool CatchSmooth = false;
        public bool RealisticDrops = false;
        public bool StompCatch = false;
        public float StompCatchForce = 1f;
        #endregion

        #region FlipSettings
        public float FlipAnimationSpeed = 1f;
        public float FlipBoardOffset = 0f;
        public float FlipSpeed = 1f;
        public float FlipStrength = 1f;
        public bool LaidbackFlips = false;
        public bool MidAirFlip = true;
        public bool MidFlipShuv = true;
        public float PopKickLeft = -0.7f;
        public float PopKickRight = 0.7f;
        public float ScoopSpeed = 1f;
        public bool PressureFlips = false;
        public bool VerticalFlips = false;
        public float Verticality = 1f;
        #endregion

        #region LateflipSettings
        public bool Lateflips = false;
        public float LateFlipAnimationSpeed = 1f;
        public float LateFlipBoardOffset = 0.15f;
        public float LateFlipSpeed = 1f;
        public float LateFlipStrength = 1f;
        public bool LateFlipLaidbackFlips = true;
        public float LateScoopSpeed = 1f;
        #endregion

        #region GrabSettings
        public bool Grabs = true;
        public BodyflipMode BodyflipMode = BodyflipMode.Off;
        public float BodyflipSpeed = 60f;
        public float GrabBoardOffset = 0f;
        public bool GrabDelay = true;
        public OneFootGrabMode OneFootGrabMode = OneFootGrabMode.Off;
        #endregion

        #region FingerflipSettings
        public bool Fingerflips = false;
        public float FingerFlipBoardOffset = 0f;
        public float FingerFlipSpeed = 1f;
        public float FingerScoopSpeed = 1f;
        #endregion

        #region FootplantSettings
        public bool Footplants = false;
        public float FootplantBoardOffset = 0f;
        public float FootplantForwardForce = 2f;
        public float FootplantJumpForce = 2f;
        #endregion

        #region GrindSettings
        public bool Grinds = true;
        public AdvancedGrinds AdvancedGrinds = AdvancedGrinds.Bluntslide;
        public bool AdvancedGrinding = false;
        public CustomGrindSettings _generalGrindSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _bluntSlideSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _boardSlideSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _crookSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _feebleSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _fiftyFiftySettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _fiveOSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _lipslideSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _losiSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _nosebluntSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _nosegrindSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _noseslideSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _overcrookSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _saladSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _smithSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _suskiSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _tailslideSettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public CustomGrindSettings _willySettings = new CustomGrindSettings(1f, true, 1.25f, 1.2f, CrouchMode.Off, 0.95f, 0.25f, 15f, 2f, true, 15f, 0.75f, true, 44f);
        public bool GrindBumpDelay = true;
        public bool GrindGrab = false;
        public bool InfiniteStallTime = false;
        public InstantStallMode InstantStallMode = InstantStallMode.Off;
        public PopOutDirection PopOutDirection = PopOutDirection.Default;
        public OneFootMode GrindOneFootMode = OneFootMode.Off;
        public float GrindTurnSpeed = 1f;
        #endregion

        #region ManualSettings
        public bool ManualBabyPop = false;
        public float ManualBabyPopForce = 1.5f;
        public bool ManualBraking = false;
        public float ManualCrouchAmount = 1f;
        public CrouchMode ManualCrouchMode = CrouchMode.LB;
        public bool ManualDelay = true;
        public float ManualRevertSensitivity = 0.3f;
        public float ManualMaxAngle = 10f;
        public bool ManualPopDelay = true;
        public float ManualPopForce = 2.5f;
        public OneFootMode ManualOneFootMode = OneFootMode.Off;
        #endregion

        #region PrimoSettings
        public bool Primos = false;
        public float PrimoFlipAnimationSpeed = 1f;
        public float PrimoBoardOffset = 0f;
        public float PrimoFlipSpeed = 1f;
        public float PrimoFlipStrength = 1f;
        public float PrimoFriction = 0.75f;
        public bool PrimoLaidbackFlips = false;
        public float PrimoPopForce = 3f;
        public bool PrimoPressureFlips = true;
        public float PrimoScoopSpeed = 1f;
        public float PrimoTurnSpeed = 1f;
        #endregion

        #region BailSettings
        public bool AutoRespawn = true;
        public bool BetterBails = false;
        public bool BailControls = false;
        public bool BailLookAtPlayer = false;
        public float BailDownForce = 2f;
        public float BailUpForce = 5f;
        public float BailArmForce = 1f;
        public float BailLegForce = 1f;
        public bool BailRespawnAt = false;
        #endregion

        #region MiscSettings
        public bool AutoBanklean = true;
        public bool AutoBoardRotationSnap = true;
        public bool AutoPump = true;
        public bool AutoRevert = true;
        public bool FlipsAfterPop = true;
        public bool HippieOllie = false;
        public bool LockTurningWhileWindUp = false;
        public float PowerslideCrouchAmount = 0.94196f;
        public float PushCrouchAmount = 1.04196f;
        public bool RandomImpactAnimations = false;
        public bool ReducedStickTurning = false;
        public bool ShuvMidFlip = true;
        #endregion

        #region OtherSettings
        public bool StartReplayAtLastFrame = false;
        public float ReplayPlaybackSpeed = 1f;
        public bool SlowMotionBails = false;
        public float SlowMotionBailSpeed = 0.5f;
        public bool SlowMotionFlips = false;
        public float SlowMotionFlipSpeed = 0.5f;
        public bool SlowMotionGrinds = false;
        public float SlowMotionGrindSpeed = 0.5f;
        public bool SlowMotionGrabs = false;
        public float SlowMotionGrabSpeed = 0.5f;
        public bool SlowMotionManuals = false;
        public float SlowMotionManualSpeed = 0.5f;
        public bool SlowMotionPrimos = false;
        public float SlowMotionPrimoSpeed = 0.5f;
        public bool SlowMotionReverts = false;
        public float SlowMotionRevertSpeed = 0.5f;
        public bool UseEscToQuit = false;
        #endregion

        #region Fixes
        public bool GrabFix = false;
        public bool ManualFix = false;
        public bool ShuvLegFix = false;
        #endregion

        #region LegCustomizer
        //public bool DynamicSteezeLegs = true;
        public CustomLegSettings DefaultFlipLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings OllieFlipLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings NollieFlipLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings SwitchFlipLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings FakieFlipLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings DefaultSteezeLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings OllieSteezeLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings NollieSteezeLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings SwitchSteezeLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        public CustomLegSettings FakieSteezeLegs = new CustomLegSettings(false, 0f, 0f, 0f, 0f, 0f, 0f);
        #endregion

        #region StanceSettings
        public float PositionSensitivity = 0.1f;
        public float RotationSensitivity = 0.1f;

        public float lfFreeFootMovementSpeedXL = 0.05f;
        public float lfFreeFootMovementSpeedXR = -0.05f;
        public float lfFreeFootMovementSpeedZF = 0.05f;
        public float lfFreeFootMovementSpeedZB = -0.05f;

        public float rfFreeFootMovementSpeedXL = 0.05f;
        public float rfFreeFootMovementSpeedXR = -0.05f;
        public float rfFreeFootMovementSpeedZF = 0.05f;
        public float rfFreeFootMovementSpeedZB = -0.05f;

        public float lfIKOffsetXL = 0.05f;
        public float lfIKOffsetXR = -0.05f;
        public float lfIKOffsetZB = -0.05f;
        public float lfIKOffsetZF = 0.05f;

        public float rfIKOffsetXL = 0.05f;
        public float rfIKOffsetXR = -0.05f;
        public float rfIKOffsetZB = -0.05f;
        public float rfIKOffsetZF = 0.05f;

        public float lfIKOffsetSpecialXL = 0.05f;
        public float lfIKOffsetSpecialXR = -0.05f;
        public float lfIKOffsetSpecialZB = -0.05f;
        public float lfIKOffsetSpecialZF = 0.05f;

        public float rfIKOffsetSpecialXL = 0.05f;
        public float rfIKOffsetSpecialXR = -0.05f;
        public float rfIKOffsetSpecialZB = -0.05f;
        public float rfIKOffsetSpecialZF = 0.05f;

        public Vector3 lfOriginalPos = Vector3.zero;
        public Vector3 rfOriginalPos = Vector3.zero;

        public Quaternion ltOriginalRot = Quaternion.identity;
        public Quaternion rtOriginalRot = Quaternion.identity;

        public bool UseSimpleOnButtonGrabs = false;
        public bool UseSpecialInReleaseState = true;
        public bool UseSpecialInPrimoState = true;
        public bool UseRandomLanding = true;
        #endregion

        #region MultiplayerSettings
        public float PopUpMessagesTimer = 3f;
        #endregion

        #region Stances
        public CustomStanceSettings DefaultStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings InAirStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings OnButtonStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings PowerslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings PushDefaultStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings PushMongoStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings PushSwitchStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings PushSwitchMongoStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings PrimoStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings PrimoSetupDefaultStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings PrimoSetupNollieStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings ReleaseStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings RidingStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings RidingSwitchStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings SetupDefaultStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings SetupFakieStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings SetupNollieStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings SetupSwitchStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings ManualStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings ManualSwitchStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings NoseManualStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings NoseManualSwitchStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings ManualOnButtonSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);

        public CustomStanceSettings BSBluntslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSBoardslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSCrookStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSFeebleStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSFiftyFiftyStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSFiveOStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSLipslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSLosiStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSNosebluntStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSNosegrindStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSNoseslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSOvercrookStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSSaladStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSSmithStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSSuskiStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSTailslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings BSWillyStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);

        public CustomStanceSettings FSBluntslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSBoardslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSCrookStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSFeebleStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSFiftyFiftyStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSFiveOStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSLipslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSLosiStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSNosebluntStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSNosegrindStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSNoseslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSOvercrookStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSSaladStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSSmithStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSSuskiStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSTailslideStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings FSWillyStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);

        public CustomStanceSettings GrindOnButtonSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);

        public CustomStanceSettings IndyStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings MelonStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings MuteStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings NosegrabStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings StalefishStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings TailgrabStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);

        public CustomStanceSettings GrabsOnButtonSimpleSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings IndyOffBoardStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings MelonOffBoardStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings MuteOffBoardStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings NosegrabOffBoardStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings StalefishOffBoardStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomStanceSettings TailgrabOffBoardStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        #endregion

        #region XLAnimationModifier
        public bool FixedRevertAnimation = false;
        #endregion

        public void OnChange()
        {
            throw new NotImplementedException();
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save<Settings>(this, modEntry);
        }
    }
}