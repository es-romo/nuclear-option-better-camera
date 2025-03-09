using HarmonyLib;
using NuclearOption.MissionEditorScripts;
using System;
using UnityEngine;

namespace BetterCamera.patches;

public static class CameraFreeStatePatch
{
    static Transform targetTransform;

    [HarmonyPatch(typeof(CameraFreeState), nameof(CameraFreeState.EnterState))]
    class EnterState
    {
        static void Postfix(CameraStateManager cam)
        {
            targetTransform = (new GameObject()).transform;
            targetTransform.forward = cam.transform.forward;
            targetTransform.up = cam.transform.up;
            targetTransform.right = cam.transform.right;
        }
    }

    [HarmonyPatch(typeof(CameraFreeState), nameof(CameraFreeState.LeaveState))]
    class LeaveState
    {
        static void Postfix(CameraStateManager cam)
        {
        }
    }

    [HarmonyPatch(typeof(CameraFreeState), nameof(CameraFreeState.UpdateState))]
    class UpdateState
    {
        static bool Prefix(ref CameraStateManager cam, CameraFreeState __instance, ref float ___stateTime, ref float ___desiredFOV, ref RaycastHit ___hit, ref float ___tiltView, ref float ___panView, ref Vector3 ___mousePosPrev, ref bool ___trackTarget)
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
                if (Mathf.Abs(num) > 0f || Mathf.Abs(num2) > 0f || Mathf.Abs(numr) > 0f)
                {
                    ___tiltView += num2;
                    ___panView += num;
                    ___trackTarget = false;

                }
                float d = 300f;
                if (Input.GetKey("left shift"))
                {
                    d = 3000f;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    cam.cameraVelocity += d * Vector3.up * Time.unscaledDeltaTime;
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    cam.cameraVelocity += d * Vector3.down * Time.unscaledDeltaTime;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    cam.cameraVelocity += d * Vector3.up * Time.unscaledDeltaTime;
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    cam.cameraVelocity += d * Vector3.down * Time.unscaledDeltaTime;
                }

                float axis = GameManager.playerInput.GetAxis("Move Longitudinal");
                float axis2 = GameManager.playerInput.GetAxis("Move Lateral");
                cam.cameraVelocity += d * cam.transform.forward * axis * Time.unscaledDeltaTime;
                cam.cameraVelocity += d * cam.transform.right * axis2 * Time.unscaledDeltaTime;
                targetTransform.transform.Rotate(num2, num, numr);
            }
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetTransform.transform.rotation, Mathf.Min(2f * Time.unscaledDeltaTime / PlayerSettings.viewSmoothing, 1f));
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
