using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    }
}
