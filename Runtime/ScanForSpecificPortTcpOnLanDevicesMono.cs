using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Eloi.ScanIP
{
    public class ScanForSpecificPortTcpOnLanDevicesMono : MonoBehaviour {

        public int m_tcpPort = 22;
        public bool m_refreshAtStart;

        [Header("Debug")]
        public List<string> m_addressRangeZone = new List<string>();
        public List<TargetIpPortToLookFor> m_deviceWithPortAccessible;
        public UnityEvent<string> m_onSshIpFound;


        public void Start()
        {
            if (m_refreshAtStart)
                Refresh();
        }

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            TargetPortToLookFor ssh = new TargetPortToLookFor(m_tcpPort, PortCheckType.Websocket);
            m_deviceWithPortAccessible = new List<TargetIpPortToLookFor>();
            m_addressRangeZone = new List<string>();
            Action<TargetIpPortToLookFor> action = (s) => {
                m_onSshIpFound?.Invoke(s.m_ip);
                m_deviceWithPortAccessible.Add(s);
            };

            CheckPortCoroutineUtility.GetAllIpv4ToCheck(out List<string> addresses, out m_addressRangeZone);
            foreach (string address in addresses)
            {
                StartCoroutine(CheckPortCoroutineUtility
                    .IsReachable(
                        address, ssh, action));
            }
        }
    }
}

