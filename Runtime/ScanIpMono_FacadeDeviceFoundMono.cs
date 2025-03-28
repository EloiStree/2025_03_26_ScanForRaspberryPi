using UnityEngine;
using UnityEngine.Events;


namespace Eloi.ScanIP
{
    public class ScanIpMono_FacadeDeviceFoundMono : MonoBehaviour
    {

        public UnityEvent<string> m_onDeviceIpFound;

        public void NotifyDeviceFound(string device)
        {

            m_onDeviceIpFound.Invoke(device);
        }

    }
}

