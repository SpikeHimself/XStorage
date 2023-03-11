using UnityEngine;

namespace XStorage.Components
{
    public class ContainerTextReceiver : MonoBehaviour, TextReceiver
    {
        public Container m_Container;

        public void Awake()
        {
            m_Container = GetComponent<Container>();
        }

        public string GetText()
        {
            return m_Container.GetXStorageName();
        }

        public void SetText(string text)
        {
            // Preemptively set the new name locally, assuming the server will do so too in just a moment
            XStorage.UpdateContainerAndInventoryName(m_Container, text);

            // Ask the server to rename this container
            RPC.SendRenameRequestToServer(m_Container.GetZDOID(), text);
        }
    }
}
