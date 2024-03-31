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
    }


}
