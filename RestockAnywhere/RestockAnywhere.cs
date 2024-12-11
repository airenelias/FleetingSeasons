using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace RestockAnywhere
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class RestockAnywhere : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger;

        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public static ConfigEntry<bool> PrioritizeClutter;
        public static ConfigEntry<bool> IgnoreRotten;

        private void Awake()
        {
            Logger = base.Logger;

            PrioritizeClutter = Config.Bind("General", "Prioritize Clutter", true,
                "Make employees prefer goods outside of the cellar when restocking");
            IgnoreRotten = Config.Bind("General", "Ignore Rotten", false,
                "Make employees ignore spoiled goods when restocking");

            harmony.PatchAll();

            Logger.LogInfo("Plugin " + PluginInfo.PLUGIN_NAME + " is loaded!");
        }
    }
}
