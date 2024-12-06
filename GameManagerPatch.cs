using HarmonyLib;
using System;
using Unity.Netcode;
using UnityEngine;

namespace FleetingSeasons
{
    [HarmonyPatch]
    public class GameManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.GetCurrentSeason))]
        private static void GetCurrentSeason(GameManager __instance, ref SeasonSO __result)
        {
            NetworkVariable<int> day = Traverse.Create(__instance).Field("day").GetValue() as NetworkVariable<int>;
            __result = __instance.seasons[(day.Value - 1) / FleetingSeasons.DaysInSeason % 4];
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.GetSeasonOnDay))]
        private static void GetSeasonOnDay(GameManager __instance, int day, ref SeasonSO __result)
        {
            __result = __instance.seasons[(day - 1) / FleetingSeasons.DaysInSeason % 4];
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.GetNormalizedDay))]
        private static void GetNormalizedDay(int day, ref int __result)
        {
            int num = day % FleetingSeasons.DaysInSeason;
            __result = num != 0 ? num : FleetingSeasons.DaysInSeason;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.GetCurrentDay))]
        private static void GetCurrentDay(GameManager __instance, ref int __result)
        {
            NetworkVariable<int> day = Traverse.Create(__instance).Field("day").GetValue() as NetworkVariable<int>;
            int num = day.Value % FleetingSeasons.DaysInSeason;
            __result = num != 0 ? num : FleetingSeasons.DaysInSeason;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.GetCurrentYear))]
        private static void GetCurrentYear(GameManager __instance, ref int __result)
        {
            NetworkVariable<int> day = Traverse.Create(__instance).Field("day").GetValue() as NetworkVariable<int>;
            __result = (day.Value - 1) / FleetingSeasons.DaysInSeason * 4 + 1;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), "Start")]
        private static void Start(GameManager __instance)
        {
            // adjust calendar UI on every load
            UIManager.Instance.calendarDates.RemoveRange(FleetingSeasons.DaysInSeason, UIManager.Instance.calendarDates.Count - FleetingSeasons.DaysInSeason);
            var calendarContent = UIManager.Instance.panelCalendar.transform.GetChild(1).GetComponent<RectTransform>();
            var calendarDates = calendarContent.transform.GetChild(1).GetComponent<RectTransform>();
            for (int dayIdx = FleetingSeasons.DaysInSeason; dayIdx < calendarDates.childCount; dayIdx++)
            {
                calendarDates.GetChild(dayIdx).gameObject.SetActive(false);
            }

            // adjust events schedule once per game launch
            if (FleetingSeasons.Enabled)
            {
                return;
            }

            foreach (EventSO currentEvent in __instance.events)
            {
                int eventLength = currentEvent.days.Count;
                int modifiedEventLength = (int)Math.Ceiling(eventLength * FleetingSeasons.SeasonLengthMultiplier.Value);
                if (eventLength > modifiedEventLength)
                {
                    currentEvent.days.RemoveRange(modifiedEventLength, eventLength - modifiedEventLength);
                }
                currentEvent.days[0] = (int)Math.Ceiling(currentEvent.days[0] * FleetingSeasons.SeasonLengthMultiplier.Value);
                for (int dayIdx = 1; dayIdx < modifiedEventLength; dayIdx++)
                {
                    currentEvent.days[dayIdx] = currentEvent.days[dayIdx - 1] + 1;
                }
            }

            FleetingSeasons.Enabled = true;
            FleetingSeasons.Logger.LogInfo("Season length was set to " + FleetingSeasons.DaysInSeason + " days");
        }
    }
}
