using HarmonyLib;
using UnityEngine;

namespace XStorage.Patches
{
    [HarmonyPatch(typeof(UITooltip), nameof(UITooltip.OnHoverStart))]
    static class UITooltip_OnHoverStart
    {
        static void Postfix(ref GameObject ___m_tooltip)
        {
            if (___m_tooltip && ___m_tooltip.transform.parent && PanelManager.Instance.IsVisible() &&
                ___m_tooltip.transform.parent == PanelManager.Instance.ContentPanel.Transform)
            {
                // Problem 1: The tooltip is only half visible because it's inside a scrollrect
                //      This is fixed by changing the tooltip's parent to something that's outside of the scrollrect, for example XStorage's RootPanel

                // Problem 2: The further you scroll the scrollrect down, the further the tooltip will be from the mouse pointer
                //      This is fixed by re-instantiating the tooltip.
                //      Yes, you read that correctly: literally copying the exact same object with the exact same properties will not have that same problem.
                //      It's Valheim magic. Fifteen hours of my life that I will never get back.

                var newTooltip = Object.Instantiate(___m_tooltip, PanelManager.Instance.RootPanel.Transform);
                Object.Destroy(___m_tooltip);
                ___m_tooltip = newTooltip;
            }
        }
    }
}
