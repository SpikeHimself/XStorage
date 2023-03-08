using UnityEngine;
using UnityEngine.UI;
using XStorage.Components;

namespace XStorage.GUI
{
    public class ContainerPanel : XUIPanel
    {
        private static readonly float WeightPanelWidth = 75f;
        public static readonly Vector2 SinglePanelSize = new Vector2(570f + WeightPanelWidth, 340f);

        public ContainerGui ContainerGui
        {
            get
            {
                return GameObject.GetComponent<ContainerGui>();
            }
        }

        public ContainerPanel(ContainerGridPanel parent, string name)
        {
            GameObject = CloneContainerPanel(parent);

            Name = name;

            RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            RectTransform.pivot = new Vector2(0.5f, 0.5f);
            RectTransform.localPosition = new Vector2(0, 0);
            RectTransform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        }

        private static GameObject CloneContainerPanel(ContainerGridPanel parent)
        {
            Jotunn.Logger.LogDebug("ContainersPanel.CloneContainerPanel");

            var vanillaContainerPanel = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui(Clone)/Inventory_screen/root/Container");

            var containerClone = UnityEngine.Object.Instantiate(vanillaContainerPanel, parent.Transform);
            containerClone.SetActive(false); // Hide for now

            CleanupContainerClone(containerClone);

            containerClone.transform.Find("Bkg").gameObject.AddComponent<CanvasGroup>().alpha = 0.2f;
            containerClone.transform.Find("Weight/bkg").gameObject.AddComponent<CanvasGroup>().alpha = 0.2f;

            containerClone.AddComponent<ContainerGui>();
            return containerClone;
        }

        private static void CleanupContainerClone(GameObject clone)
        {
            var containerScroll = clone.transform.Find("ContainerScroll");
            containerScroll.gameObject.SetActive(false);

            var containerGridScrollRect = clone.transform.Find("ContainerGrid").GetComponent<ScrollRect>();
            GameObject.Destroy(containerGridScrollRect);

            var gridElements = clone.transform.Find("ContainerGrid/Root").transform;
            foreach (Transform element in gridElements)
            {
                GameObject.Destroy(element.gameObject);
            }
        }
    }
}
