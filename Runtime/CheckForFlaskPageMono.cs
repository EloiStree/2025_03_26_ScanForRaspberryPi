using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.ScanIP
{


    public class CheckForFlaskPageMono : MonoBehaviour {

        //MAYBE MAKE A FLASK SERVER WITH THE DEVICE MAC ADDRESS
        // http://{0}:8080/id
        public string m_url = "http://{0}:8080/hostname";
        public string m_expected = "raspberrypi";

        [Header("Debug")]
        public List<string> m_addressRangeZone = new List<string>();

        public List<FoundHttp> m_foundHttp = new List<FoundHttp>();
        public List<FoundHttp> m_foundHttpWithExpected = new List<FoundHttp>();

        public UnityEvent<string> m_onFoundIp;
        public UnityEvent<string> m_onFoundIpWithExpected;


        [System.Serializable]
        public class FoundHttp {
            public string m_address;
            public string m_result;
        }

        [ContextMenu("Refresh")]
        public void Refresh() {
            CheckPortCoroutineUtility.GetAllIpv4ToCheck(out List<string> addresses, out m_addressRangeZone);
            foreach (string address in addresses)
            {

                Action<string> a = (s) => {

                    var found = new FoundHttp()
                    {
                        m_address = address,
                        m_result = s,
                    };
                    m_foundHttp.Add(found);
                    m_onFoundIp.Invoke(address);
                   
                    if (s.Trim() == m_expected.Trim()) { 
                        m_foundHttpWithExpected.Add(found);
                        m_onFoundIpWithExpected.Invoke(address);
                    }
                };
                StartCoroutine(CheckPortCoroutineUtility
                    .IsReachableUrl(
                       string.Format( m_url, address), a ));
            }
        }

        
    }
}

