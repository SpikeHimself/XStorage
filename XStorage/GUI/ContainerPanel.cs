using UnityEngine;
using UnityEngine.UI;
using XStorage.Components;

namespace XStorage.GUI
{
    public class ContainerPanel : XUIPanel
    {
        public static readonly float WeightPanelWidth = 75;
        public static readonly Vector2 SinglePanelSize = new Vector2(570, 340);
        public static readonly Vector2 SinglePanelWithWeightPanelSize = new Vector2(SinglePanelSize.x + WeightPanelWidth, SinglePanelSize.y);

        public ContainerGui ContainerGui
        {
            get
            {
                return GameObject.GetComponent<ContainerGui>();
            }
        }

        public ContainerPanel(ContainerGridPanel parent, string name)
        {
            GameObject = ClonePrefab(parent.Transform);
            SetActive(false);

            Name = name;

            RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            RectTransform.pivot = new Vector2(0.5f, 0.5f);
            RectTransform.localPosition = new Vector2(0, 0);
            RectTransform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        }

        private static GameObject ClonePrefab(Transform parent)
        {
            CreatePanelPrefab(parent);
            return UnityEngine.Object.Instantiate(panelPrefab, parent);
        }

        #region Panel Prefab
        private static GameObject panelPrefab;
        private static void CreatePanelPrefab(Transform parent)
        {
            if(panelPrefab )
            {
                return;
            }

            var vanillaContainerPanel = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui(Clone)/Inventory_screen/root/Container");
            
            panelPrefab = UnityEngine.Object.Instantiate(vanillaContainerPanel, parent);
            panelPrefab.SetActive(false); // Hide for now

            var containerScroll = panelPrefab.transform.Find("ContainerScroll");
            containerScroll.gameObject.SetActive(false);

            var containerGridScrollRect = panelPrefab.transform.Find("ContainerGrid").GetComponent<ScrollRect>();
            GameObject.Destroy(containerGridScrollRect);

            var gridElements = panelPrefab.transform.Find("ContainerGrid/Root").transform;
            foreach (Transform element in gridElements)
            {
                GameObject.Destroy(element.gameObject);
            }

            panelPrefab.transform.Find("Bkg").gameObject.AddComponent<CanvasGroup>().alpha = 0.2f;
            panelPrefab.transform.Find("Weight/bkg").gameObject.AddComponent<CanvasGroup>().alpha = 0.2f;

            panelPrefab.AddComponent<ContainerGui>();
        }
        #endregion
    }
}
