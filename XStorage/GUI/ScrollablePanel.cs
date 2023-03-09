using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace XStorage.GUI
{
    public class ScrollablePanel : XUIPanel
    {
        private ScrollRect ScrollRect
        {
            get
            {
                return GameObject.GetComponent<ScrollRect>();
            }
        }

        public GameObject ViewPort { get; }

        private ContainerGridPanel scrollRectContent;
        public ContainerGridPanel ScrollRectContent
        {
            get
            {
                return scrollRectContent;
            }
            set
            {
                scrollRectContent = value;
                ScrollRect.content = value.RectTransform;
            }
        }

        public ScrollablePanel(Transform parent, RectOffset padding)
        {
            GameObject = new GameObject("Scroll View", typeof(Image), typeof(ScrollRect), typeof(Mask));
            GameObject.FillParent(padding);
            Parent = parent;

            GameObject.GetComponent<Image>().color = new Color(0, 0, 0, 1f);
            GameObject.GetComponent<Mask>().showMaskGraphic = false;

            ScrollRect.horizontal = false;
            ScrollRect.vertical = true;
            ScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            ScrollRect.scrollSensitivity = 35f;
            ScrollRect.movementType = ScrollRect.MovementType.Clamped;
            ScrollRect.inertia = false;

            ViewPort = CreateViewPort(GameObject.transform);
            ScrollRect.viewport = (RectTransform)ViewPort.transform;

            var goScrollbar = CreateScrollbar(GameObject.transform, handleSize: 8f, handleDistanceToBorder: 8f);
            ScrollRect.verticalScrollbar = goScrollbar.GetComponent<Scrollbar>();
        }

        public void ScrollUp()
        {
            if (ScrollRect)
            {
                ScrollRect.verticalNormalizedPosition = 1f;
            }
        }

        #region Pain
        private GameObject CreateViewPort(Transform parent)
        {
            var goViewPort = new GameObject("ViewPort", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            goViewPort.transform.SetParent(parent, false);
            goViewPort.FillParent();
            goViewPort.GetComponent<Image>().color = new Color(0, 0, 0, 0);

            return goViewPort;
        }

        private GameObject CreateScrollbar(Transform parent, float handleSize, float handleDistanceToBorder)
        {
            GameObject goScrollbar = new GameObject("Scrollbar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Scrollbar));
            goScrollbar.transform.SetParent(parent, false);
            goScrollbar.AnchorToRightEdge(handleSize, handleDistanceToBorder);

            goScrollbar.GetComponent<Image>().color = GUIManager.Instance.ValheimScrollbarHandleColorBlock.disabledColor;

            var scrollBar = goScrollbar.GetComponent<Scrollbar>();
            scrollBar.colors = GUIManager.Instance.ValheimScrollbarHandleColorBlock;
            scrollBar.size = handleSize;
            scrollBar.direction = Scrollbar.Direction.BottomToTop;
            scrollBar.SetValueWithoutNotify(1);

            var goSlidingArea = new GameObject("Sliding Area", typeof(RectTransform));
            goSlidingArea.transform.SetParent(goScrollbar.transform, false);
            goSlidingArea.FillParent();

            var goHandle = CreateHandle(goSlidingArea.transform, handleSize);
            scrollBar.handleRect = (RectTransform)goHandle.transform;
            scrollBar.targetGraphic = goHandle.GetComponent<Image>();

            return goScrollbar;
        }

        private GameObject CreateHandle(Transform parent, float handleSize)
        {
            GameObject goHandle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            goHandle.transform.SetParent(parent, false);
            goHandle.FillParent();

            var image = goHandle.GetComponent<Image>();
            image.sprite = GUIManager.Instance.GetSprite("UISprite");
            image.type = Image.Type.Sliced;

            return goHandle;
        }
        #endregion

    }
}
