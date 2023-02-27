using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace XStorage
{
    internal class ContainersPanel
    {
        ////////////////////////////
        //// Singleton instance ////
        private static readonly Lazy<ContainersPanel> lazy = new Lazy<ContainersPanel>(() => new ContainersPanel());
        public static ContainersPanel Instance { get { return lazy.Value; } }
        ////////////////////////////

        GameObject rootPanel;
        Transform rootPanelContent;
        List<ContainerGui> containerPanels;

        int VisiblePanelsCount
        {
            get
            {
                return containerPanels.Where(c => c.IsContainerOpen()).Count();
            }
        }

        private ContainersPanel()
        {
            //CreateRoot();
            containerPanels = new List<ContainerGui>();
        }

        public void Clear()
        {
            containerPanels.Clear();
        }

        public void Show(Container container)
        {
            CreateRoot();

            var newContainerPanel = AddOrEnablePanel(container);
            newContainerPanel.Show(container);

            UpdateHeight();
            rootPanel.SetActive(true);
        }

        public void Hide()
        {
            if (!rootPanel)
            {
                return;
            }

            Jotunn.Logger.LogDebug("ContainersPanel.Hide");
            containerPanels.ForEach(c => c.Hide());
            rootPanel.SetActive(false);
        }

        private void UpdateHeight()
        {
            // Visible containers, constrained to a minimum of 1 and a maximum of 3
            var visiblePanelsCount = Math.Min(3, Math.Max(1, VisiblePanelsCount));

            var vanillaContainerPanel = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui(Clone)/Inventory_screen/root/Container");
            var singlePanelHeight = vanillaContainerPanel.GetComponent<RectTransform>().rect.height;

            var padding = 8f;
            var totalHeight = visiblePanelsCount * (padding + singlePanelHeight);

            Jotunn.Logger.LogDebug($"Setting root panel height: {totalHeight}");
            rootPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        }

        public void AddPanel()
        {
            Jotunn.Logger.LogDebug("ContainersPanel.AddPanel");
            containerPanels.Add(CloneContainerPanel());

            Jotunn.Logger.LogDebug($"[ContainersPanel.AddPanel] Total container panels: {containerPanels.Count}");
        }

        private ContainerGui AddOrEnablePanel(Container container)
        {
            Jotunn.Logger.LogDebug("ContainersPanel.AddOrEnablePanel");

            var activePanel = FindPanel(container);
            if (activePanel)
            {
                return activePanel;
            }

            var inactivePanel = FindInactivePanel();
            if (inactivePanel != null)
            {
                return inactivePanel;
            }

            // Add a new (disabled) panel
            AddPanel();

            // Recurse - this time i will find the disabled panel that was just added
            return AddOrEnablePanel(container);
        }

        //public void HidePanel(Container container)
        //{
        //    var panel = containerPanels.Where(c => c.currentContainer == container).FirstOrDefault();
        //    if (panel)
        //    {
        //        panel.Hide();
        //    }
        //}

        public bool ContainsPanel(Container container)
        {
            return (bool)FindPanel(container);
        }

        public ContainerGui FindPanel(Container container)
        {
            return containerPanels.Where(c => c.m_currentContainer == container).FirstOrDefault();
        }

        public ContainerGui FindInactivePanel()
        {
            return containerPanels.Where(c => !c.IsContainerOpen()).FirstOrDefault();
        }

        public List<Container> GetContainers()
        {
            return containerPanels.Where(c => c.IsContainerOpen()).Select(c => c.m_currentContainer).ToList();
        }

        private void CreateRoot()
        {
            if (rootPanel)
            {
                // root panel was already created, nothing to do
                return;
            }

            Jotunn.Logger.LogDebug("Creating root panel");

            var inventoryScreenRoot = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui(Clone)/Inventory_screen/root");

            if (!inventoryScreenRoot)
            {
                Jotunn.Logger.LogError("Can't find inventory screen root");
                return;
            }

            rootPanel = GUIManager.Instance.CreateWoodpanel(
                parent: inventoryScreenRoot.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0f, 0f),
                width: 700f,
                height: 1000f,
                draggable: true);
            rootPanel.name = "XStorage Root Panel";

            var canvas = GUIManager.Instance.CreateScrollView(
                parent: rootPanel.transform,
                showHorizontalScrollbar: false,
                showVerticalScrollbar: true,
                handleSize: 10f,
                handleDistanceToBorder: 10f,
                handleColors: GUIManager.Instance.ValheimScrollbarHandleColorBlock,
                slidingAreaBackgroundColor: Color.grey,
                width: 700f,
                height: 992f);

            var canvasRt = canvas.GetComponent<RectTransform>();
            //canvasRt.pivot = new Vector2(0f, 0f);
            canvasRt.anchorMin = new Vector2(0f, 0f);
            canvasRt.anchorMax = new Vector2(1f, 1f);
            canvasRt.pivot = new Vector2(0.5f, 0.5f);
            canvasRt.localPosition = new Vector3(0f, 0f, 0f);
            canvasRt.position = new Vector3(0f, -4f, 0f);
            canvasRt.anchoredPosition = new Vector2(0f, 0f);
            canvasRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 700f);
            canvasRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 992f);

            var canvasScrollRect = canvas.transform.GetComponentInChildren<ScrollRect>();
            canvasScrollRect.movementType = ScrollRect.MovementType.Clamped;

            rootPanelContent = canvasScrollRect.content.transform;

            var layoutGroup = rootPanelContent.gameObject.GetComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;

            // Disable the root panel, for now
            rootPanel.SetActive(false);
        }

        private ContainerGui CloneContainerPanel()
        {
            Jotunn.Logger.LogDebug("ContainersPanel.CloneContainerPanel");

            var vanillaContainerPanel = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui(Clone)/Inventory_screen/root/Container");

            var containerClone = GameObject.Instantiate(vanillaContainerPanel, rootPanelContent);
            containerClone.name = "XStorage Container" + containerPanels.Count;
            containerClone.SetActive(false); // Hide for now

            var containerScroll = containerClone.transform.Find("ContainerScroll");
            containerScroll.gameObject.SetActive(false);

            var containerGridScrollRect = containerClone.transform.Find("ContainerGrid").GetComponent<ScrollRect>();
            GameObject.Destroy(containerGridScrollRect);

            var gridElements = containerClone.transform.Find("ContainerGrid/Root").transform;
            foreach (Transform element in gridElements)
            {
                GameObject.Destroy(element.gameObject);
            }

            //var bkg = containerClone.transform.Find("Bkg").gameObject;
            //var bgkCanvasGroup = bkg.AddComponent<CanvasGroup>();
            //bgkCanvasGroup.alpha = 0.15f;
            containerClone.transform.Find("Bkg").gameObject.AddComponent<CanvasGroup>().alpha = 0.2f;

            //var weightBkg = containerClone.transform.Find("Weight/bkg").gameObject;
            //var weightBkgCanvasGroup = weightBkg.AddComponent<CanvasGroup>();
            //weightBkgCanvasGroup.alpha = 0.15f;
            containerClone.transform.Find("Weight/bkg").gameObject.AddComponent<CanvasGroup>().alpha = 0.2f;

            var containerCloneRt = containerClone.GetComponent<RectTransform>();
            containerCloneRt.anchorMin = new Vector2(0.5f, 0.5f);
            containerCloneRt.anchorMax = new Vector2(0.5f, 0.5f);
            containerCloneRt.pivot = new Vector2(0.5f, 0.5f);
            containerClone.transform.localPosition = new Vector2(0, 0);

            containerClone.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);

            var containerGui = containerClone.AddComponent<ContainerGui>();
            return containerGui;
        }

    }
}
