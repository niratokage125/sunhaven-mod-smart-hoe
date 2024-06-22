using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Wish;

namespace SmartHoe
{
    [HarmonyPatch(typeof(Hoe))]
    public static class HoePatch
    {
        [HarmonyPatch(nameof(Hoe.Use2)), HarmonyPrefix]
        public static bool Use2_Prefix(Vector2Int ___pos)
        {
            if (!Plugin.modEnabled.Value || !Plugin.hoeEnabled.Value)
            {
                return true;
            }

            if (!SingletonBehaviour<TileManager>.Instance.IsFarmOrHoedOrWatered(___pos))
            {
                return false;
            }

            return true;
        }

        [HarmonyPatch("LateUpdate"), HarmonyPrefix, HarmonyPriority(Priority.VeryLow)]
        public static bool LateUpdate_Prefix(Hoe __instance)
        {
            if (!Plugin.modEnabled.Value || !Plugin.hoeEnabled.Value || !Plugin.classicHoe.Value)
            {
                return true;
            }
            MyLateUpdate(__instance);
            return false;
        }

        [HarmonyPatch("LateUpdate"), HarmonyReversePatch]
        public static void MyLateUpdate(object instance)
        {
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                for (int i = 0; i + 2 < code.Count; i++)
                {
                    if (code[i].opcode == OpCodes.Ldfld &&
                        code[i].OperandIs(AccessTools.Field(typeof(Hoe), "timeLastHoed")) &&
                        code[i + 2].OperandIs(1.5f))
                    {
                        code[i + 2].operand = -1f;
                        Plugin.logger.LogInfo("Patched Hoe.LateUpdate");
                    }
                }

                return code;
            }
            _ = Transpiler(null);

        }
    }
}
