using System.Collections.Generic;
using HarmonyLib;
using Sandbox.Engine.Utils;
using VRageMath;
using System.Reflection;

namespace Camera.Zoom
{
    [HarmonyPatch]
    public class CameraPatches
    {
        static CameraPatches()
        {
            characterSpring = AccessTools.Field(typeof(MyThirdPersonSpectator), "NormalSpringCharacter");
            normalSpring = AccessTools.Field(typeof(MyThirdPersonSpectator), "NormalSpring");
            lookAt = AccessTools.Field(typeof(MyThirdPersonSpectator), "m_lookAt");
            clampedlookAt = AccessTools.Field(typeof(MyThirdPersonSpectator), "m_clampedlookAt");
        }

        private static FieldInfo characterSpring;
        private static FieldInfo normalSpring;
        private static FieldInfo lookAt;
        private static FieldInfo clampedlookAt;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MyThirdPersonSpectator), "Update")]
        public static void UpdatePrefix(MyThirdPersonSpectator __instance)
        {
		    if (__instance == null)
            {
                return; // Early exit if __instance is null
            }

            // Custom logic to prevent camera zoom-in
            // For example, you can adjust the camera position or ignore certain collisions
            PropertyInfo cameraPositionProperty = __instance.GetType().GetProperty("CameraPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo targetPositionProperty = __instance.GetType().GetProperty("TargetPosition", BindingFlags.NonPublic | BindingFlags.Instance);

            if (cameraPositionProperty == null || targetPositionProperty == null)
            {
                return; // Early exit if properties are not found
            }

            Vector3D cameraPosition = cameraPositionProperty.GetValue(__instance) as Vector3D? ?? Vector3D.Zero;
            Vector3D targetPosition = targetPositionProperty.GetValue(__instance) as Vector3D? ?? Vector3D.Zero;

            // Example: Prevent camera from zooming in if the distance to the target is less than a threshold
            double minDistance = 5.0; // Adjust this value as needed
            if (Vector3D.Distance(cameraPosition, targetPosition) < minDistance)
            {
                // Adjust the camera position to maintain the minimum distance
                Vector3D direction = Vector3D.Normalize(cameraPosition - targetPosition);
                cameraPositionProperty.SetValue(__instance, targetPosition + direction * minDistance);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MyThirdPersonSpectator), "IsCameraForced")]
        public static bool IsCameraForced(ref bool __result)
        {
            __result = false;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MyThirdPersonSpectator), "IsEntityFiltered")]
        public static bool IsEntityFiltered(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}