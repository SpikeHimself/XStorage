using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XStorage.Components;
using XStorage.GUI;

namespace XStorage
{
    internal class PanelManager
    {
        ////////////////////////////
        //// Singleton instance ////
        private static readonly Lazy<PanelManager> lazy = new Lazy<PanelManager>(() => new PanelManager());
        public static PanelManager Instance { get { return lazy.Value; } }
        ////////////////////////////

        public RootPanel RootPanel;
        public ContainerGridPanel ContentPanel
        {
            get
            {
                return RootPanel.ContentPanel;
            }
        }

        private List<ContainerGui> containerPanels;

        int VisiblePanelsCount
        {
            get
            {
                return containerPanels.Where(c => c.IsContainerOpen()).Count();
            }
        }

        private PanelManager()
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
            return RootPanel != null &&
                RootPanel.IsVisible() &&
                VisiblePanelsCount > 0;
        }

        public void Show(Container container)
        {
            CreateRootPanel();

            if (!IsVisible())
            {
                Jotunn.Logger.LogDebug("Opening ContainersPanel, resetting scroll view");
                RootPanel.ScrollablePanel.ScrollUp();
            }

            var newContainerPanel = AddOrEnablePanel(container);
            newContainerPanel.Show(container);

            RootPanel.UpdateSize(VisiblePanelsCount);
            RootPanel.SetActive(true);
        }

        public void Hide()
        {
            if (RootPanel == null || !RootPanel.GameObject)
            {
                return;
            }

            RootPanel.SavePosition();

            Jotunn.Logger.LogDebug("Hiding");
            containerPanels.ForEach(c => c.Hide());
            RootPanel.SetActive(false);
        }

        public void AddPanel()
        {
            var newPanel = new ContainerPanel(
                parent: ContentPanel,
                name: "XStorage Container" + containerPanels.Count);

            containerPanels.Add(newPanel.ContainerGui);

            Jotunn.Logger.LogDebug($"[AddPanel] Total container panels: {containerPanels.Count}");
        }

        private ContainerGui AddOrEnablePanel(Container container)
        {
            Jotunn.Logger.LogDebug("AddOrEnablePanel");

            // If this container is already being displayed, return its panel instead of adding a new one
            var activePanel = FindPanel(container);
            if (activePanel)
            {
                return activePanel;
            }

            // Find a panel that was previously created but is not currently in use
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

        public bool ContainsPanel(Container container)
        {
            return containerPanels.Where(c => c.m_currentContainer == container).Any();
        }

        public ContainerGui FindPanel(Container container)
        {
            return containerPanels.Where(c => c.m_currentContainer == container).FirstOrDefault();
        }

        public ContainerGui FindInactivePanel()
        {
            return containerPanels.Where(c => !c.IsContainerOpen()).FirstOrDefault();
        }

        public List<Container> GetContainerList()
        {
            return containerPanels.Where(c => c.IsContainerOpen()).Select(c => c.m_currentContainer).ToList();
        }

        private void CreateRootPanel()
        {
            if (RootPanel != null)
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

            RootPanel = new RootPanel(
                parent: inventoryScreenRoot.transform,
                gridCellSize: ContainerPanel.SinglePanelSize);
        }
    }
}
