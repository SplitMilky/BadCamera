﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Sandbox.Engine.Utils;

namespace Camera.Zoom
{
	[HarmonyPatch]
	public class CameraPatches
	{
		static CameraPatches()
		{
			characterSpring = AccessTools.Field(typeof(MyThirdPersonSpectator), "NormalSpringCharacter");
			normalSpring = AccessTools.Field(typeof(MyThirdPersonSpectator), "NormalSpring");
		}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(MyThirdPersonSpectator), "ComputeEntitySafeOBB")]
		public static IEnumerable<CodeInstruction> ComputeEntitySafeOBBTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = instructions.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Brfalse)
				{
					list[i].opcode = OpCodes.Br;
					break;
				}
			}
			foreach (CodeInstruction codeInstruction in list)
			{
				yield return codeInstruction;
			}
		}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(MyThirdPersonSpectator), "FindSafeStart")]
		public static IEnumerable<CodeInstruction> FindSafeStartTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = instructions.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Brfalse)
				{
					list[i].opcode = OpCodes.Brtrue;
					break;
				}
			}
			foreach (CodeInstruction codeInstruction in list)
			{
				yield return codeInstruction;
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

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(MyThirdPersonSpectator), "RaycastOccludingObjects")]
		public static IEnumerable<CodeInstruction> RaycastOccludingObjectsTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			yield return new CodeInstruction(OpCodes.Ldc_I4_0, null);
			yield return new CodeInstruction(OpCodes.Ret, null);
			yield break;
		}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(MyThirdPersonSpectator), "RecalibrateCameraPosition")]
		public static IEnumerable<CodeInstruction> RecalibrateCameraPositionTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction codeInstruction in instructions)
			{
				if (codeInstruction.Calls(AccessTools.Method("VRageMath.BoundingBox:Inflate", new Type[]
				{
					typeof(float)
				}, null)))
				{
					yield return new CodeInstruction(OpCodes.Pop, null);
					yield return new CodeInstruction(OpCodes.Pop, null);
				}
				else
				{
					yield return codeInstruction;
				}
			}
		}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(MyThirdPersonSpectator), "Update")]
		public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction codeInstruction in instructions)
			{
				if (codeInstruction.LoadsField(normalSpring, false))
				{
					yield return new CodeInstruction(OpCodes.Ldfld, characterSpring);
				}
				else
				{
					yield return codeInstruction;
				}
			}
		}

		private static FieldInfo characterSpring;
		private static FieldInfo normalSpring;
	}
}
