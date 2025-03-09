using HarmonyLib;
using NuclearOption.MissionEditorScripts;
using UnityEngine;

namespace BetterCamera.patches;

public static class CameraFreeStatePatch
{
    static GameObject cameraTarget = new GameObject();
    static bool verticalCameraAbsolute = true;

    [HarmonyPatch(typeof(CameraFreeState), nameof(CameraFreeState.EnterState))]
    class EnterState
    {
        static void Postfix(CameraStateManager cam)
        {
            cameraTarget.transform.forward = cam.transform.forward;
            cameraTarget.transform.up = cam.transform.up;
            cameraTarget.transform.right = cam.transform.right;
        }
    }

    [HarmonyPatch(typeof(CameraFreeState), nameof(CameraFreeState.UpdateState))]
    class UpdateState
    {
        static bool Prefix(ref CameraStateManager cam, ref float ___stateTime, ref float ___desiredFOV, ref RaycastHit ___hit, ref Vector3 ___mousePosPrev)
        {
            ___stateTime += Time.deltaTime;
            cam.windNoiseExternal.volume = 0f;
            if (Input.GetKey(KeyCode.PageUp))
            {
                ___desiredFOV -= 25f * Time.unscaledDeltaTime;
            }
            if (Input.GetKey(KeyCode.PageDown))
            {
                ___desiredFOV += 25f * Time.unscaledDeltaTime;
            }
            if (Input.GetKeyDown(KeyCode.CapsLock))
            {
                verticalCameraAbsolute = !verticalCameraAbsolute;
            }
            ___desiredFOV = Mathf.Clamp(___desiredFOV, 5f, 90f);
            cam.mainCamera.fieldOfView = Mathf.Lerp(cam.mainCamera.fieldOfView, ___desiredFOV, Time.unscaledDeltaTime);
            if (Input.GetMouseButton(1))
            {
                CursorManager.Refresh();
            }
            if (!InputFieldChecker.InsideInputField)
            {
                float num = GameManager.playerInput.GetAxis("Pan View") * 0.3f * PlayerSettings.viewSensitivity * (float)(PlayerSettings.viewInvertPitch ? -1 : 1);
                float num2 = GameManager.playerInput.GetAxis("Tilt View") * 0.3f * PlayerSettings.viewSensitivity;
                float numr = 0f;
                if (Input.GetKey(KeyCode.Q))
                {
                    numr = 3f * PlayerSettings.viewSensitivity;
                }
                if (Input.GetKey(KeyCode.E))
                {
                    numr = -3f * PlayerSettings.viewSensitivity;
                }
                float num3 = 3f * PlayerSettings.viewSensitivity;
                if (!GameManager.playerInput.GetButton("Free Look"))
                {
                    num = 0f;
                    num2 = 0f;
                    num3 = 0f;
                    numr = 0f;
                }
                ___mousePosPrev = Input.mousePosition;
                float d = 300f;
                if (Input.GetKey("left shift"))
                {
                    d = 3000f;
                }
                var up = verticalCameraAbsolute ? Vector3.up : cam.transform.up;
                if (Input.GetKey(KeyCode.Space))
                {
                    cam.cameraVelocity += d * up * Time.unscaledDeltaTime;
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    cam.cameraVelocity += d * -up * Time.unscaledDeltaTime;
                }
                float axis = GameManager.playerInput.GetAxis("Move Longitudinal");
                float axis2 = GameManager.playerInput.GetAxis("Move Lateral");
                cam.cameraVelocity += d * cam.transform.forward * axis * Time.unscaledDeltaTime;
                cam.cameraVelocity += d * cam.transform.right * axis2 * Time.unscaledDeltaTime;
                cameraTarget.transform.transform.Rotate(num2, num, numr);
            }
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, cameraTarget.transform.transform.rotation, Mathf.Min(2f * Time.unscaledDeltaTime / PlayerSettings.viewSmoothing, 1f));
            cam.transform.position += cam.cameraVelocity * 0.02f;
            if ((MissionEditor.i == null || !MissionEditor.i.allowCameraClip) && Physics.Linecast(cam.transform.position + Vector3.up * 5000f, cam.transform.position - Vector3.up * 5000f, out ___hit, 64))
            {
                cam.transform.position = new Vector3(cam.transform.position.x, Mathf.Max(cam.transform.position.y, ___hit.point.y + 1.7f), cam.transform.position.z);
            }
            Vector3 position = cam.transform.position;
            float num4 = Datum.LocalSeaY + 1.7f;
            if (position.y < num4)
            {
                position.y = num4;
                cam.transform.position = position;
            }
            cam.cameraVelocity *= 0.96f;
            if (GameManager.LocalPlayer == null)
            {
                return false;
            }
            GameManager.LocalPlayer.transform.position = cam.transform.position;
            GameManager.LocalPlayer.transform.rotation = cam.transform.rotation;
            return false;
        }
    }
}
