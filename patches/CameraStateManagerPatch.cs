using HarmonyLib;
using UnityEngine;

namespace BetterCamera.patches;
static class CameraStateManagerPatch
{
    [HarmonyPatch(typeof(CameraStateManager), "Update")]
    class Update()
    {
        static void Postfix()
        {
            Unit unit = null;
            if (Input.GetKeyDown(KeyCode.Keypad0)) unit = CamerOrbitStatePatch.Units[0];
            else if (Input.GetKeyDown(KeyCode.Keypad1)) unit = CamerOrbitStatePatch.Units[1];
            else if (Input.GetKeyDown(KeyCode.Keypad2)) unit = CamerOrbitStatePatch.Units[2];
            else if (Input.GetKeyDown(KeyCode.Keypad3)) unit = CamerOrbitStatePatch.Units[3];
            else if (Input.GetKeyDown(KeyCode.Keypad4)) unit = CamerOrbitStatePatch.Units[4];
            else if (Input.GetKeyDown(KeyCode.Keypad5)) unit = CamerOrbitStatePatch.Units[5];
            else if (Input.GetKeyDown(KeyCode.Keypad6)) unit = CamerOrbitStatePatch.Units[6];
            else if (Input.GetKeyDown(KeyCode.Keypad7)) unit = CamerOrbitStatePatch.Units[7];
            else if (Input.GetKeyDown(KeyCode.Keypad8)) unit = CamerOrbitStatePatch.Units[8];
            else if (Input.GetKeyDown(KeyCode.Keypad9)) unit = CamerOrbitStatePatch.Units[9];
            if (unit != null) CameraStateManager.i.SetFollowingUnit(unit); 
        }
    }
}
