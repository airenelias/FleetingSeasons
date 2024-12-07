using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace RestockAnywhere
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class RestockAnywhere : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger;

        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            Logger = base.Logger;
            harmony.PatchAll();

            Logger.LogInfo("Plugin RestockAnywhere is loaded!");
        }
    }
}
