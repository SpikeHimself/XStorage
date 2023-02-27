using UnityEngine;

namespace XStorage
{
    public class ContainerTextReceiver : MonoBehaviour, TextReceiver
    {
        public Container m_Container;
        //private ZDO m_Zdo;

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
            XStorage.RenameContainerAndInventory(m_Container, text);
            RPC.SendRenameRequestToServer(m_Container.GetZDOID(), text);
        }

    }
}
