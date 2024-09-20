using HarmonyLib;
using Sandbox.Engine.Utils;
using System.Reflection;
using VRageMath;


namespace BadCamera
{
    [HarmonyPatch]
    public static class MyThirdPersonSpectator_RaycastOccludingObjects_Patch
    {
        static MethodBase TargetMethod()
        {
            // Access the private method 'RaycastOccludingObjects' using reflection
            return typeof(MyThirdPersonSpectator).GetMethod(
                "RaycastOccludingObjects",
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static bool Prefix(object __instance, object controlledEntity, object entitySafeObb,
            ref Vector3D raycastOrigin, ref Vector3D raycastEnd, ref Vector3D raycastSafeCameraStart,
            ref Vector3D outSafePosition, ref object __result)
        {
            // Set the outSafePosition to the desired end position
            outSafePosition = raycastEnd;

            // Force the result to be 'Ok' to indicate no occlusion
            // Since we can't access the enum, we'll set the underlying integer value
            // Assuming 'Ok' corresponds to 0

            // Use reflection to get the type of 'MyCameraRaycastResult'
            var myCameraRaycastResultType = __instance.GetType().GetNestedType("MyCameraRaycastResult", BindingFlags.NonPublic);

            // Get the 'Ok' value from the enum
            var okValue = System.Enum.ToObject(myCameraRaycastResultType, 0);

            // Set the __result to 'Ok'
            __result = okValue;

            // Skip the original method
            return false;
        }
    }
}
