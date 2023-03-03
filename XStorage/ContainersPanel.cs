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

        private Vector2 singlePanelSize = new Vector2(650, 340f);
        private float padding = 20f;

        public int MaxVisiblePanels = 3;

        public GameObject RootPanel;
        public ScrollablePanel ScrollablePanel;
        private List<ContainerGui> containerPanels;

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

        public bool IsVisible()
        {
            return RootPanel && RootPanel.activeSelf && VisiblePanelsCount > 0;
        }

        public void Show(Container container)
        {
            CreateRoot();

            if (!IsVisible())
            {
                Jotunn.Logger.LogDebug("Opening ContainersPanel, resetting scroll view");
                ScrollablePanel.ScrollUp();
            }

            var newContainerPanel = AddOrEnablePanel(container);
            newContainerPanel.Show(container);

            UpdateHeight();
            RootPanel.SetActive(true);
        }

        public void Hide()
        {
            if (!RootPanel)
            {
                return;
            }

            Jotunn.Logger.LogDebug("Hiding");
            containerPanels.ForEach(c => c.Hide());
            RootPanel.SetActive(false);
        }

        private void UpdateHeight()
        {
            // Visible containers, constrained to a minimum of 1 and a maximum of 3
            var visiblePanelsCount = Math.Min(MaxVisiblePanels, Math.Max(1, VisiblePanelsCount));

            var totalHeight = (2f * padding) + (visiblePanelsCount * singlePanelSize.y);

            Jotunn.Logger.LogDebug($"Setting root panel height: {totalHeight}");
            RootPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
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
            if (RootPanel)
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

            RootPanel = GUIManager.Instance.CreateWoodpanel(
                parent: inventoryScreenRoot.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0f, 0f),
                width: singlePanelSize.x + (2f * padding),
                height: singlePanelSize.y + (2f * padding),
                draggable: true);
            RootPanel.name = "XStorage Root Panel";

            // Disable the root panel, for now
            RootPanel.SetActive(false);


            ScrollablePanel = new ScrollablePanel(RootPanel.transform, padding);
        }

        private ContainerGui CloneContainerPanel()
        {
            Jotunn.Logger.LogDebug("ContainersPanel.CloneContainerPanel");

            var vanillaContainerPanel = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui(Clone)/Inventory_screen/root/Container");

            var containerClone = GameObject.Instantiate(vanillaContainerPanel, ScrollablePanel.Content.transform);
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
