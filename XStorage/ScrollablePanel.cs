using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace XStorage
{
    public class ScrollablePanel
    {
        public RectTransform Content
        {
            get
            {
                return m_content;
            }
        }

        public GridLayoutGroup GridLayoutGroup
        {
            get
            {
                return Content.GetComponent<GridLayoutGroup>();
            }
        }

        private RectTransform m_content;
        private ScrollRect m_scrollrect;

        public ScrollablePanel(Transform parent, float padding, Vector2 singlePanelSize, float gridSpacing)
        {
            CreateScrollablePanel(
                parent,
                handleSize: 8f,
                handleDistanceToBorder: 1f,
                padding,
                singlePanelSize,
                gridSpacing);
        }

        public void ScrollUp()
        {
            if (m_scrollrect)
            {
                m_scrollrect.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
            }
        }

        #region Pain
        private void CreateScrollablePanel(Transform parent, float handleSize, float handleDistanceToBorder, float padding, Vector2 singlePanelSize, float gridSpacing)
        {
            var scrollView = CreateScrollView(parent, padding);
            var viewPort = CreateViewPort(scrollView.transform);
            var scrollbar = CreateScrollbar(scrollView.transform, handleSize, handleDistanceToBorder);

            var content = CreateContentPanel(viewPort.transform, singlePanelSize, gridSpacing);
            m_content = (RectTransform)content.transform;

            m_scrollrect = scrollView.GetComponent<ScrollRect>();
            m_scrollrect.viewport = (RectTransform)viewPort.transform;
            m_scrollrect.verticalScrollbar = scrollbar.GetComponent<Scrollbar>();
            m_scrollrect.content = Content;
        }

        private GameObject CreateScrollView(Transform parent, float padding)
        {
            var scrollView = new GameObject("Scroll View", typeof(Image), typeof(ScrollRect), typeof(Mask));
            scrollView.transform.SetParent(parent.transform, false);
            scrollView.FillParent(padding);

            scrollView.GetComponent<Image>().color = new Color(0, 0, 0, 1f);
            scrollView.GetComponent<Mask>().showMaskGraphic = false;
            scrollView.GetComponent<ScrollRect>().horizontal = false;
            scrollView.GetComponent<ScrollRect>().vertical = true;
            scrollView.GetComponent<ScrollRect>().horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            scrollView.GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollView.GetComponent<ScrollRect>().scrollSensitivity = 35f;
            scrollView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
            scrollView.GetComponent<ScrollRect>().inertia = false;

            return scrollView;
        }

        private GameObject CreateContentPanel(Transform parent, Vector2 singlePanelSize, float gridSpacing)
        {
            var contentPanel = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup), typeof(Canvas), typeof(GraphicRaycaster), typeof(ContentSizeFitter));
            contentPanel.transform.SetParent(parent, false);
            contentPanel.FillParent();

            var rt = (RectTransform)contentPanel.transform;
            rt.pivot = new Vector2(0.5f, 1f);
            //contentPanel.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
            //contentPanel.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = false;
            //contentPanel.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
            //contentPanel.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
            //contentPanel.GetComponent<VerticalLayoutGroup>().childControlWidth = false;
            contentPanel.GetComponent<GridLayoutGroup>().cellSize = singlePanelSize;
            contentPanel.GetComponent<GridLayoutGroup>().spacing = new Vector2(gridSpacing, 0);
            contentPanel.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            contentPanel.GetComponent<GridLayoutGroup>().constraintCount = 2;
            contentPanel.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            contentPanel.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
            contentPanel.GetComponent<Canvas>().planeDistance = 5.2f;
            contentPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return contentPanel;
        }

        private GameObject CreateViewPort(Transform parent)
        {
            var viewPort = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            viewPort.transform.SetParent(parent, false);
            viewPort.FillParent();

            viewPort.GetComponent<Image>().color = new Color(0, 0, 0, 0);

            return viewPort;
        }

        private GameObject CreateScrollbar(Transform parent, float handleSize, float handleDistanceToBorder)
        {
            GameObject verticalScrollbar = new GameObject("Scrollbar Vertical", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Scrollbar));
            verticalScrollbar.transform.SetParent(parent, false);
            verticalScrollbar.AnchorToRightEdge(handleSize, handleDistanceToBorder);

            verticalScrollbar.GetComponent<Image>().color = GUIManager.Instance.ValheimScrollbarHandleColorBlock.disabledColor;
            verticalScrollbar.GetComponent<Scrollbar>().colors = GUIManager.Instance.ValheimScrollbarHandleColorBlock;
            verticalScrollbar.GetComponent<Scrollbar>().size = 0.4f;
            verticalScrollbar.GetComponent<Scrollbar>().size = handleSize;
            verticalScrollbar.GetComponent<Scrollbar>().direction = Scrollbar.Direction.BottomToTop;
            verticalScrollbar.GetComponent<Scrollbar>().SetValueWithoutNotify(1f);

            var slidingArea = CreateSlidingArea(verticalScrollbar.transform, handleSize, handleDistanceToBorder);
            var handle = CreateHandle(slidingArea.transform, handleSize);

            verticalScrollbar.GetComponent<Scrollbar>().handleRect = (RectTransform)handle.transform;
            verticalScrollbar.GetComponent<Scrollbar>().targetGraphic = handle.GetComponent<Image>();

            return verticalScrollbar;
        }

        private GameObject CreateHandle(Transform parent, float handleSize)
        {
            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(parent, false);

            var rt = (RectTransform)handle.transform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(handleSize / 2f, 0);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, handleSize / 2f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, handleSize / 2f);
            handle.GetComponent<Image>().sprite = GUIManager.Instance.GetSprite("UISprite");
            handle.GetComponent<Image>().type = Image.Type.Sliced;

            return handle;
        }

        private GameObject CreateSlidingArea(Transform parent, float handleSize, float handleDistanceToBorder)
        {
            GameObject slidingArea = new GameObject("Sliding Area", typeof(RectTransform));
            slidingArea.transform.SetParent(parent, false);
            slidingArea.AnchorToRightEdge(handleSize, handleDistanceToBorder);

            return slidingArea;
        }


        #endregion

    }
}
