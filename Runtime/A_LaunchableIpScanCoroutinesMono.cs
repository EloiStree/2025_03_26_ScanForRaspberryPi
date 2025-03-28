using UnityEngine;

namespace Eloi.ScanIP
{
    public abstract class A_LaunchableIpScanCoroutinesMono : MonoBehaviour, I_LaunchableIpScanCoroutines
    {
        public bool m_launchScanAtEnable;

        private void OnEnable()
        {
            if (m_launchScanAtEnable) 
                LaunchIpScanCoroutinesIfActive();
        }

        [ContextMenu("Launch Scan Coroutines if active")]
        public void LaunchIpScanCoroutinesIfActive() {

            if (this.gameObject.activeInHierarchy)
                LaunchIpScanCoroutines();

        }


        public  abstract void LaunchIpScanCoroutines();
    }
}

