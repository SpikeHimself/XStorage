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

        private RectTransform m_content;
        private ScrollRect m_scrollrect;

        public ScrollablePanel(Transform parent, float padding)
        {
            CreateScrollablePanel(
                parent: parent,
                handleSize: 8f,
                handleDistanceToBorder: 1f,
                padding: padding);
        }

        public void ScrollUp()
        {
            if (m_scrollrect)
            {
                m_scrollrect.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
            }
        }

        #region Pain
        private void CreateScrollablePanel(Transform parent, float handleSize, float handleDistanceToBorder, float padding)
        {
            var scrollView = CreateScrollView(parent, padding);
            var viewPort = CreateViewPort(scrollView.transform);
            var scrollbar = CreateScrollbar(scrollView.transform, handleSize, handleDistanceToBorder);

            var content = CreateContentPanel(viewPort.transform);
            m_content = content.GetComponent<RectTransform>();

            m_scrollrect = scrollView.GetComponent<ScrollRect>();
            m_scrollrect.viewport = viewPort.GetComponent<RectTransform>();
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

        private GameObject CreateContentPanel(Transform parent)
        {
            var contentPanel = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(Canvas), typeof(GraphicRaycaster), typeof(ContentSizeFitter));
            contentPanel.transform.SetParent(parent, false);
            contentPanel.FillParent();

            contentPanel.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            contentPanel.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
            contentPanel.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = false;
            contentPanel.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
            contentPanel.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
            contentPanel.GetComponent<VerticalLayoutGroup>().childControlWidth = false;
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

            verticalScrollbar.GetComponent<Scrollbar>().handleRect = handle.GetComponent<RectTransform>();
            verticalScrollbar.GetComponent<Scrollbar>().targetGraphic = handle.GetComponent<Image>();

            return verticalScrollbar;
        }

        private GameObject CreateHandle(Transform parent, float handleSize)
        {
            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(parent, false);

            handle.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            handle.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            handle.GetComponent<RectTransform>().anchoredPosition = new Vector2(handleSize / 2f, 0);
            handle.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, handleSize / 2f);
            handle.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, handleSize / 2f);
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
