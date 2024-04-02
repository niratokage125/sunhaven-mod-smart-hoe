using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using Wish;

namespace SmartHoe
{
    [HarmonyPatch]
    public static class Patch
    {
        private static FloatRef moveSpeedWhileHoeingRef;

        [HarmonyPatch(typeof(Tool),nameof(Tool.Use1)), HarmonyPrefix]
        public static bool Use1_Prefix(Tool __instance, Vector2Int ___pos)
        {
            if (!Plugin.modEnabled.Value || !(__instance is Hoe))
            {
                return true;
            }

            if (Plugin.dontHoeTileEnabled.Value && SingletonBehaviour<TileManager>.Instance.HasTile(___pos, ScenePortalManager.ActiveSceneIndex))
            {
                return false;
            }
            if (!(SingletonBehaviour<TileManager>.Instance.IsHoeable(___pos) || SingletonBehaviour<TileManager>.Instance.IsFarmable(___pos)))
            {
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Hoe), nameof(Hoe.Use2)), HarmonyPrefix]
        public static bool Use2_Prefix(Vector2Int ___pos)
        {
            if (!Plugin.modEnabled.Value)
            {
                return true;
            }

            if (!SingletonBehaviour<TileManager>.Instance.IsFarmOrHoedOrWatered(___pos))
            {
                return false;
            }

            return true;
        }


        [HarmonyPatch(typeof(Weapon), "Attack"), HarmonyPostfix]
        public static void Attack_Postfix(Weapon __instance, bool __result)
        {
            var hoe = __instance as Hoe;
            if (hoe == null || !__result)
            {
                return;
            }
            var traverse = Traverse.Create(hoe);
            var player = traverse.Field<Player>("player").Value;
            if (Plugin.modEnabled.Value && moveSpeedWhileHoeingRef == null)
            {
                moveSpeedWhileHoeingRef = new FloatRef { value = Plugin.moveSpeedWhileHoeing.Value };
                player.moveSpeedMultipliers.Add(moveSpeedWhileHoeingRef);
                return;
            }
        }
        [HarmonyPatch(typeof(Weapon), "FinishAttackAnimation"), HarmonyPostfix]
        public static void FinishAttackAnimation_Postfix(Weapon __instance)
        {
            var hoe = __instance as Hoe;
            if (hoe == null)
            {
                return;
            }
            var traverse = Traverse.Create(hoe);
            var player = traverse.Field<Player>("player").Value;
            if (moveSpeedWhileHoeingRef != null)
            {
                player.moveSpeedMultipliers.Remove(moveSpeedWhileHoeingRef);
                moveSpeedWhileHoeingRef = null;
                return;
            }
        }
    }


}
