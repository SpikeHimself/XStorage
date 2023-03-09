using UnityEngine;
using XStorage.Components;

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
            return container.GetZDO().GetString(XStorageConfig.ZdoProperty_ContainerName, string.Empty);
        }

        internal static string GetXStorageNameOrDefault(this Container container)
        {
            return container.GetZDO().GetString(XStorageConfig.ZdoProperty_ContainerName, Localization.instance.Localize(container.m_name));
        }

        internal static bool HasRoomFor(this Container container, string itemName)
        {
            var inv = container.GetInventory();
            return inv.FindFreeStackSpace(itemName) > 0 || inv.HaveEmptySlot();
        }

        internal static bool IsPlacedByPlayer(this Container container)
        {
            return container.m_piece && container.m_piece.IsPlacedByPlayer();
        }
        #endregion

        #region UI
        /// <summary>
        /// Set pivot to middle-centre and stretch to the parent by anchoring it to all corners.
        /// Warning: This assumes the GameObject has a RectTransform component. No checks are carried out.
        /// </summary>
        /// <param name="go">The GameObject of which the RectTransform should fill its parent</param>
        /// <param name="padding">The size difference (offset) from the parent's edges</param>
        public static void FillParent(this GameObject go, float padding = 0)
        {
            var rt = (RectTransform)go.transform;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, 0);

            float sizeDelta = -(2f * padding);
            rt.sizeDelta = new Vector2(sizeDelta, sizeDelta);
        }

        /// <summary>
        /// Anchor this GameObject to its parent's right edge.
        /// Warning: This assumes the GameObject has a RectTransform component. No checks are carried out.
        /// </summary>
        /// <param name="go">The GameObject of which the RectTransform should be anchored to its parent</param>
        /// <param name="width">The width of this GameObject's RectTransform</param>
        /// <param name="padding">The room to leave at the top and bottom of this GameObject's RectTransform</param>
        public static void AnchorToRightEdge(this GameObject go, float width, float padding = 0)
        {
            var rt = (RectTransform)go.transform;
            rt.anchorMin = new Vector2(1f, 0);
            rt.anchorMax = new Vector2(1f, 1);
            rt.pivot = new Vector2(1f, 0.5f);
            rt.anchoredPosition = new Vector2(0, -(width / 2f));

            var heightDelta = (2f * padding) - width;
            rt.sizeDelta = new Vector2(width, heightDelta);
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
