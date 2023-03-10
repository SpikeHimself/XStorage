using UnityEngine;

namespace XStorage.GUI
{
    public abstract class XUIPanel
    {
        public GameObject GameObject { get; protected set; }

        public string Name
        {
            get
            {
                return GameObject.name;
            }
            set
            {
                GameObject.name = value;
            }
        }

        public Transform Transform
        {
            get
            {
                return GameObject.transform;
            }
        }

        public Transform Parent
        {
            get
            {
                return Transform.parent;
            }
            set
            {
                Transform.SetParent(value, worldPositionStays: false);
            }
        }

        private RectTransform rt;
        public RectTransform RectTransform
        {
            get
            {
                if (!rt)
                {
                    rt = Transform as RectTransform;
                }
                return rt;
            }
        }

        public bool IsVisible()
        {
            return GameObject && GameObject.activeSelf;
        }

        public void SetActive(bool active)
        {
            GameObject.SetActive(active);
        }
    }
}
