using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace FleetingSeasons
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class FleetingSeasons : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger;

        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public static bool Enabled = false;

        public static int DaysInSeason = 28; // default amount of days in season
        public static ConfigEntry<double> SeasonLengthMultiplier;

        private void Awake()
        {
            Logger = base.Logger;
            SeasonLengthMultiplier = Config.Bind("General", "Season Length", 0.5,
                "Season length multiplier (e.g. 1 is full 28 days 4 weeks per season, 0.5 is 2 weeks, 0.75 is 3 weeks out of 4)");
            if (SeasonLengthMultiplier.Value > 1)
            {
                Logger.LogWarning("Season length modifier can't be set higher than 1");
                SeasonLengthMultiplier.Value = 1;
            }
            DaysInSeason = (int)Math.Ceiling(DaysInSeason * SeasonLengthMultiplier.Value);

            harmony.PatchAll();

            Logger.LogInfo("Plugin FleetingSeasons is loaded!");
        }
    }
}
