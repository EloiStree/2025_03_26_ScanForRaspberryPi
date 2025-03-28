using UnityEngine;
using UnityEngine.Events;


namespace Eloi.ScanIP
{
    public class ScanIpMono_FacadeDevicesFoundMono : MonoBehaviour
    {

        public UnityEvent<string[]> m_onDevicesIpv4Found;

        public void NotifyDevicesFound(string[] device)
        {

            m_onDevicesIpv4Found.Invoke(device);
        }

    }
}

