using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


namespace Eloi.ScanIP
{


    public class LookForDeviceIpFromPortMono : MonoBehaviour
    {
        public List<string> m_addressesMask;
        public List<string> m_addresses;
        public List<CheckPortCallBackResult> m_checkPortCallBacks;

        public TargetPortToLookForGroup m_targetGroup;
        public string m_ip="192.168.1.126";
        public CheckPortCallBackResult m_result;
        public CheckPortIpCallBackResult m_ipResult;
        public CheckPortIpCallBackResult m_ipResultValide;
        public string m_address;

        public void Start()
        {
            Refresh();

        }

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            StartCoroutine(Coroutine_Refresh());
        }

        public IEnumerator Coroutine_Refresh()
        {
            CheckPortCoroutineUtility.GetAllIpv4ToCheck(out m_addresses, out m_addressesMask);
            foreach (string address in m_addresses)
            {
                yield return CheckPortCoroutineUtility
                    .IsAllPortReachable(
                        address, m_targetGroup, m_result, m_ipResult);

                if (m_ipResult.m_isPortReachable)
                    m_ipResultValide = m_ipResult;
                m_address = address;
                yield return new WaitForSeconds(0.1f);
            }
            yield break;


        }
    }
}

