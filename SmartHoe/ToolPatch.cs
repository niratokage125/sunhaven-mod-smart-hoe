using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Wish;

namespace SmartHoe
{
    [HarmonyPatch(typeof(Tool))]
    public static class ToolPatch
    {
        [HarmonyPatch(nameof(Tool.Use1)), HarmonyPrefix]
        public static bool Use1_Prefix(Tool __instance, Vector2Int ___pos)
        {
            if (!Plugin.modEnabled.Value)
            {
                return true;
            }

            if (Plugin.hoeEnabled.Value && __instance is Hoe) 
            { 
                if (Plugin.dontHoeTileEnabled.Value && SingletonBehaviour<TileManager>.Instance.HasTile(___pos, ScenePortalManager.ActiveSceneIndex))
                {
                    return false;
                }
                if (!(SingletonBehaviour<TileManager>.Instance.IsHoeable(___pos) || SingletonBehaviour<TileManager>.Instance.IsFarmable(___pos)))
                {
                    return false;
                }
            }

            if (Plugin.pickaxeEnabled.Value && __instance is Pickaxe pickaxe)
            {
                var traverse = Traverse.Create(pickaxe);
                var _currentDecoration = traverse.Field<Decoration>("_currentDecoration").Value;
                var currentSpot = traverse.Field<Vector2Int>("currentSpot").Value;
                var position = currentSpot / 6;
                var tileInfo = SingletonBehaviour<TileManager>.Instance.GetTileInfo(position);
                if (_currentDecoration == null && !(tileInfo != TileInfo.Blank))
                {
                    return false;
                }
            }

            if (Plugin.axeEnabled.Value && __instance is Axe axe)
            {
                var traverse = Traverse.Create(axe);
                var _currentTree = traverse.Field<Decoration>("_currentTree").Value;
                if (_currentTree == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
