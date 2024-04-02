using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace SmartHoe
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> dontHoeTileEnabled;
        public static ConfigEntry<Single> moveSpeedWhileHoeing;
        private const string PluginGuid = "niratokage125.sunhaven.SmartHoe";
        private const string PluginName = "SmartHoe";
        private const string PluginVer = "1.0.1";
        private void Awake()
        {
            logger = Logger;
            modEnabled = Config.Bind<bool>("General", "Mod Enabled", true, "Set to false to disable this mod.");
            dontHoeTileEnabled = Config.Bind<bool>("General", "Do Not Hoe Tiles", false, "If enabled, floor tiles are no longer hoed by tools. (like Earthquake)");
            moveSpeedWhileHoeing = Config.Bind<Single>("General", "Move Speed While Hoeing", 0.5f, new ConfigDescription("Description", new AcceptableValueRange<Single>(0.0001f, 1f)));
            var harmony = new Harmony(PluginGuid);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo($"Plugin {PluginGuid} v{PluginVer} is loaded");
        }
    }
}
