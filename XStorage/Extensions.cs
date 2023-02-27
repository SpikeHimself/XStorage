using UnityEngine;

namespace XStorage
{
    internal static class Extensions
    {
        #region Character
        internal static Vector3 GetPosition(this Character character)
        {
            return character.m_nview.GetZDO().GetPosition();
        }
        #endregion

        #region Container
        internal static float Distance(this Container container, Character character)
        {
            return Vector3.Distance(container.GetPosition(), character.GetPosition());
        }

        internal static float Distance(this Container container, Container other)
        {
            return Vector3.Distance(container.GetPosition(), other.GetPosition());
        }

        internal static Vector3 GetPosition(this Container container)
        {
            return container.GetZDO().GetPosition();
        }

        internal static ZDO GetZDO(this Container container)
        {
            return container.m_nview.GetZDO();
        }

        internal static ZDOID GetZDOID(this Container container)
        {
            return container.GetZDO().m_uid;
        }

        internal static string GetXStorageName(this Container container)
        {
            return container.GetZDO().GetString(XStorage.ZdoProperty_ContainerName, string.Empty);
        }

        internal static string GetXStorageNameOrDefault(this Container container)
        {
            return container.GetZDO().GetString(XStorage.ZdoProperty_ContainerName, Localization.instance.Localize(container.m_name));
        }

        internal static bool HasRoomFor(this Container container, string itemName)
        {
            return container.GetInventory().FindFreeStackSpace(itemName) > 0 ||
                   (
                     //container.GetInventory().HaveItem(itemName) &&
                     container.GetInventory().HaveEmptySlot()
                   );
        }

        internal static bool IsPlacedByPlayer(this Container container)
        {
            return container.m_piece && container.m_piece.IsPlacedByPlayer();
        }
        #endregion

        #region Vector3
        internal static float Distance(this Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }
        #endregion
    }
}
