using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RestockAnywhere
{
    [HarmonyPatch]
    public class RestockerWaitPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RestockerWait), "FindItemToRestock")]
        private static bool FindItemToRestock(RestockerController ___rc, ref bool __result)
        {
            // get list of restock targets
            foreach (Item targetItem in ((IEnumerable<Item>)UnityEngine.Object.FindObjectsOfType<Item>()).Where<Item>((Func<Item, bool>)(x => x.onStand.Value && x.amount.Value <= 0 && !x.IsRestockTarget())).OrderBy<Item, float>((Func<Item, float>)(go => Vector3.Distance(go.transform.position, ___rc.transform.position))).ToList<Item>())
            {
                RestockAnywhere.Logger.LogDebug("Found restock target '" + targetItem.itemSO.itemName + "' amount " + targetItem.amount.Value + " at " + targetItem.transform.position.ToString());
                // get list of restock sources skip rotten items if condition met
                List<Item> list = ((IEnumerable<Item>)UnityEngine.Object.FindObjectsOfType<Item>()).Where<Item>((Func<Item, bool>)(x => (UnityEngine.Object)x.itemSO == (UnityEngine.Object)targetItem.itemSO && (!RestockAnywhere.IgnoreRotten.Value || !x.IsExpired()) && x.amount.Value > 0 && !x.IsRestockSource())).OrderBy<Item, float>((Func<Item, float>)(go => Vector3.Distance(go.transform.position, ___rc.transform.position))).ToList<Item>();
                if (list.Count > 0)
                {
                    // look for items outside of cellar if condition met
                    if (RestockAnywhere.PrioritizeClutter.Value)
                    {
                        Item sourceItem = list.FirstOrDefault(x => !x.IsInFreezer());
                        if (sourceItem != default)
                        {
                            ___rc.SetItems(targetItem, sourceItem);
                            __result = true;
                            RestockAnywhere.Logger.LogDebug("Found outside restock source '" + sourceItem.itemSO.itemName + "' amount " + sourceItem.amount.Value + " at " + sourceItem.transform.position.ToString());
                            return false;
                        }
                    }
                    // get closest if none was found
                    ___rc.SetItems(targetItem, list[0]);
                    __result = true;
                    RestockAnywhere.Logger.LogDebug("Found closest restock source '" + list[0].itemSO.itemName + "' amount " + list[0].amount.Value + " at " + list[0].transform.position.ToString());
                    return false;
                }
            }
            __result = false;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RestockerTakeSource), "OnUpdate")]
        private static void OnUpdate(RestockerController ___rc)
        {
            if (___rc.sourceItem == null)
            {
                RestockAnywhere.Logger.LogDebug("Failed to reach restock source, clearing item states");
                ___rc.targetItem.SetRestockTarget(false);
                ___rc.sourceItem.SetRestockSource(false);
            }
        }
    }
}
