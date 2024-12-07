using HarmonyLib;

namespace RestockAnywhere
{
    [HarmonyPatch]
    public class ItemPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Item), nameof(Item.IsInFreezer))]
        private static void IsInFreezer(ref bool __result)
        {
            __result = true;
        }
    }
}
