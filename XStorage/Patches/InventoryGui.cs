using HarmonyLib;

namespace XStorage.Patches
{
    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Show))]
    static class InventoryGui_Show
    {
        static void Postfix(Container container)
        {
            if (container)
            {
                Jotunn.Logger.LogDebug($"Showing container `{container.GetXStorageNameOrDefault()}`");

                // Do not open nearby containers if this container has the SkipMark set
                // Other mods can use this if they don't want XStorage to open nearby chests
                if (container.GetXStorageSkipMark())
                {
                    Jotunn.Logger.LogDebug("This container has the SkipMark set. Not opening nearby containers.");
                    return;
                }

                XStorage.OpenNearbyContainers(container);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Hide))]
    static class InventoryGui_Hide
    {
        static void Prefix(InventoryGui __instance)
        {
            if (!__instance.m_animator.GetBool("visible"))
            {
                return;
            }

            PanelManager.Instance.Hide();
        }
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.OnSelectedItem))]
    static class InventoryGui_OnSelectedItem
    {
        static bool Prefix(InventoryGui __instance, InventoryGrid grid, ItemDrop.ItemData item, InventoryGrid.Modifier mod)
        {
            if (grid != InventoryGui.instance.m_playerGrid)
            {
                // The grid being clicked is the container grid, not the player inventory: Ignore
                return true;
            }

            if (!__instance.m_currentContainer)
            {
                // No container opened: Ignore
                return true;
            }

            if (mod == InventoryGrid.Modifier.Move && item != null && !item.m_shared.m_questItem)
            {
                bool itemMoved = XStorage.MoveItemToSuitableContainer(grid, item);
                if (itemMoved)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
