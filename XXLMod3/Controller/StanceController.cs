using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XXLMod3.Core;
using XXLMod3.Windows;

namespace XXLMod3.Controller
{
    public class StanceController : MonoBehaviour
    {
        public static StanceController Instance { get; private set; }

        public Transform LeftFootTransform;
        private Transform RightFootTransform;

        public Transform LeftFootRotTransform;
        public Transform RightFootRotTransform;

        public static GameObject LeftFootIndicator;
        public static GameObject RightFootIndicator;

        public static GameObject LeftFootRotIndicator;
        public static GameObject RightFootRotIndicator;

        public GameObject ActiveLeftFootTarget;
        public GameObject ActiveRightFootTarget;

        public GameObject ActiveLeftFootRotTarget;
        public GameObject ActiveRightFootRotTarget;

        public GameObject ActiveLeftToeRotTarget;
        public GameObject ActiveRightToeRotTarget;

        private Transform LeftToeTransform;
        private Transform RightToeTransform;

        public GameObject LTIndicator;
        public GameObject RTIndicator;

        public static GameObject LTDefaultRot;
        public static GameObject RTDefaultRot;

        public static GameObject LTNollieRot;
        public static GameObject RTNollieRot;

        public static GameObject LTSwitchRot;
        public static GameObject RTSwitchRot;

        public static GameObject LTFakieRot;
        public static GameObject RTFakieRot;

        private bool IsInitialized = false;

        public CustomStanceSettings DefaultStanceSettings = new CustomStanceSettings(true, 1f, 100f, 1f, 100f, Quaternion.identity, Quaternion.identity, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity);
        public CustomFeetObject DefaultFeet = new CustomFeetObject();
        private CustomFeetObject RidingFeet = new CustomFeetObject();
        private CustomFeetObject RidingSwitchFeet = new CustomFeetObject();
        private CustomFeetObject SetupDefaultFeet = new CustomFeetObject();
        private CustomFeetObject SetupNollieFeet = new CustomFeetObject();
        private CustomFeetObject SetupSwitchFeet = new CustomFeetObject();
        private CustomFeetObject SetupFakieFeet = new CustomFeetObject();
        private CustomFeetObject PrimoFeet = new CustomFeetObject();
        private CustomFeetObject PrimoSetupDefaultFeet = new CustomFeetObject();
        private CustomFeetObject PrimoSetupNollieFeet = new CustomFeetObject();
        private CustomFeetObject PowerslideFeet = new CustomFeetObject();
        private CustomFeetObject PushingDefaultFeet = new CustomFeetObject();
        private CustomFeetObject PushingMongoFeet = new CustomFeetObject();
        private CustomFeetObject PushingSwitchFeet = new CustomFeetObject();
        private CustomFeetObject PushingSwitchMongoFeet = new CustomFeetObject();
        private CustomFeetObject InAirFeet = new CustomFeetObject();
        private CustomFeetObject ReleaseFeet = new CustomFeetObject();
        public CustomFeetObject OnButtonFeet = new CustomFeetObject();
        private CustomFeetObject RandomFeet = new CustomFeetObject();
        private CustomFeetObject ManualFeet = new CustomFeetObject();
        private CustomFeetObject ManualSwitchFeet = new CustomFeetObject();
        private CustomFeetObject NoseManualFeet = new CustomFeetObject();
        private CustomFeetObject NoseManualSwitchFeet = new CustomFeetObject();
        private CustomFeetObject ManualOnButtonFeet = new CustomFeetObject();

        public CustomFeetObject BSBluntslideFeet = new CustomFeetObject();
        public CustomFeetObject BSBoardslideFeet = new CustomFeetObject();
        public CustomFeetObject BSCrookFeet = new CustomFeetObject();
        public CustomFeetObject BSFeebleFeet = new CustomFeetObject();
        public CustomFeetObject BSFiftyFiftyFeet = new CustomFeetObject();
        public CustomFeetObject BSFiveOFeet = new CustomFeetObject();
        public CustomFeetObject BSLipslideFeet = new CustomFeetObject();
        public CustomFeetObject BSLosiFeet = new CustomFeetObject();
        public CustomFeetObject BSNosebluntFeet = new CustomFeetObject();
        public CustomFeetObject BSNosegrindFeet = new CustomFeetObject();
        public CustomFeetObject BSNoseslideFeet = new CustomFeetObject();
        public CustomFeetObject BSOvercrookFeet = new CustomFeetObject();
        public CustomFeetObject BSSaladFeet = new CustomFeetObject();
        public CustomFeetObject BSSmithFeet = new CustomFeetObject();
        public CustomFeetObject BSSuskiFeet = new CustomFeetObject();
        public CustomFeetObject BSTailslideFeet = new CustomFeetObject();
        public CustomFeetObject BSWillyFeet = new CustomFeetObject();

        public CustomFeetObject FSBluntslideFeet = new CustomFeetObject();
        public CustomFeetObject FSBoardslideFeet = new CustomFeetObject();
        public CustomFeetObject FSCrookFeet = new CustomFeetObject();
        public CustomFeetObject FSFeebleFeet = new CustomFeetObject();
        public CustomFeetObject FSFiftyFiftyFeet = new CustomFeetObject();
        public CustomFeetObject FSFiveOFeet = new CustomFeetObject();
        public CustomFeetObject FSLipslideFeet = new CustomFeetObject();
        public CustomFeetObject FSLosiFeet = new CustomFeetObject();
        public CustomFeetObject FSNosebluntFeet = new CustomFeetObject();
        public CustomFeetObject FSNosegrindFeet = new CustomFeetObject();
        public CustomFeetObject FSNoseslideFeet = new CustomFeetObject();
        public CustomFeetObject FSOvercrookFeet = new CustomFeetObject();
        public CustomFeetObject FSSaladFeet = new CustomFeetObject();
        public CustomFeetObject FSSmithFeet = new CustomFeetObject();
        public CustomFeetObject FSSuskiFeet = new CustomFeetObject();
        public CustomFeetObject FSTailslideFeet = new CustomFeetObject();
        public CustomFeetObject FSWillyFeet = new CustomFeetObject();

        public CustomFeetObject GrindOnButtonFeet = new CustomFeetObject();

        public CustomFeetObject IndyFeet = new CustomFeetObject();
        public CustomFeetObject MelonFeet = new CustomFeetObject();
        public CustomFeetObject MuteFeet = new CustomFeetObject();
        public CustomFeetObject NosegrabFeet = new CustomFeetObject();
        public CustomFeetObject StalefishFeet = new CustomFeetObject();
        public CustomFeetObject TailgrabFeet = new CustomFeetObject();

        public CustomFeetObject GrabsOnButtonSimpleFeet = new CustomFeetObject();
        public CustomFeetObject IndyOffBoardFeet = new CustomFeetObject();
        public CustomFeetObject MelonOffBoardFeet = new CustomFeetObject();
        public CustomFeetObject MuteOffBoardFeet = new CustomFeetObject();
        public CustomFeetObject NosegrabOffBoardFeet = new CustomFeetObject();
        public CustomFeetObject StalefishOffBoardFeet = new CustomFeetObject();
        public CustomFeetObject TailgrabOffBoardFeet = new CustomFeetObject();

        List<CustomFeetObject> CustomFeetObjects = new List<CustomFeetObject>();

        public static bool IsInEditMode;
        public static bool IsMongoPushing;
        public static bool IsRevertTriggered;

        private bool GetRandomNumberLeft = false;
        private bool GetRandomNumberRight = false;

        private float RandomLeftX = 0f;
        private float RandomLeftZ = 0f;
        private float RandomRightX = 0f;
        private float RandomRightZ = 0f;

        public bool IsRandomStance;

        private void Awake() => Instance = this;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (LeftFootTransform == null)
            {
                if(SettingsManager.Instance.stance == SkaterXL.Core.Stance.Goofy)
                {
                    LeftFootTransform = Traverse.Create(PlayerController.Instance.ikController).Field("ikLeftFootPositionOffsetGoofy").GetValue() as Transform;
                }
                else
                {
                    LeftFootTransform = Traverse.Create(PlayerController.Instance.ikController).Field("ikLeftFootPositionOffset").GetValue() as Transform;
                }
                LeftFootRotTransform = Traverse.Create(PlayerController.Instance.ikController).Field("ikAnimLeftFootTarget").GetValue() as Transform;
                LeftToeTransform = PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.LeftToes);

                Main.settings.lfOriginalPos = LeftFootTransform.localPosition;

                LeftFootIndicator = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, LeftFootTransform.position, LeftFootTransform.rotation, LeftFootTransform);
                LeftFootRotIndicator = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, LeftFootRotTransform.position, LeftFootRotTransform.rotation, LeftFootRotTransform);
                LTIndicator = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, LeftToeTransform.transform.position, LeftToeTransform.transform.rotation, LeftFootIndicator.transform);
                LTIndicator.transform.localRotation = PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.LeftToes).localRotation;
            }

            if (RightFootTransform == null)
            {
                if(SettingsManager.Instance.stance == SkaterXL.Core.Stance.Goofy)
                {
                    RightFootTransform = Traverse.Create(PlayerController.Instance.ikController).Field("ikRightFootPositionOffsetGoofy").GetValue() as Transform;
                }
                else
                {
                    RightFootTransform = Traverse.Create(PlayerController.Instance.ikController).Field("ikRightFootPositionOffset").GetValue() as Transform;
                }
                RightFootRotTransform = Traverse.Create(PlayerController.Instance.ikController).Field("ikAnimRightFootTarget").GetValue() as Transform;
                RightToeTransform = PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.RightToes);

                Main.settings.rfOriginalPos = RightFootTransform.localPosition;

                RightFootIndicator = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, RightFootTransform.position, RightFootTransform.rotation, RightFootTransform);
                RightFootRotIndicator = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, RightFootRotTransform.position, RightFootRotTransform.rotation, RightFootRotTransform);
                RTIndicator = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, RightToeTransform.transform.position, RightToeTransform.transform.rotation, RightFootIndicator.transform);
                RTIndicator.transform.localRotation = PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.RightToes).localRotation;
            }

            CreateFeetObject(DefaultFeet, DefaultStanceSettings);
            CreateFeetObject(RidingFeet, Main.settings.RidingStanceSettings);
            CreateFeetObject(RidingSwitchFeet, Main.settings.RidingSwitchStanceSettings);
            CreateFeetObject(SetupDefaultFeet, Main.settings.SetupDefaultStanceSettings);
            CreateFeetObject(SetupNollieFeet, Main.settings.SetupNollieStanceSettings);
            CreateFeetObject(SetupSwitchFeet, Main.settings.SetupSwitchStanceSettings);
            CreateFeetObject(SetupFakieFeet, Main.settings.SetupFakieStanceSettings);
            CreateFeetObject(PowerslideFeet, Main.settings.PowerslideStanceSettings);
            CreateFeetObject(PrimoFeet, Main.settings.PrimoStanceSettings);
            CreateFeetObject(PrimoSetupDefaultFeet, Main.settings.PrimoSetupDefaultStanceSettings);
            CreateFeetObject(PrimoSetupNollieFeet, Main.settings.PrimoSetupNollieStanceSettings);
            CreateFeetObject(PushingDefaultFeet, Main.settings.PushDefaultStanceSettings);
            CreateFeetObject(PushingMongoFeet, Main.settings.PushMongoStanceSettings);
            CreateFeetObject(PushingSwitchFeet, Main.settings.PushSwitchStanceSettings);
            CreateFeetObject(PushingSwitchMongoFeet, Main.settings.PushSwitchMongoStanceSettings);
            CreateFeetObject(InAirFeet, Main.settings.InAirStanceSettings);
            CreateFeetObject(ReleaseFeet, Main.settings.ReleaseStanceSettings);
            CreateFeetObject(OnButtonFeet, Main.settings.OnButtonStanceSettings);
            CreateFeetObject(RandomFeet, Main.settings.RidingStanceSettings);
            CreateFeetObject(ManualFeet, Main.settings.ManualStanceSettings);
            CreateFeetObject(ManualSwitchFeet, Main.settings.ManualSwitchStanceSettings);
            CreateFeetObject(NoseManualFeet, Main.settings.NoseManualStanceSettings);
            CreateFeetObject(NoseManualSwitchFeet, Main.settings.NoseManualSwitchStanceSettings);
            CreateFeetObject(ManualOnButtonFeet, Main.settings.ManualOnButtonSettings);

            CreateFeetObject(BSBluntslideFeet, Main.settings.BSBluntslideStanceSettings);
            CreateFeetObject(BSBoardslideFeet, Main.settings.BSBoardslideStanceSettings);
            CreateFeetObject(BSCrookFeet, Main.settings.BSCrookStanceSettings);
            CreateFeetObject(BSFeebleFeet, Main.settings.BSFeebleStanceSettings);
            CreateFeetObject(BSFiftyFiftyFeet, Main.settings.BSFiftyFiftyStanceSettings);
            CreateFeetObject(BSFiveOFeet, Main.settings.BSFiveOStanceSettings);
            CreateFeetObject(BSLipslideFeet, Main.settings.BSLipslideStanceSettings);
            CreateFeetObject(BSLosiFeet, Main.settings.BSLosiStanceSettings);
            CreateFeetObject(BSNosebluntFeet, Main.settings.BSNosebluntStanceSettings);
            CreateFeetObject(BSNosegrindFeet, Main.settings.BSNosegrindStanceSettings);
            CreateFeetObject(BSNoseslideFeet, Main.settings.BSNoseslideStanceSettings);
            CreateFeetObject(BSOvercrookFeet, Main.settings.BSOvercrookStanceSettings);
            CreateFeetObject(BSSaladFeet, Main.settings.BSSaladStanceSettings);
            CreateFeetObject(BSSmithFeet, Main.settings.BSSmithStanceSettings);
            CreateFeetObject(BSSuskiFeet, Main.settings.BSSuskiStanceSettings);
            CreateFeetObject(BSTailslideFeet, Main.settings.BSTailslideStanceSettings);
            CreateFeetObject(BSWillyFeet, Main.settings.BSWillyStanceSettings);

            CreateFeetObject(FSBluntslideFeet, Main.settings.FSBluntslideStanceSettings);
            CreateFeetObject(FSBoardslideFeet, Main.settings.FSBoardslideStanceSettings);
            CreateFeetObject(FSCrookFeet, Main.settings.FSCrookStanceSettings);
            CreateFeetObject(FSFeebleFeet, Main.settings.FSFeebleStanceSettings);
            CreateFeetObject(FSFiftyFiftyFeet, Main.settings.FSFiftyFiftyStanceSettings);
            CreateFeetObject(FSFiveOFeet, Main.settings.FSFiveOStanceSettings);
            CreateFeetObject(FSLipslideFeet, Main.settings.FSLipslideStanceSettings);
            CreateFeetObject(FSLosiFeet, Main.settings.FSLosiStanceSettings);
            CreateFeetObject(FSNosebluntFeet, Main.settings.FSNosebluntStanceSettings);
            CreateFeetObject(FSNosegrindFeet, Main.settings.FSNosegrindStanceSettings);
            CreateFeetObject(FSNoseslideFeet, Main.settings.FSNoseslideStanceSettings);
            CreateFeetObject(FSOvercrookFeet, Main.settings.FSOvercrookStanceSettings);
            CreateFeetObject(FSSaladFeet, Main.settings.FSSaladStanceSettings);
            CreateFeetObject(FSSmithFeet, Main.settings.FSSmithStanceSettings);
            CreateFeetObject(FSSuskiFeet, Main.settings.FSSuskiStanceSettings);
            CreateFeetObject(FSTailslideFeet, Main.settings.FSTailslideStanceSettings);
            CreateFeetObject(FSWillyFeet, Main.settings.FSWillyStanceSettings);

            CreateFeetObject(GrindOnButtonFeet, Main.settings.GrindOnButtonSettings);

            CreateFeetObject(IndyFeet, Main.settings.IndyStanceSettings);
            CreateFeetObject(MelonFeet, Main.settings.MelonStanceSettings);
            CreateFeetObject(MuteFeet, Main.settings.MuteStanceSettings);
            CreateFeetObject(NosegrabFeet, Main.settings.NosegrabStanceSettings);
            CreateFeetObject(StalefishFeet, Main.settings.StalefishStanceSettings);
            CreateFeetObject(TailgrabFeet, Main.settings.TailgrabStanceSettings);

            CreateFeetObject(GrabsOnButtonSimpleFeet, Main.settings.GrabsOnButtonSimpleSettings);
            CreateFeetObject(IndyOffBoardFeet, Main.settings.IndyOffBoardStanceSettings);
            CreateFeetObject(MelonOffBoardFeet, Main.settings.MelonOffBoardStanceSettings);
            CreateFeetObject(MuteOffBoardFeet, Main.settings.MuteOffBoardStanceSettings);
            CreateFeetObject(NosegrabOffBoardFeet, Main.settings.NosegrabOffBoardStanceSettings);
            CreateFeetObject(StalefishOffBoardFeet, Main.settings.StalefishOffBoardStanceSettings);
            CreateFeetObject(TailgrabOffBoardFeet, Main.settings.TailgrabOffBoardStanceSettings);

            IsInitialized = true;
        }

        private void CreateFeetObject(CustomFeetObject feetObject, BaseStanceSettings stanceSettings)
        {
            feetObject.LeftFootPos = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, LeftFootTransform.position, LeftFootTransform.rotation, LeftFootTransform);
            feetObject.LeftFootRot = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, LeftFootRotTransform.position, LeftFootRotTransform.rotation, LeftFootRotTransform);
            feetObject.RightFootPos = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, RightFootTransform.position, RightFootTransform.rotation, RightFootTransform);
            feetObject.RightFootRot = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, RightFootRotTransform.position, RightFootRotTransform.rotation, RightFootRotTransform);

            LoadSavedPosition(feetObject.LeftFootPos, stanceSettings.lfPos);
            LoadSavedPosition(feetObject.RightFootPos, stanceSettings.rfPos);
            LoadSavedRotation(feetObject.LeftFootRot, stanceSettings.lfRot);
            LoadSavedRotation(feetObject.RightFootRot, stanceSettings.rfRot);

            feetObject.LeftToeRot = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, LeftToeTransform.transform.position, LeftToeTransform.transform.rotation, LeftFootIndicator.transform);
            feetObject.RightToeRot = Instantiate(AssetBundleHelper.SphereIndicatorPrefab, RightToeTransform.transform.position, RightToeTransform.transform.rotation, RightFootIndicator.transform);

            feetObject.LeftToeRot.transform.localRotation = PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.LeftToes).localRotation;
            feetObject.RightToeRot.transform.localRotation = PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.RightToes).localRotation;

            LoadSavedToeRotation(feetObject.LeftToeRot, stanceSettings.ltRot);
            LoadSavedToeRotation(feetObject.RightToeRot, stanceSettings.rtRot);

            feetObject.StanceSettings = stanceSettings;

            CustomFeetObjects.Add(feetObject);
        }

        private void LoadSavedPosition(GameObject foot, Vector3 targetPos)
        {
            if (targetPos == Vector3.zero)
            {
                targetPos = foot.transform.position;
            }
            else
            {
                foot.transform.localPosition = targetPos;
            }
        }

        private void LoadSavedRotation(GameObject foot, Quaternion targetRot)
        {
            if (targetRot == Quaternion.identity)
            {
                targetRot = foot.transform.localRotation;
            }
            else
            {
                foot.transform.localRotation = Quaternion.Euler(targetRot.eulerAngles);
            }
        }

        private void LoadSavedToeRotation(GameObject toe, Quaternion targetRot)
        {
            if (targetRot == Quaternion.identity)
            {
                targetRot = toe.transform.localRotation;
            }
            else
            {
                toe.transform.localRotation = Quaternion.Euler(targetRot.eulerAngles);
            }
        }

        private void Update()
        {
            if (IsInitialized)
            {
                if (IsInEditMode)
                {
                    switch (StanceUI.StanceTab)
                    {
                        case StanceTab.Riding:
                            SetActiveTarget(RidingFeet, true, false, false);
                            break;
                        case StanceTab.RidingSwitch:
                            SetActiveTarget(RidingSwitchFeet, true, false, false);
                            break;
                        case StanceTab.Setup:
                            SetActiveTarget(SetupDefaultFeet, true, false, true);
                            break;
                        case StanceTab.SetupNollie:
                            SetActiveTarget(SetupNollieFeet, true, false, true);
                            break;
                        case StanceTab.SetupSwitch:
                            SetActiveTarget(SetupSwitchFeet, true, false, true);
                            break;
                        case StanceTab.SetupFakie:
                            SetActiveTarget(SetupFakieFeet, true, false, true);
                            break;
                        case StanceTab.Powerslide:
                            SetActiveTarget(PowerslideFeet, false, false, false);
                            break;
                        case StanceTab.Primo:
                            SetActiveTarget(PrimoFeet, false, false, false);
                            break;
                        case StanceTab.PrimoSetup:
                            SetActiveTarget(PrimoSetupDefaultFeet, false, false, false);
                            break;
                        case StanceTab.PrimoSetupNollie:
                            SetActiveTarget(PrimoSetupNollieFeet, false, false, false);
                            break;
                        case StanceTab.Pushing:
                            SetActiveTarget(PushingDefaultFeet, false, false, false);
                            break;
                        case StanceTab.PushingMongo:
                            SetActiveTarget(PushingMongoFeet, false, false, false);
                            break;
                        case StanceTab.PushingSwich:
                            SetActiveTarget(PushingSwitchFeet, false, false, false);
                            break;
                        case StanceTab.PushingSwitchMongo:
                            SetActiveTarget(PushingSwitchMongoFeet, false, false, false);
                            break;
                        case StanceTab.InAir:
                            SetActiveTarget(InAirFeet, false, false, false);
                            break;
                        case StanceTab.Catch:
                            SetActiveTarget(ReleaseFeet, false, false, false);
                            break;
                        case StanceTab.OnButton:
                            SetActiveTarget(OnButtonFeet, false, true, false);
                            break;
                        case StanceTab.Grinding:
                            switch (StanceUI.GrindStanceTab)
                            {
                                case GrindStanceTab.BSBluntslide:
                                    SetActiveTarget(BSBluntslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSBoardslide:
                                    SetActiveTarget(BSBoardslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSCrook:
                                    SetActiveTarget(BSCrookFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSFeeble:
                                    SetActiveTarget(BSFeebleFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSFiftyFifty:
                                    SetActiveTarget(BSFiftyFiftyFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSFiveO:
                                    SetActiveTarget(BSFiveOFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSLipslide:
                                    SetActiveTarget(BSLipslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSLosi:
                                    SetActiveTarget(BSLosiFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSNoseblunt:
                                    SetActiveTarget(BSNosebluntFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSNosegrind:
                                    SetActiveTarget(BSNosegrindFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSNoseslide:
                                    SetActiveTarget(BSNoseslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSOvercrook:
                                    SetActiveTarget(BSOvercrookFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSSalad:
                                    SetActiveTarget(BSSaladFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSSmith:
                                    SetActiveTarget(BSSmithFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSSuski:
                                    SetActiveTarget(BSSuskiFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSTailslide:
                                    SetActiveTarget(BSTailslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.BSWilly:
                                    SetActiveTarget(BSWillyFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSBluntslide:
                                    SetActiveTarget(FSBluntslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSBoardslide:
                                    SetActiveTarget(FSBoardslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSCrook:
                                    SetActiveTarget(FSCrookFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSFeeble:
                                    SetActiveTarget(FSFeebleFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSFiftyFifty:
                                    SetActiveTarget(FSFiftyFiftyFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSFiveO:
                                    SetActiveTarget(FSFiveOFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSLipslide:
                                    SetActiveTarget(FSLipslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSLosi:
                                    SetActiveTarget(FSLosiFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSNoseblunt:
                                    SetActiveTarget(FSNosebluntFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSNosegrind:
                                    SetActiveTarget(FSNosegrindFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSNoseslide:
                                    SetActiveTarget(FSNoseslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSOvercrook:
                                    SetActiveTarget(FSOvercrookFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSSalad:
                                    SetActiveTarget(FSSaladFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSSmith:
                                    SetActiveTarget(FSSmithFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSSuski:
                                    SetActiveTarget(FSSuskiFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSTailslide:
                                    SetActiveTarget(FSTailslideFeet, false, false, false);
                                    break;
                                case GrindStanceTab.FSWilly:
                                    SetActiveTarget(FSWillyFeet, false, false, false);
                                    break;
                            }
                            break;
                        case StanceTab.Grabs:
                            switch (StanceUI.GrabStanceTab)
                            {
                                case GrabStanceTab.Indy:
                                    SetActiveTarget(IndyFeet, false, false, false);
                                    break;
                                case GrabStanceTab.Melon:
                                    SetActiveTarget(MelonFeet, false, false, false);
                                    break;
                                case GrabStanceTab.Mute:
                                    SetActiveTarget(MuteFeet, false, false, false);
                                    break;
                                case GrabStanceTab.Nosegrab:
                                    SetActiveTarget(NosegrabFeet, false, false, false);
                                    break;
                                case GrabStanceTab.Stalefish:
                                    SetActiveTarget(StalefishFeet, false, false, false);
                                    break;
                                case GrabStanceTab.Tailgrab:
                                    SetActiveTarget(TailgrabFeet, false, false, false);
                                    break;
                            }
                            break;
                        case StanceTab.GrabsOnButton:
                            switch (StanceUI.GrabOffBoardStanceTab)
                            {
                                case GrabOffBoardStanceTab.Simple:
                                    SetActiveTarget(GrabsOnButtonSimpleFeet, false, false, false);
                                    break;
                                case GrabOffBoardStanceTab.Indy:
                                    SetActiveTarget(IndyOffBoardFeet, false, false, false);
                                    break;
                                case GrabOffBoardStanceTab.Melon:
                                    SetActiveTarget(MelonOffBoardFeet, false, false, false);
                                    break;
                                case GrabOffBoardStanceTab.Mute:
                                    SetActiveTarget(MuteOffBoardFeet, false, false, false);
                                    break;
                                case GrabOffBoardStanceTab.Nosegrab:
                                    SetActiveTarget(NosegrabOffBoardFeet, false, false, false);
                                    break;
                                case GrabOffBoardStanceTab.Stalefish:
                                    SetActiveTarget(StalefishOffBoardFeet, false, false, false);
                                    break;
                                case GrabOffBoardStanceTab.Tailgrab:
                                    SetActiveTarget(TailgrabOffBoardFeet, false, false, false);
                                    break;
                            }
                            break;
                        case StanceTab.Manual:
                            SetActiveTarget(ManualFeet, false, false, false);
                            break;
                        case StanceTab.ManualSwitch:
                            SetActiveTarget(ManualSwitchFeet, false, false, false);
                            break;
                        case StanceTab.NoseManual:
                            SetActiveTarget(NoseManualFeet, false, false, false);
                            break;
                        case StanceTab.NoseManualSwitch:
                            SetActiveTarget(NoseManualSwitchFeet, false, false, false);
                            break;
                        case StanceTab.ManualOnButton:
                            SetActiveTarget(ManualOnButtonFeet, false, false, false);
                            break;
                        case StanceTab.GrindOnButton:
                            SetActiveTarget(GrindOnButtonFeet, false, false, false);
                            break;
                    }

                    LeftFootIndicator.transform.position = ActiveLeftFootTarget.transform.position;
                    RightFootIndicator.transform.position = ActiveRightFootTarget.transform.position;

                    LeftFootRotIndicator.transform.rotation = ActiveLeftFootRotTarget.transform.rotation;
                    RightFootRotIndicator.transform.rotation = ActiveRightFootRotTarget.transform.rotation;

                    LTIndicator.transform.localRotation = ActiveLeftToeRotTarget.transform.localRotation;
                    RTIndicator.transform.localRotation = ActiveRightToeRotTarget.transform.localRotation;
                }
                else
                {
                    if ((XXLController.CurrentState != CurrentState.Released && XXLController.CurrentState != CurrentState.Impact) && Main.settings.UseRandomLanding)
                    {
                        if (GetRandomNumberLeft && GetRandomNumberRight)
                        {
                            GetRandomNumberLeft = false;
                            GetRandomNumberRight = false;
                            RandomFeet.LeftFootPos.transform.localPosition = LeftFootTransform.localPosition;
                            RandomFeet.RightFootPos.transform.localPosition = RightFootTransform.localPosition;
                        }
                    }
                    UpdateFootPosition();
                }
            }
        }

        private void SetActiveTarget(CustomFeetObject _feetObject, bool _freeFootMovement, bool _onButton, bool _hasToe)
        {
            if (_feetObject.StanceSettings.Active)
            {
                ActiveLeftFootTarget = _feetObject.LeftFootPos;
                ActiveLeftFootRotTarget = _feetObject.LeftFootRot;
                ActiveRightFootTarget = _feetObject.RightFootPos;
                ActiveRightFootRotTarget = _feetObject.RightFootRot;

                if (_onButton)
                {
                    SetFreeFootMovementLeft(false, true);
                    SetFreeFootMovementRight(false, true);
                    return;
                }

                if (_freeFootMovement)
                {
                    SetFreeFootMovementLeft(true, false);
                    SetFreeFootMovementRight(true, false);
                }

                if (_hasToe)
                {
                    ActiveLeftToeRotTarget = _feetObject.LeftToeRot;
                    ActiveRightToeRotTarget = _feetObject.RightToeRot;
                }
                else
                {
                    ActiveLeftToeRotTarget = LeftToeTransform.gameObject;
                    ActiveRightToeRotTarget = RightToeTransform.gameObject;
                }
                return;
            }
            ActiveLeftFootTarget = LeftFootTransform.gameObject;
            ActiveLeftFootRotTarget = LeftFootRotTransform.gameObject;
            ActiveRightFootTarget = RightFootTransform.gameObject;
            ActiveRightFootRotTarget = RightFootRotTransform.gameObject;
            ActiveLeftToeRotTarget = LeftToeTransform.gameObject;
            ActiveRightToeRotTarget = RightToeTransform.gameObject;
        }

        public void SetFreeFootMovementLeft(bool defaultMovement, bool specialMovement)
        {
            if (defaultMovement)
            {
                Main.settings.lfFreeFootMovementSpeedXL = Main.settings.lfIKOffsetXL;
                Main.settings.lfFreeFootMovementSpeedXR = Main.settings.lfIKOffsetXR;
                Main.settings.lfFreeFootMovementSpeedZF = Main.settings.lfIKOffsetZF;
                Main.settings.lfFreeFootMovementSpeedZB = Main.settings.lfIKOffsetZB;
            }
            else if (specialMovement)
            {
                Main.settings.lfFreeFootMovementSpeedXL = Main.settings.lfIKOffsetSpecialXL;
                Main.settings.lfFreeFootMovementSpeedXR = Main.settings.lfIKOffsetSpecialXR;
                Main.settings.lfFreeFootMovementSpeedZF = Main.settings.lfIKOffsetSpecialZF;
                Main.settings.lfFreeFootMovementSpeedZB = Main.settings.lfIKOffsetSpecialZB;
            }
        }

        public void SetFreeFootMovementRight(bool defaultMovement, bool specialMovement)
        {
            if (defaultMovement)
            {
                Main.settings.rfFreeFootMovementSpeedXL = Main.settings.rfIKOffsetXL;
                Main.settings.rfFreeFootMovementSpeedXR = Main.settings.rfIKOffsetXR;
                Main.settings.rfFreeFootMovementSpeedZF = Main.settings.rfIKOffsetZF;
                Main.settings.rfFreeFootMovementSpeedZB = Main.settings.rfIKOffsetZB;
            }
            else if (specialMovement)
            {
                Main.settings.rfFreeFootMovementSpeedXL = Main.settings.rfIKOffsetSpecialXL;
                Main.settings.rfFreeFootMovementSpeedXR = Main.settings.rfIKOffsetSpecialXR;
                Main.settings.rfFreeFootMovementSpeedZF = Main.settings.rfIKOffsetSpecialZF;
                Main.settings.rfFreeFootMovementSpeedZB = Main.settings.rfIKOffsetSpecialZB;
            }
        }

        public void SaveFootPositionRotation()
        {
            SaveFeet(RidingFeet);
            SaveFeet(RidingSwitchFeet);
            SaveFeet(SetupDefaultFeet, true);
            SaveFeet(SetupNollieFeet, true);
            SaveFeet(SetupSwitchFeet, true);
            SaveFeet(SetupFakieFeet, true);
            SaveFeet(PrimoFeet);
            SaveFeet(PrimoSetupDefaultFeet);
            SaveFeet(PrimoSetupNollieFeet);
            SaveFeet(PowerslideFeet);
            SaveFeet(PushingDefaultFeet);
            SaveFeet(PushingMongoFeet);
            SaveFeet(PushingSwitchFeet);
            SaveFeet(PushingSwitchMongoFeet);
            SaveFeet(InAirFeet);
            SaveFeet(ReleaseFeet);
            SaveFeet(OnButtonFeet);
            SaveFeet(ManualFeet);
            SaveFeet(ManualSwitchFeet);
            SaveFeet(NoseManualFeet);
            SaveFeet(NoseManualSwitchFeet);
            SaveFeet(ManualOnButtonFeet);

            SaveFeet(BSBluntslideFeet);
            SaveFeet(BSBoardslideFeet);
            SaveFeet(BSCrookFeet);
            SaveFeet(BSFeebleFeet);
            SaveFeet(BSFiftyFiftyFeet);
            SaveFeet(BSFiveOFeet);
            SaveFeet(BSLipslideFeet);
            SaveFeet(BSLosiFeet);
            SaveFeet(BSNosebluntFeet);
            SaveFeet(BSNosegrindFeet);
            SaveFeet(BSNoseslideFeet);
            SaveFeet(BSOvercrookFeet);
            SaveFeet(BSSaladFeet);
            SaveFeet(BSSmithFeet);
            SaveFeet(BSSuskiFeet);
            SaveFeet(BSTailslideFeet);
            SaveFeet(BSWillyFeet);

            SaveFeet(FSBluntslideFeet);
            SaveFeet(FSBoardslideFeet);
            SaveFeet(FSCrookFeet);
            SaveFeet(FSFeebleFeet);
            SaveFeet(FSFiftyFiftyFeet);
            SaveFeet(FSFiveOFeet);
            SaveFeet(FSLipslideFeet);
            SaveFeet(FSLosiFeet);
            SaveFeet(FSNosebluntFeet);
            SaveFeet(FSNosegrindFeet);
            SaveFeet(FSNoseslideFeet);
            SaveFeet(FSOvercrookFeet);
            SaveFeet(FSSaladFeet);
            SaveFeet(FSSmithFeet);
            SaveFeet(FSSuskiFeet);
            SaveFeet(FSTailslideFeet);
            SaveFeet(FSWillyFeet);

            SaveFeet(GrindOnButtonFeet);

            SaveFeet(IndyFeet);
            SaveFeet(MelonFeet);
            SaveFeet(MuteFeet);
            SaveFeet(NosegrabFeet);
            SaveFeet(StalefishFeet);
            SaveFeet(TailgrabFeet);

            SaveFeet(GrabsOnButtonSimpleFeet);
            SaveFeet(IndyOffBoardFeet);
            SaveFeet(MelonOffBoardFeet);
            SaveFeet(MuteOffBoardFeet);
            SaveFeet(NosegrabOffBoardFeet);
            SaveFeet(StalefishOffBoardFeet);
            SaveFeet(TailgrabOffBoardFeet);
        }

        private void SaveFeet(CustomFeetObject _feetObject, bool _hasToe = false)
        {
            _feetObject.StanceSettings.lfPos = _feetObject.LeftFootPos.transform.localPosition;
            _feetObject.StanceSettings.lfRot = _feetObject.LeftFootRot.transform.localRotation;

            _feetObject.StanceSettings.rfPos = _feetObject.RightFootPos.transform.localPosition;
            _feetObject.StanceSettings.rfRot = _feetObject.RightFootRot.transform.localRotation;

            if (_hasToe)
            {
                _feetObject.StanceSettings.ltRot = _feetObject.LeftToeRot.transform.localRotation;
                _feetObject.StanceSettings.rtRot = _feetObject.RightToeRot.transform.localRotation;
            }
        }

        private void UpdateFootPosition()
        {
            switch (XXLController.CurrentState)
            {
                case CurrentState.Riding:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            SetFreeFootMovementLeft(true, false);
                            DoLeftFootRidingTransition(RidingFeet);
                        }
                        else
                        {
                            SetFreeFootMovementLeft(true, false);
                            DoLeftFootRidingTransition(RidingSwitchFeet);
                        }
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            SetFreeFootMovementRight(true, false);
                            DoRightFootRidingTransition(RidingFeet);
                        }
                        else
                        {
                            SetFreeFootMovementRight(true, false);
                            DoRightFootRidingTransition(RidingSwitchFeet);
                        }
                    }
                    break;
                case CurrentState.Pushing:
                    if(SettingsManager.Instance.stance == SkaterXL.Core.Stance.Goofy)
                    {
                        if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                        {
                            SetFreeFootMovementLeft(false, true);
                            DoLeftFootTransition(OnButtonFeet);
                        }
                        else
                        {
                            if (!PlayerController.Instance.IsSwitch)
                            {
                                if (!IsMongoPushing)
                                {
                                    SetFreeFootMovementLeft(true, false);
                                    SetLeftFootToDefault();
                                }
                                else
                                {
                                    SetFreeFootMovementLeft(true, false);
                                    DoLeftFootTransition(PushingMongoFeet);
                                }
                            }
                            else
                            {
                                if (!IsMongoPushing)
                                {
                                    SetFreeFootMovementLeft(true, false);
                                    DoLeftFootTransition(PushingSwitchFeet);
                                }
                                else
                                {
                                    SetFreeFootMovementLeft(true, false);
                                    SetLeftFootToDefault();
                                }
                            }
                        }
                        if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                        {
                            SetFreeFootMovementRight(false, true);
                            DoRightFootTransition(OnButtonFeet);
                        }
                        else
                        {
                            if (!PlayerController.Instance.IsSwitch)
                            {
                                if (!IsMongoPushing)
                                {
                                    SetFreeFootMovementRight(true, false);
                                    DoRightFootTransition(PushingDefaultFeet);
                                }
                                else
                                {
                                    SetFreeFootMovementRight(true, false);
                                    SetRightFootToDefault();
                                }
                            }
                            else
                            {
                                if (!IsMongoPushing)
                                {
                                    SetFreeFootMovementRight(true, false);
                                    SetRightFootToDefault();
                                }
                                else
                                {
                                    SetFreeFootMovementRight(true, false);
                                    DoRightFootTransition(PushingSwitchMongoFeet);
                                }
                            }
                        }
                        return;
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (!IsMongoPushing)
                            {
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(PushingDefaultFeet);
                            }
                            else
                            {
                                SetFreeFootMovementLeft(true, false);
                                SetLeftFootToDefault();
                            }
                        }
                        else
                        {
                            if (!IsMongoPushing)
                            {
                                SetFreeFootMovementLeft(true, false);
                                SetLeftFootToDefault();
                            }
                            else
                            {
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(PushingSwitchMongoFeet);
                            }
                        }
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (!IsMongoPushing)
                            {
                                SetFreeFootMovementRight(true, false);
                                SetRightFootToDefault();
                            }
                            else
                            {
                                SetFreeFootMovementRight(true, false);
                                DoRightFootTransition(PushingMongoFeet);
                            }
                        }
                        else
                        {
                            if (!IsMongoPushing)
                            {
                                SetFreeFootMovementRight(true, false);
                                DoRightFootTransition(PushingSwitchFeet);
                            }
                            else
                            {
                                SetFreeFootMovementRight(true, false);
                                SetRightFootToDefault();
                            }
                        }
                    }
                    break;
                case CurrentState.Manual:
                    if (Main.settings.ManualOneFootMode == OneFootMode.Bumper && PlayerController.Instance.inputController.player.GetButton("LB") || Main.settings.ManualOneFootMode == OneFootMode.Buttons && PlayerController.Instance.inputController.player.GetButton("X") || Main.settings.ManualOneFootMode == OneFootMode.Sticks && PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(ManualOnButtonFeet);
                    }
                    else
                    {
                        switch (XXLController.ManualType)
                        {
                            case ManualStance.Manual:
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(ManualFeet);
                                break;
                            case ManualStance.ManualSwitch:
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(ManualSwitchFeet);
                                break;
                            case ManualStance.NoseManual:
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(NoseManualFeet);
                                break;
                            case ManualStance.NoseManualSwitch:
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(NoseManualSwitchFeet);
                                break;
                        }
                    }
                    if (Main.settings.ManualOneFootMode == OneFootMode.Bumper && PlayerController.Instance.inputController.player.GetButton("RB") || Main.settings.ManualOneFootMode == OneFootMode.Buttons && PlayerController.Instance.inputController.player.GetButton("A") || Main.settings.ManualOneFootMode == OneFootMode.Sticks && PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(ManualOnButtonFeet);
                    }
                    else
                    {
                        switch (XXLController.ManualType)
                        {
                            case ManualStance.Manual:
                                SetFreeFootMovementLeft(true, false);
                                DoRightFootTransition(ManualFeet);
                                break;
                            case ManualStance.ManualSwitch:
                                SetFreeFootMovementLeft(true, false);
                                DoRightFootTransition(ManualSwitchFeet);
                                break;
                            case ManualStance.NoseManual:
                                SetFreeFootMovementLeft(true, false);
                                DoRightFootTransition(NoseManualFeet);
                                break;
                            case ManualStance.NoseManualSwitch:
                                SetFreeFootMovementLeft(true, false);
                                DoRightFootTransition(NoseManualSwitchFeet);
                                break;
                        }
                    }
                    break;
                case CurrentState.Released:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button") && Main.settings.UseSpecialInReleaseState)
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (Main.settings.UseRandomLanding)
                        {
                            SetFreeFootMovementLeft(true, false);
                            GetRandomLeftFootPosition(RandomFeet.LeftFootPos, LeftFootRotTransform.gameObject, Main.settings.ReleaseStanceSettings.lfPosSpeed, Main.settings.ReleaseStanceSettings.lfRotSpeed);
                        }
                        else
                        {
                            SetFreeFootMovementLeft(true, false);
                            DoLeftFootTransition(ReleaseFeet);
                        }
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button") && Main.settings.UseSpecialInReleaseState)
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (Main.settings.UseRandomLanding)
                        {
                            SetFreeFootMovementRight(true, false);
                            GetRandomRightFootPosition(RandomFeet.RightFootPos, RightFootRotTransform.gameObject, Main.settings.ReleaseStanceSettings.rfPosSpeed, Main.settings.ReleaseStanceSettings.rfRotSpeed);
                        }
                        else
                        {
                            SetFreeFootMovementRight(true, false);
                            DoRightFootTransition(ReleaseFeet);
                        }
                    }
                    break;
                case CurrentState.Impact:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button") && Main.settings.UseSpecialInReleaseState)
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (Main.settings.UseRandomLanding)
                        {
                            SetFreeFootMovementLeft(true, false);
                            GetRandomLeftFootPosition(RandomFeet.LeftFootPos, LeftFootRotTransform.gameObject, Main.settings.ReleaseStanceSettings.lfPosSpeed, Main.settings.ReleaseStanceSettings.lfRotSpeed);
                        }
                        else
                        {
                            SetFreeFootMovementLeft(true, false);
                            DoLeftFootTransition(ReleaseFeet);
                        }
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button") && Main.settings.UseSpecialInReleaseState)
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (Main.settings.UseRandomLanding)
                        {
                            SetFreeFootMovementRight(true, false);
                            GetRandomRightFootPosition(RandomFeet.RightFootPos, RightFootRotTransform.gameObject, Main.settings.ReleaseStanceSettings.rfPosSpeed, Main.settings.ReleaseStanceSettings.rfRotSpeed);
                        }
                        else
                        {
                            SetFreeFootMovementRight(true, false);
                            DoRightFootTransition(ReleaseFeet);
                        }
                    }
                    break;
                case CurrentState.Powerslide:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        SetFreeFootMovementLeft(true, false);
                        DoLeftFootTransition(PowerslideFeet);
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        SetFreeFootMovementRight(true, false);
                        DoRightFootTransition(PowerslideFeet);
                    }
                    break;
                case CurrentState.Setup:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (PlayerController.Instance.IsSwitch != true)
                        {
                            if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 0f)
                            {
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(SetupDefaultFeet);
                                LTIndicator.transform.localRotation = SetupDefaultFeet.LeftToeRot.transform.localRotation;
                            }
                            else if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 1f)
                            {
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(SetupNollieFeet);
                                LTIndicator.transform.localRotation = SetupNollieFeet.LeftToeRot.transform.localRotation;
                            }
                        }
                        else
                        {
                            if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 0f)
                            {
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(SetupFakieFeet);
                                LTIndicator.transform.localRotation = SetupFakieFeet.LeftToeRot.transform.localRotation;
                            }
                            else if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 1f)
                            {
                                SetFreeFootMovementLeft(true, false);
                                DoLeftFootTransition(SetupSwitchFeet);
                                LTIndicator.transform.localRotation = SetupSwitchFeet.LeftToeRot.transform.localRotation;
                            }
                        }
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 0f)
                            {
                                SetFreeFootMovementRight(true, false);
                                DoRightFootTransition(SetupDefaultFeet);
                                RTIndicator.transform.localRotation = SetupDefaultFeet.RightToeRot.transform.localRotation;
                            }
                            else if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 1f)
                            {
                                SetFreeFootMovementRight(true, false);
                                DoRightFootTransition(SetupNollieFeet);
                                RTIndicator.transform.localRotation = SetupNollieFeet.RightToeRot.transform.localRotation;
                            }
                        }
                        else
                        {
                            if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 0f)
                            {
                                SetFreeFootMovementRight(true, false);
                                DoRightFootTransition(SetupFakieFeet);
                                RTIndicator.transform.localRotation = SetupFakieFeet.RightToeRot.transform.localRotation;
                            }
                            else if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 1f)
                            {
                                SetFreeFootMovementRight(true, false);
                                DoRightFootTransition(SetupSwitchFeet);
                                RTIndicator.transform.localRotation = SetupSwitchFeet.RightToeRot.transform.localRotation;
                            }
                        }
                    }
                    break;
                case CurrentState.BeginPop:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        SetFreeFootMovementLeft(true, false);
                        SetLeftFootToDefault();
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        SetFreeFootMovementRight(true, false);
                        SetRightFootToDefault();
                    }
                    break;
                case CurrentState.Pop:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        SetFreeFootMovementLeft(true, false);
                        SetLeftFootToDefault();
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        SetFreeFootMovementRight(true, false);
                        SetRightFootToDefault();
                    }
                    break;
                case CurrentState.InAir:
                    DoLeftFootTransition(InAirFeet);
                    DoRightFootTransition(InAirFeet);
                    break;
                case CurrentState.Primo:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button") && Main.settings.UseSpecialInPrimoState)
                    {
                        SetFreeFootMovementLeft(false, true);
                        DoLeftFootTransition(OnButtonFeet);
                    }
                    SetFreeFootMovementLeft(true, false);
                    DoLeftFootTransition(PrimoFeet);
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button") && Main.settings.UseSpecialInPrimoState)
                    {
                        SetFreeFootMovementRight(false, true);
                        DoRightFootTransition(OnButtonFeet);
                    }
                    else
                    {
                        SetFreeFootMovementRight(true, false);
                        DoRightFootTransition(PrimoFeet);
                    }
                    break;
                case CurrentState.PrimoSetup:
                    if (PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 1f)
                    {
                        if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button") && Main.settings.UseSpecialInPrimoState)
                        {
                            SetFreeFootMovementLeft(false, true);
                            DoLeftFootTransition(OnButtonFeet);
                        }
                        SetFreeFootMovementLeft(true, false);
                        DoLeftFootTransition(PrimoSetupNollieFeet);
                        if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button") && Main.settings.UseSpecialInPrimoState)
                        {
                            SetFreeFootMovementRight(false, true);
                            DoRightFootTransition(OnButtonFeet);
                        }
                        else
                        {
                            SetFreeFootMovementRight(true, false);
                            DoRightFootTransition(PrimoSetupNollieFeet);
                        }
                    }
                    else
                    {
                        if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button") && Main.settings.UseSpecialInPrimoState)
                        {
                            SetFreeFootMovementLeft(false, true);
                            DoLeftFootTransition(OnButtonFeet);
                        }
                        else
                        {
                            SetFreeFootMovementLeft(true, false);
                            DoLeftFootTransition(PrimoSetupDefaultFeet);
                        }
                        if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button") && Main.settings.UseSpecialInPrimoState)
                        {
                            SetFreeFootMovementRight(false, true);
                            DoRightFootTransition(OnButtonFeet);
                        }
                        else
                        {
                            SetFreeFootMovementRight(true, false);
                            DoRightFootTransition(PrimoSetupDefaultFeet);
                        }
                    }
                    break;
            }
        }

        public void DoLeftFootplant()
        {
            LeftFootIndicator.transform.position = XXLController.LeftFootPos;
            LeftFootRotIndicator.transform.rotation = XXLController.LeftFootRot;
        }

        public void DoRightFootplant()
        {
            RightFootIndicator.transform.position = XXLController.RightFootPos;
            RightFootRotIndicator.transform.rotation = XXLController.RightFootRot;
        }

        public void SetLeftFootToDefault()
        {
            LeftFootIndicator.transform.position = Vector3.MoveTowards(LeftFootIndicator.transform.position, LeftFootTransform.transform.position, 1f * Time.deltaTime);
            LeftFootRotIndicator.transform.rotation = Quaternion.RotateTowards(LeftFootRotIndicator.transform.rotation, LeftFootRotTransform.transform.rotation, 100f * Time.deltaTime);
        }

        public void SetRightFootToDefault()
        {
            RightFootIndicator.transform.position = Vector3.MoveTowards(RightFootIndicator.transform.position, RightFootTransform.position, 1f * Time.deltaTime);
            RightFootRotIndicator.transform.rotation = Quaternion.RotateTowards(RightFootRotIndicator.transform.rotation, RightFootRotTransform.rotation, 100f * Time.deltaTime);
        }

        public void DoLeftFootTransition(CustomFeetObject _feetObject)
        {
            if (_feetObject.StanceSettings.Active)
            {
                LeftFootIndicator.transform.position = Vector3.MoveTowards(LeftFootIndicator.transform.position, _feetObject.LeftFootPos.transform.position, _feetObject.StanceSettings.lfPosSpeed * Time.deltaTime);
                LeftFootRotIndicator.transform.rotation = Quaternion.RotateTowards(LeftFootRotIndicator.transform.rotation, _feetObject.LeftFootRot.transform.rotation, _feetObject.StanceSettings.lfRotSpeed * Time.deltaTime);
                return;
            }
            SetLeftFootToDefault();
        }

        public void DoRightFootTransition(CustomFeetObject _feetObject)
        {
            if (_feetObject.StanceSettings.Active)
            {
                RightFootIndicator.transform.position = Vector3.MoveTowards(RightFootIndicator.transform.position, _feetObject.RightFootPos.transform.position, _feetObject.StanceSettings.rfPosSpeed * Time.deltaTime);
                RightFootRotIndicator.transform.rotation = Quaternion.RotateTowards(RightFootRotIndicator.transform.rotation, _feetObject.RightFootRot.transform.rotation, _feetObject.StanceSettings.rfRotSpeed * Time.deltaTime);
                return;
            }
            SetRightFootToDefault();
        }

        public void DoLeftFootRidingTransition(CustomFeetObject _feetObject)
        {
            if (_feetObject.StanceSettings.Active)
            {
                LeftFootIndicator.transform.position = Vector3.MoveTowards(LeftFootIndicator.transform.position, _feetObject.LeftFootPos.transform.position, IsRandomStance ? 0.2f * Time.deltaTime : _feetObject.StanceSettings.lfPosSpeed * Time.deltaTime);
                LeftFootRotIndicator.transform.rotation = Quaternion.RotateTowards(LeftFootRotIndicator.transform.rotation, _feetObject.LeftFootRot.transform.rotation, _feetObject.StanceSettings.lfRotSpeed * Time.deltaTime);
                return;
            }
            SetLeftFootToDefault();
        }

        public void DoRightFootRidingTransition(CustomFeetObject _feetObject)
        {
            if (_feetObject.StanceSettings.Active)
            {
                RightFootIndicator.transform.position = Vector3.MoveTowards(RightFootIndicator.transform.position, _feetObject.RightFootPos.transform.position, IsRandomStance ? 0.2f * Time.deltaTime : _feetObject.StanceSettings.rfPosSpeed * Time.deltaTime);
                RightFootRotIndicator.transform.rotation = Quaternion.RotateTowards(RightFootRotIndicator.transform.rotation, _feetObject.RightFootRot.transform.rotation, _feetObject.StanceSettings.rfRotSpeed * Time.deltaTime);
                return;
            }
            SetRightFootToDefault();
        }

        private void GetRandomLeftFootPosition(GameObject lfPos, GameObject lfRot, float lfPosSpeed, float lfRotSpeed)
        {
            if (!GetRandomNumberLeft)
            {
                Vector3 vector = lfPos.transform.localPosition;
                RandomLeftX = UnityEngine.Random.Range(-0.05f, 0.05f);
                RandomLeftZ = UnityEngine.Random.Range(-0.05f, 0.05f);
                lfPos.transform.localPosition = new Vector3(lfPos.transform.localPosition.x + RandomLeftX, lfPos.transform.localPosition.y, lfPos.transform.localPosition.z + RandomLeftZ);
                GetRandomNumberLeft = true;
            }
            LeftFootIndicator.transform.position = Vector3.MoveTowards(LeftFootIndicator.transform.position, lfPos.transform.position, 3f * Time.deltaTime);
            IsRandomStance = true;
        }

        private void GetRandomRightFootPosition(GameObject rfPos, GameObject rfRot, float rfPosSpeed, float rfRotSpeed)
        {
            if (!GetRandomNumberRight)
            {
                Vector3 vector = rfPos.transform.localPosition;

                RandomRightX = UnityEngine.Random.Range(-0.05f, 0.05f);
                RandomRightZ = UnityEngine.Random.Range(-0.05f, 0.05f);
                rfPos.transform.localPosition = new Vector3(rfPos.transform.localPosition.x + RandomRightX, rfPos.transform.localPosition.y, rfPos.transform.localPosition.z + RandomRightZ);
                GetRandomNumberRight = true;
            }
            RightFootIndicator.transform.position = Vector3.MoveTowards(RightFootIndicator.transform.position, rfPos.transform.position, 3f * Time.deltaTime);
            IsRandomStance = true;
        }
    }
}
