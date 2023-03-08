using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace XStorage.GUI
{
    public class RootPanel : XUIPanel
    {
        private const float Padding = 15f;

        public ScrollablePanel ScrollablePanel;
        public ContainerGridPanel ContentPanel;

        private GridSize size;
        public GridSize Size
        {
            get
            {
                return size;
            }
            set
            {
                if (size != value)
                {
                    size = value;
                    UpdateSize(size);
                }
            }
        }

        public RootPanel(Transform parent, Vector2 gridCellSize)
        {
            Jotunn.Logger.LogDebug("Creating root panel");

            GameObject = GUIManager.Instance.CreateWoodpanel(
                parent,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0f, 0f),
                width: 100,
                height: 50,
                draggable: true);

            Name = "XStorage Root Panel";

            ScrollablePanel = new ScrollablePanel(
                parent: Transform,
                RootPanel.Padding);

            ContentPanel = new ContainerGridPanel(
                parent: ScrollablePanel.ViewPort.transform,
                cellSize: gridCellSize,
                //gridSpacing: ContainerPanel.WeightPanelWidth);
                gridSpacing: 1f);

            ScrollablePanel.ScrollRectContent = ContentPanel;
        }

        private void UpdateSize(GridSize newSize)
        {
            var newWidth = (2f * RootPanel.Padding) + (newSize.Columns * ContainerPanel.SinglePanelSize.x);
            var newHeight = (2f * RootPanel.Padding) + (newSize.Rows * ContainerPanel.SinglePanelSize.y);

            Jotunn.Logger.LogDebug($"Setting size: {newSize}");

            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

            ContentPanel.GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            ContentPanel.GridLayoutGroup.constraintCount = newSize.Columns;
        }
        
        public void SavePosition()
        {
            var key = $"{Config.ZdoProperty_GridSize}_{Size}";
            var value = Transform.localPosition.ToString();
            Jotunn.Logger.LogDebug($"Saving position: `{key}` = `{value}`");

            var customData = Player.m_localPlayer.m_customData;
            if( !customData.ContainsKey(key) )
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
            var key = $"{Config.ZdoProperty_GridSize}_{Size}";

            var customData = Player.m_localPlayer.m_customData;
            if( customData.ContainsKey(key))
            {
                Vector3 value = Util.StringToVector3(customData[key]);
                Jotunn.Logger.LogDebug($"Restoring position: `{key}` = `{value}`");
                Transform.localPosition = value;
            }
        }

    }
}
