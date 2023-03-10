using HarmonyLib;

namespace XStorage.Patches
{
    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Show))]
    static class InventoryGui_Show
    {
        static void Postfix(Container container)
        {
            if (container) // && !XStorageConfig.Instance.ChestChaining.Value)
            {
                Jotunn.Logger.LogDebug($"Showing container `{container.GetXStorageNameOrDefault()}`");
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
        static bool Prefix(InventoryGui __instance, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod)
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

            if (mod == InventoryGrid.Modifier.Move && !item.m_shared.m_questItem)
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
