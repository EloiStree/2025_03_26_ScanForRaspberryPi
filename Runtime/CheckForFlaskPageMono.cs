using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.ScanIP
{

    public interface I_LaunchableIpScanCoroutines {

        void LaunchIpScanCoroutines();
    }

    public class CheckForFlaskPageMono : A_LaunchableIpScanCoroutinesMono
    {

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

        public UnityEvent<string> m_onDebugProcess;

        public UnityEvent<string> m_onDebugProcessIps;

        [System.Serializable]
        public class FoundHttp {
            public string m_address;
            public string m_result;
        }

        [ContextMenu("Refresh")]
        public override void LaunchIpScanCoroutines() {
            m_callback.Clear();
            CheckPortCoroutineUtility.GetAllIpv4ToCheck(out List<string> addresses, out m_addressRangeZone);
            CheckPortCoroutineUtility.GetLocalIpRangeGroup(out List<string> ips, false);


            m_onDebugProcessIps.Invoke(string.Join(" , ", ips.ToArray()));



            foreach (string address in addresses)
            {
                if (ips.Contains(address))
                    continue;
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
                CheckPortCallBackResult resultCheck = new CheckPortCallBackResult();
                m_callback.Add(resultCheck);
                StartCoroutine(CheckPortCoroutineUtility
                    .IsReachableUrl(
                       string.Format( m_url, address), a, resultCheck));
            }
        }
        public List<CheckPortCallBackResult> m_callback;
        public float m_percentDone;

        public void Update()
        {
            if (m_callback != null && m_callback.Count > 0)
            {
                m_percentDone = m_callback.Where(c => c.m_finishedCoroutine).Count() / (float)m_callback.Count;
            }
            else { 
            
                m_percentDone = 0;
            }
            string debug = $@"
    PCT: {m_percentDone}
    Count: {m_callback.Count}
    Count: {m_callback.Count}
    RangeZone: {string.Join(" , ",m_addressRangeZone)}
";
            m_onDebugProcess.Invoke(debug);
        }


    }
}

