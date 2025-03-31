
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.ScanIP
{
    public class LaunchableIpScanEventMono :  A_LaunchableIpScanCoroutinesMono {

        public UnityEvent m_launchScan;

        [ContextMenu("Launch Scan Coroutines")]
        public override void LaunchIpScanCoroutines()
        {
            m_launchScan.Invoke();
        }
    }
}

