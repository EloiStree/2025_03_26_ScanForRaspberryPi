
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Eloi.ScanIP
{
    public class LookForWebsocketsPortMono : MonoBehaviour {



        public TargetPortToLookFor m_websockets = new TargetPortToLookFor()
        {

            m_port = 4615,
            m_portType = PortCheckType.Websocket
        };


        public List<string> m_addressesMask;
        public List<CheckPortCallBackResult> m_checkPortCallBacks;

        public TargetPortToLookForGroup m_targetGroup;
        public CheckPortCallBackResult m_result;
        public string m_address;

        public void Start()
        {
            Refresh();

        }

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            m_found = new List<string>();
            Action<string> action = (s) => { m_found.Add(s); };

            CheckPortCoroutineUtility.GetAllIpv4ToCheck(out List<string> addresses, out m_addressesMask);
            foreach (string address in addresses)
            {
                StartCoroutine(CheckPortCoroutineUtility
                    .IsReachable(
                        address, m_websockets, action));
                m_address = address;
            }
        }

        public List<string> m_found;
        
    }
}

