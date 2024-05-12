using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Wish;

namespace SmartHoe
{
    [HarmonyPatch(typeof(Weapon))]
    public static class WeaponPatch
    {
        private static FloatRef moveSpeedWhileHoeingRef;
        [HarmonyPatch("Attack"), HarmonyPostfix]
        public static void Attack_Postfix(Weapon __instance, bool __result)
        {
            var hoe = __instance as Hoe;
            if (hoe == null || !__result)
            {
                return;
            }
            var traverse = Traverse.Create(hoe);
            var player = traverse.Field<Player>("player").Value;
            if (Plugin.modEnabled.Value && Plugin.hoeEnabled.Value && moveSpeedWhileHoeingRef == null)
            {
                moveSpeedWhileHoeingRef = new FloatRef { value = Plugin.moveSpeedWhileHoeing.Value };
                player.moveSpeedMultipliers.Add(moveSpeedWhileHoeingRef);
                return;
            }
        }
        [HarmonyPatch("FinishAttackAnimation"), HarmonyPostfix]
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
