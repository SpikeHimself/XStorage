using UnityEngine;
using UnityEngine.UI;

namespace XStorage.Components
{
    /// <summary>
    /// The ContainerGui component is responsible for visualising the contents of a container. It mimics some of the Container 
    /// related behaviour found in InventoryGui. ContainerGui assumes that it will be added to the gameobject which has
    /// InventoryGrid as a component in one of its children, such that: GetComponentInChildren<InventoryGrid>() yields the 
    /// InventoryGrid component.
    /// </summary>
    public class ContainerGui : MonoBehaviour
    {
        private RectTransform m_rect;

        // Container being displayed by this ContainerGui
        public Container m_currentContainer;

        // Child controls
        private InventoryGrid m_grid;
        private Text m_containerName;
        private Text m_containerWeight;
        private Button m_takeAllButton;

        // Used to reset the InventoryGrid view on the first update
        private bool m_firstGridUpdate = true;

        public void Awake()
        {
            Jotunn.Logger.LogDebug("ContainerGui.Awake");

            m_rect = (RectTransform)transform;
            m_grid = GetComponentInChildren<InventoryGrid>();

            m_grid.m_onSelected += OnGridItemSelected;
            m_grid.m_onRightClick += OnGridItemRightClick;

            m_takeAllButton = GetComponentInChildren<Button>();
            m_takeAllButton.onClick.AddListener(OnTakeAll);

            m_containerName = transform.Find("container_name").GetComponent<Text>();
            m_containerWeight = transform.Find("Weight/weight_text").GetComponent<Text>();
        }

        public void Update()
        {
            UpdateContainer();
            UpdateContainerWeight();
        }

        public void OnDestroy()
        {
            Jotunn.Logger.LogDebug("ContainerGui.OnDestroy");

            if (m_grid)
            {
                m_grid.m_onSelected -= OnGridItemSelected;
                m_grid.m_onRightClick -= OnGridItemRightClick;
            }
        }

        private void OnGridItemRightClick(InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos)
        {
            // Let InventoryGui handle this event
            InventoryGui.instance.OnRightClickItem(grid, item, pos);
        }

        private void OnGridItemSelected(InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod)
        {
            // By default, ctrl+click-ing (i.e. moving) an item, takes the item from the Player inventory and adds it to the currently active (vanilla) chest
            // Since the current item isn't coming from the Player inventory, this would create a bug through which items are duplicated, so we need to handle it manually
            if (mod == InventoryGrid.Modifier.Move)
            {
                Player localPlayer = Player.m_localPlayer;
                localPlayer.GetInventory().MoveItemToThis(grid.GetInventory(), item);
                InventoryGui.instance.UpdateCraftingPanel();
                InventoryGui.instance.m_moveItemEffects.Create(InventoryGui.instance.transform.position, Quaternion.identity);
                return;
            }

            // Any action that isn't ctrl+click (move) can be handled by vanilla code in InventoryGui
            InventoryGui.instance.OnSelectedItem(grid, item, pos, mod);
        }

        public void OnTakeAll()
        {
            var player = Player.m_localPlayer;
            if (!player.IsTeleporting() && IsContainerOpen())
            {
                CancelDrag();
                Inventory inventory = m_currentContainer.GetInventory();
                player.GetInventory().MoveAll(inventory);
                InventoryGui.instance.UpdateCraftingPanel();
            }
        }

        public void UpdateContainer()
        {
            if (IsContainerOpen())
            {
                m_currentContainer.SetInUse(true);
                m_grid.UpdateInventory(m_currentContainer.GetInventory(), null, null);
                m_containerName.text = Localization.instance.Localize(m_currentContainer.GetInventory().GetName());
                if (m_firstGridUpdate)
                {
                    m_grid.ResetView();
                    m_firstGridUpdate = false;
                }
            }
            else
            {
                Hide();
            }
        }

        public void UpdateContainerWeight()
        {
            if (IsContainerOpen())
            {
                int weightCeiled = Mathf.CeilToInt(m_currentContainer.GetInventory().GetTotalWeight());
                m_containerWeight.text = weightCeiled.ToString();
            }
        }

        public void Show(Container container)
        {
            m_firstGridUpdate = true;
            m_currentContainer = container;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            if (IsContainerOpen())
            {
                m_currentContainer.SetInUse(false);
                m_currentContainer = null;
            }

            m_firstGridUpdate = true;
        }

        public bool IsContainerOpen()
        {
            return m_currentContainer;
        }

        public void CancelDrag()
        {
            InventoryGui.instance.SetupDragItem(null, null, 1);
        }

    }
}
