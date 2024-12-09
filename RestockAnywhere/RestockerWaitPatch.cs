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
                            return false;
                        }
                    }
                    // get closest if none was found
                    ___rc.SetItems(targetItem, list[0]);
                    __result = true;
                    return false;
                }
            }
            __result = false;
            return false;
        }
    }
}
