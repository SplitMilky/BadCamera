using HarmonyLib;
using Sandbox.Engine.Utils;
using VRage.Game.Components;
using VRageMath;
using Sandbox.Game.Entities;
using VRage.Game;
using System;
using System.Reflection;
using VRage.Game.Entity;

namespace BadCamera
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class NoCameraCollisionMod : MySessionComponentBase
    {
        private bool _initialized = false;

        public override void LoadData()
        {
            if (_initialized)
                return;

            var harmony = new Harmony("com.yourname.nocameracollision");
            harmony.PatchAll();

            _initialized = true;
        }
    }

    [HarmonyPatch]
    public static class MyThirdPersonSpectatorPatch
    {
        // Prefix method matching the target method's signature exactly
        static bool Prefix(
            ref object __result,
            MyEntity controlledEntity,
            MyOrientedBoundingBoxD entitySafeObb,
            ref Vector3D raycastOrigin,
            ref Vector3D raycastEnd,
            ref Vector3D raycastSafeCameraStart,
            ref Vector3D outSafePosition)
        {
            var myThirdPersonSpectatorType = typeof(MyThirdPersonSpectator);
            var myCameraRaycastResultType = myThirdPersonSpectatorType.GetNestedType("MyCameraRaycastResult", BindingFlags.NonPublic);

            var okValue = Enum.Parse(myCameraRaycastResultType, "Ok");

            // Set the result to 'Ok', indicating no occluders were found
            __result = okValue;

            // Set 'outSafePosition' to the desired position
            outSafePosition = raycastEnd;

            // Skip the original method
            return false;
        }
    }
}
