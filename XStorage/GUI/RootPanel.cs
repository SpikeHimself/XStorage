using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace XStorage.GUI
{
    public class RootPanel : XUIPanel
    {
        //private const int Padding = 15;
        //private const int PaddingTop = 40;
        private static readonly RectOffset Padding = new RectOffset(left: 15, right: 15, top: 40, bottom: 15);

        public ScrollablePanel ScrollablePanel;
        public ContainerGridPanel ContentPanel;

        private GridSize gridSize;
        public GridSize GridSize
        {
            get
            {
                return gridSize;
            }
            set
            {
                if (gridSize != value)
                {
                    gridSize = value;
                    UpdatePanelSize();
                }
            }
        }

        public RootPanel(Transform parent, Vector2 gridCellSize)
        {
            Jotunn.Logger.LogDebug("Creating root panel");

            var initialSize = new Vector2(
                Padding.left + gridCellSize.x + Padding.right,
                Padding.top + gridCellSize.y + Padding.bottom);

            GameObject = GUIManager.Instance.CreateWoodpanel(
                parent,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0, 0),
                width: initialSize.x,
                height: initialSize.y,
                draggable: true);

            Name = "XStorage Root Panel";

            var headerText = GUIManager.Instance.CreateText(
                "[XStorage] Did you know you can drag this window?",
                parent: Transform,
                anchorMin: new Vector2(0, 1),
                anchorMax: new Vector2(1, 1),
                position: new Vector2(0, 0),
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: 18,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: initialSize.x,
                height: Padding.top,
                addContentSizeFitter: false);

            var headerTextRt = (RectTransform)headerText.transform;
            headerTextRt.pivot = new Vector2(0, 1);
            headerTextRt.anchoredPosition = new Vector2(24, -12);

            ScrollablePanel = new ScrollablePanel(
                parent: Transform,
                padding: Padding);

            ContentPanel = new ContainerGridPanel(
                parent: ScrollablePanel.ViewPort.transform,
                cellSize: gridCellSize,
                gridSpacing: ContainerPanel.WeightPanelWidth);

            ScrollablePanel.ScrollRectContent = ContentPanel;
        }

        private Vector2 CalculatePanelSize()
        {
            var width =
                Padding.left
                + GridSize.Columns * ContainerPanel.SinglePanelWithWeightPanelSize.x
                + Padding.right;

            var height =
                Padding.top
                + GridSize.Rows * ContainerPanel.SinglePanelWithWeightPanelSize.y
                + Padding.bottom;

            return new Vector2(width, height);
        }

        private void UpdatePanelSize()
        {
            Jotunn.Logger.LogDebug($"Setting size: {GridSize}");

            var newPanelSize = CalculatePanelSize();
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newPanelSize.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newPanelSize.y);

            ContentPanel.GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            ContentPanel.GridLayoutGroup.constraintCount = GridSize.Columns;
        }

        public void SavePosition()
        {
            var key = $"{XStorageConfig.ZdoProperty_GridSize}_{GridSize}";
            var value = Transform.localPosition.ToString();
            Jotunn.Logger.LogDebug($"Saving position: `{key}` = `{value}`");

            var customData = Player.m_localPlayer.m_customData;
            if (!customData.ContainsKey(key))
            {
                customData.Add(key, value);
            }
            else
            {
                customData[key] = value;
            }
        }

        public void RestorePosition()
        {
            Transform.localPosition = Vector3.zero;

            var key = $"{XStorageConfig.ZdoProperty_GridSize}_{GridSize}";
            var customData = Player.m_localPlayer.m_customData;
            if (customData.TryGetValue(key, out var value))
            {
                Jotunn.Logger.LogDebug($"Restoring position: `{key}` = `{value}`");
                Transform.localPosition = Util.StringToVector3(value);
            }
        }
    }
}
