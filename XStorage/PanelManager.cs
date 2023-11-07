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
            containerPanels = new List<ContainerGui>();
        }

        internal void Reset()
        {
            Clear();
            RootPanel = null;
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
                Log.Debug("Opening ContainersPanel, resetting scroll view");
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

            Log.Debug("Hiding");
            containerPanels.ForEach(c => c.Hide());
            RootPanel.SetActive(false);
        }

        public void AddPanel()
        {
            var newPanel = new ContainerPanel(
                parent: ContentPanel,
                name: $"{Mod.Info.Name} Container{containerPanels.Count}");

            containerPanels.Add(newPanel.ContainerGui);

            Log.Debug($"Total container panels: {containerPanels.Count}");
        }

        private ContainerGui AddOrEnablePanel(Container container)
        {
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
            if (RootPanel != null && RootPanel.GameObject)
            {
                // root panel was already created, nothing to do
                return;
            }

            Log.Debug("Creating root panel");

            var inventoryScreenRoot = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui/Inventory_screen/root");
            if (!inventoryScreenRoot)
            {
                Log.Error("Can't find inventory screen root");
                return;
            }

            RootPanel = new RootPanel(
                parent: inventoryScreenRoot.transform,
                gridCellSize: ContainerPanel.SinglePanelSize);
        }
    }
}
