using Jotunn.Managers;
using System;
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

        public GridSize GridSize { get; private set; }

        public RootPanel(Transform parent, Vector2 gridCellSize)
        {
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

            Name = $"{Mod.Info.Name} Root Panel";

            var headerText = GUIManager.Instance.CreateText(
                $"[ {Mod.Info.Name} ]  (you can drag this window!)",
                parent: Transform,
                anchorMin: new Vector2(0, 1),
                anchorMax: new Vector2(1, 1),
                position: new Vector2(0, 0),
                font: GUIManager.Instance.NorseBold,
                fontSize: 18,
                color: GUIManager.Instance.ValheimBeige,
                outline: true,
                outlineColor: Color.black,
                width: initialSize.x,
                height: Padding.top,
                addContentSizeFitter: false);


            var headerTextRt = (RectTransform)headerText.transform;
            headerTextRt.pivot = new Vector2(0, 1);
            headerTextRt.anchoredPosition = new Vector2(24, -12);

            // Create the scrollable panel
            //   This contains the ViewPort through which we view the container grid panel
            ScrollablePanel = new ScrollablePanel(
                parent: Transform,
                padding: Padding);

            // Create the container grid panel
            //   This panel contains a GridLayoutGroup which lays out all the containers
            ContentPanel = new ContainerGridPanel(
                parent: ScrollablePanel.ViewPort.transform,
                cellSize: gridCellSize,
                gridSpacing: ContainerPanel.WeightPanelWidth);

            // Set the ScrollablePanel's ScrollRect's content to be the ContainerGridPanel we just created
            ScrollablePanel.ScrollRectContent = ContentPanel;

            var localScale = XConfig.Instance.GetPanelScale();
            RectTransform.localScale = new Vector3(localScale, localScale, localScale);
        }

        public void UpdateSize(int visiblePanelCount)
        {
            // Don't go outside the screen bounds
            var scale = GuiScaler.m_scalers[0].GetScreenSizeFactor();
            int maxColsOnScreen = Mathf.FloorToInt(Screen.width / scale / ContainerPanel.SinglePanelWithWeightPanelSize.x);
            int maxRowsOnScreen = Mathf.FloorToInt(Screen.height / scale / ContainerPanel.SinglePanelWithWeightPanelSize.y);

            // Restrict to configured maximums
            int maxCols = (int)Math.Min(XConfig.Instance.MaxSize.Columns, maxColsOnScreen);
            int maxRows = (int)Math.Min(XConfig.Instance.MaxSize.Rows, maxRowsOnScreen);

            GridSize = GridSize.Calculate(maxCols, maxRows, visiblePanelCount);

            UpdatePanelSize();
            RestorePosition();
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
            Log.Debug($"Setting size based on {GridSize}");

            var newPanelSize = CalculatePanelSize();
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newPanelSize.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newPanelSize.y);

            ContentPanel.GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            ContentPanel.GridLayoutGroup.constraintCount = GridSize.Columns;
        }

        #region Save/Restore panel position
        public void SavePosition()
        {
            XConfig.Instance.SavePanelPosition(GridSize, Transform.position);
        }

        public void RestorePosition()
        {
            Transform.position = XConfig.Instance.GetPanelPosition(GridSize);
            ClampToScreen();
        }

        private void ClampToScreen()
        {
            Vector2 pos = Transform.position;
            Rect rect = RectTransform.rect;
            Vector2 lossyScale = RectTransform.lossyScale;

            float minX = rect.width / 2f * lossyScale.x;
            float maxX = Screen.width - minX;
            float minY = rect.height / 2f * lossyScale.y;
            float maxY = Screen.height - minY;

            pos.x = Mathf.RoundToInt(Mathf.Clamp(pos.x, minX, maxX));
            pos.y = Mathf.RoundToInt(Mathf.Clamp(pos.y, minY, maxY));

            Transform.position = pos;
        }
        #endregion
    }
}
