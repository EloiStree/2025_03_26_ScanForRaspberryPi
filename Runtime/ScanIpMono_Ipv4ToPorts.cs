using UnityEngine;
using UnityEngine.Events;

public class ScanIpMono_Ipv4ToPorts : MonoBehaviour
{

    public IpToPortFormat[] m_ipv4ToPortFormats=new IpToPortFormat[] {

        new IpToPortFormat("{0}"),
        new IpToPortFormat("{0}:8080"),
        new IpToPortFormat("{0}:123"),
        new IpToPortFormat("ws://{0}:4615"),
        new IpToPortFormat("ws://{0}:4625"),
        new IpToPortFormat("wss://{0}:4625"),
    };
    public string m_lastIpv4Received;
    public UnityEvent<string> m_onIpReceived;

    public void PushIpv4ToUse(string ipv4) {

        foreach (IpToPortFormat ip in m_ipv4ToPortFormats) {

            if (ip != null) {

                ip.Invoke(ipv4);
            }
        }
        m_lastIpv4Received = ipv4;
        m_onIpReceived.Invoke(ipv4);

    }

    [System.Serializable]
    public class IpToPortFormat {

        public string m_format = "ws://{0}:3615";
        public UnityEvent<string> m_onUrlGeneratedWithIp;

        public IpToPortFormat(string format)
        {
            m_format = format;
        }
        public IpToPortFormat(){ }

        public void Invoke(string ipv4)
        {
            m_onUrlGeneratedWithIp.Invoke(string.Format(m_format, ipv4));
        }
    }
}
