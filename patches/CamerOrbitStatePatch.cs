using HarmonyLib;
using UnityEngine;

namespace BetterCamera.patches;
public static class CamerOrbitStatePatch
{
    public static Unit[] Units = [null, null, null, null, null, null, null, null, null, null];
    [HarmonyPatch(typeof(CameraOrbitState), nameof(CameraOrbitState.UpdateState))]
    class UpdateState
    {
        static void Postfix(CameraStateManager cam)
        {
            if (Input.GetKey(KeyCode.LeftAlt)) {
                int? num = null ;
                //This is awful. I know.
                if (Input.GetKeyDown(KeyCode.Keypad0)) num = 0;
                if (Input.GetKeyDown(KeyCode.Keypad1)) num = 1;
                if (Input.GetKeyDown(KeyCode.Keypad2)) num = 2;
                if (Input.GetKeyDown(KeyCode.Keypad3)) num = 3;
                if (Input.GetKeyDown(KeyCode.Keypad4)) num = 4;
                if (Input.GetKeyDown(KeyCode.Keypad5)) num = 5;
                if (Input.GetKeyDown(KeyCode.Keypad6)) num = 6;
                if (Input.GetKeyDown(KeyCode.Keypad7)) num = 7;
                if (Input.GetKeyDown(KeyCode.Keypad8)) num = 8;
                if (Input.GetKeyDown(KeyCode.Keypad9)) num = 9;
                if (num != null)
                {
                    Plugin.Logger.LogMessage($"{nameof(CameraOrbitState)} xddxddxddxddxddxddxddxddxddxddxddxdd");
                    Units[(int)num] = cam.followingUnit;
                }
            }
        }
    }
}
