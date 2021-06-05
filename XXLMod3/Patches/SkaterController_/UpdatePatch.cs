using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using XXLMod3.PlayerStates;

namespace XXLMod3.Patches.SkaterController_
{
    [HarmonyPatch(typeof(SkaterController), "Update")]
    class UpdatePatch
    {
        static bool deckFXFound;
        static float truckTightness = 1f;
        static void Postfix()
        {
            if (Main.enabled)
            {
                if (!PlayerController.Instance.playerSM.IsInState(typeof(Custom_Manualling)))
                {
                    //if (!deckFXFound)
                    //{
                    //    var deckFX = UnityModManager.modEntries.FirstOrDefault(x => x.Info.DisplayName == "DeckFX");
                    //    if (deckFX == null)
                    //    {
                    //        return;
                    //    }
                    //    var deckFXAssembly = deckFX.Assembly;
                    //    var deckFXSettings = Traverse.Create(deckFXAssembly).Field("settings");
                    //    truckTightness = Traverse.Create(deckFXSettings).Field("truckTightness").GetValue<float>();
                    //    deckFXFound = true;
                    //}
                    JointDrive angularXDrive = default(JointDrive);
                    float num = 1f;
                    float num2 = 0.02f;
                    num *= truckTightness + 0.25f;
                    num2 *= (truckTightness + 0.25f) * 2f;
                    bool flag2 = PlayerController.Instance.inputController.player.GetAxis("LT") < 0.1f && PlayerController.Instance.inputController.player.GetAxis("RT") < 0.1f;
                    bool flag3 = flag2;
                    if (flag3)
                    {
                        num = 0.75f;
                        num2 = 0.02f;
                    }
                    angularXDrive.positionSpring = num;
                    angularXDrive.positionDamper = num2;
                    angularXDrive.maximumForce = 3.402823E+23f;
                    PlayerController.Instance.boardController.backTruckJoint.angularXDrive = angularXDrive;
                    PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = angularXDrive;
                }
            }
        }
    }
}