using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XStorage.GUI;

namespace XStorage
{
    internal class ContainersPanel
    {
        ////////////////////////////
        //// Singleton instance ////
        private static readonly Lazy<ContainersPanel> lazy = new Lazy<ContainersPanel>(() => new ContainersPanel());
        public static ContainersPanel Instance { get { return lazy.Value; } }
        ////////////////////////////

        private Vector2 singlePanelSize = new Vector2(570f, 340f);
        private float weightPanelWidth = 60f;

        private float padding = 15f;

        public int MaxRows = 3;
        public int PreferredColumns = 1;
        public int MaxColumns = 2;

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

            UpdateSize();
            RootPanel.SetActive(true);
        }

        public void Hide()
        {
            if (!RootPanel)
            {
                return;
            }

            SaveLocation();
            Jotunn.Logger.LogDebug("Hiding");
            containerPanels.ForEach(c => c.Hide());
            RootPanel.SetActive(false);
        }

        private void UpdateSize()
        {
            var singlePanelWidth = singlePanelSize.x + weightPanelWidth;
            var singlePanelHeight = singlePanelSize.y;

            var columns = Math.Min(VisiblePanelsCount, PreferredColumns);
            if (VisiblePanelsCount / columns > MaxRows)
            {
                columns = MaxColumns;
            }

            // Visible rows, constrained to a minimum of 1 and a maximum of MaxRows
            var rows = (int)Math.Ceiling(Math.Min((float)MaxRows, Math.Max(1, VisiblePanelsCount / columns)));


            var rootPanelWidth = (2f * padding) + (columns * singlePanelWidth);
            var rootPanelHeight = (2f * padding) + (rows * singlePanelHeight);

            Jotunn.Logger.LogDebug($"Setting size: {columns} x {rows}");

            var rt = (RectTransform)RootPanel.transform;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rootPanelWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rootPanelHeight);

            ScrollablePanel.GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            ScrollablePanel.GridLayoutGroup.constraintCount = columns;
            RestoreSavedLocation();
        }

        private Dictionary<Vector2, Vector2> savedLocations = new Dictionary<Vector2, Vector2>();
        private void SaveLocation()
        {
            var rt = (RectTransform)RootPanel.transform;
            var size = rt.rect.size;
            var location = rt.localPosition;
            if (!savedLocations.ContainsKey(size))
            {
                savedLocations.Add(size, location);
            }
            else
            {
                savedLocations[size] = location;
            }
        }
        private void RestoreSavedLocation()
        {
            var rt = (RectTransform)RootPanel.transform;
            var size = rt.rect.size;
            if (savedLocations.ContainsKey(size))
            {
                rt.localPosition = savedLocations[size];
            }
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
                width: 100,
                height: 50,
                draggable: false);
            RootPanel.AddComponent<XDragWindowControl>();
            RootPanel.name = "XStorage Root Panel";

            ScrollablePanel = new ScrollablePanel(
                parent: RootPanel.transform,
                padding,
                singlePanelSize,
                gridSpacing: weightPanelWidth);
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

            var containerCloneRt = (RectTransform)containerClone.transform;
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
