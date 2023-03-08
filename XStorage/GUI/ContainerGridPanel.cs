using UnityEngine;
using UnityEngine.UI;

namespace XStorage.GUI
{
    /// <summary>
    /// Display containers in a grid
    /// </summary>
    public class ContainerGridPanel : XUIPanel
    {
        public GridLayoutGroup GridLayoutGroup
        {
            get
            {
                return GameObject.GetComponent<GridLayoutGroup>();
            }
        }

        public ContentSizeFitter ContentSizeFitter
        {
            get
            {
                return GameObject.GetComponent<ContentSizeFitter>();
            }
        }

        public ContainerGridPanel(Transform parent, Vector2 cellSize, float gridSpacing)
        {
            GameObject = new GameObject("ContainerGridPanel", typeof(RectTransform), typeof(GridLayoutGroup), typeof(Canvas), typeof(GraphicRaycaster), typeof(ContentSizeFitter));
            GameObject.FillParent();
            Parent = parent;

            RectTransform.pivot = new Vector2(0.5f, 1f);
            ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            GridLayoutGroup.cellSize = cellSize;
            GridLayoutGroup.spacing = new Vector2(gridSpacing, 0);
            GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            GridLayoutGroup.constraintCount = 2;
            GridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            GridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        }
    }
}
