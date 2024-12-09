using HarmonyLib;
using UnityEngine;

namespace FleetingSeasons
{
    [HarmonyPatch]
    public class UIManagerPatch
    {
        private static Vector3 CalendarDateDefaultScale, CalendarDateFocusedScale;
        private static Color CalendarDateDefaultColor, CalendarDateFocusedColor;

        private static bool CalendarDateFocused = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.ToggleCalendarPanel))]
        private static void ToggleCalendarPanel(UIManager __instance)
        {
            // if toggled on close button call on escape method
            if (!__instance.panelCalendar.activeInHierarchy)
            {
                CloseAllPanels(__instance);
                return;
            }

            var calendarContent = __instance.panelCalendar.transform.GetChild(1).GetComponent<RectTransform>();
            var calendarDates = calendarContent.transform.GetChild(1).GetComponent<RectTransform>();
            var calendarDate = calendarDates.GetChild(GameManager.Instance.GetCurrentDay() - 1);
            var calendarDateImage = calendarDate.GetComponent<UnityEngine.UI.Image>();

            if (CalendarDateDefaultScale == default || CalendarDateDefaultColor == default)
            {
                CalendarDateDefaultScale = calendarDate.transform.localScale;
                CalendarDateDefaultColor = calendarDateImage.color;
                CalendarDateFocusedScale = new Vector3(
                    CalendarDateDefaultScale.x * 1.2f,
                    CalendarDateDefaultScale.y * 1.2f,
                    CalendarDateDefaultScale.z);
                CalendarDateFocusedColor = new Color(
                    CalendarDateDefaultColor.r * 1.005f,
                    CalendarDateDefaultColor.g * 1.01f,
                    CalendarDateDefaultColor.b * 1.2f,
                    CalendarDateDefaultColor.a);
            }

            calendarDate.transform.localScale = CalendarDateFocusedScale;
            calendarDateImage.color = CalendarDateFocusedColor;

            CalendarDateFocused = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.CloseAllPanels))]
        private static void CloseAllPanels(UIManager __instance)
        {
            // toggled on escape
            if (CalendarDateFocused)
            {
                var calendarContent = __instance.panelCalendar.transform.GetChild(1).GetComponent<RectTransform>();
                var calendarDates = calendarContent.transform.GetChild(1).GetComponent<RectTransform>();
                var calendarDate = calendarDates.GetChild(GameManager.Instance.GetCurrentDay() - 1);
                var calendarDateImage = calendarDate.GetComponent<UnityEngine.UI.Image>();

                calendarDate.transform.localScale = CalendarDateDefaultScale;
                calendarDateImage.color = CalendarDateDefaultColor;

                CalendarDateFocused = false;
            }
        }
    }
}
