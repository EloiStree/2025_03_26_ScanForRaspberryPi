using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System;


namespace Eloi.ScanIP
{
    public class ScanForRaspberryOpenPortMono : MonoBehaviour {


        public List<RaspberryPiKownPort> m_scanDevices=new List<RaspberryPiKownPort> ();

        public Queue<string> m_toScan = new Queue<string> ();
        public void Update()
        {
            while (m_toScan.Count > 0) { 
            
                string ipAddress = m_toScan.Dequeue ();
                RaspberryPiKownPort device = new RaspberryPiKownPort();
                device.m_ipAddress = ipAddress;
                m_scanDevices.Add(device);
                Action<string> a = (s) => device.m_hasSsh_22.m_isPortReachable = true;
                StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                    ipAddress, new TargetPortToLookFor(22, PortCheckType.Websocket), a));
                Action<string> a2 = (s) => device.m_hasAsymIID_4615.m_isPortReachable = true;
                StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                    ipAddress, new TargetPortToLookFor(4615, PortCheckType.Websocket), a2));
                Action<string> a3 = (s) => device.m_hasAsymIID_4615.m_isPortReachable = true;
                StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                    ipAddress, new TargetPortToLookFor(4625, PortCheckType.Websocket), a3));
                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(80, PortCheckType.HTTP), device.m_hasHttp_80));
                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(8080, PortCheckType.HTTP), device.m_hasHttp_8080));
                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(443, PortCheckType.HTTPS), device.m_hasHttps_443));


                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(53, PortCheckType.TCP), device.m_hasDns_53));


                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(5353, PortCheckType.TCP), device.m_hasmDNS_5353));


                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(5900, PortCheckType.TCP), device.m_hasVnc_5900));
                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(1883, PortCheckType.TCP), device.m_hasMqtt_1883));
                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(4615, PortCheckType.TCP), device.m_hasAsymIID_4615));
                //StartCoroutine(CheckPortCoroutineUtility.IsReachable(
                //    ipAddress, new TargetPortToLookFor(4625, PortCheckType.TCP), device.m_hasTrustedIID_4625));
            }
           
        }
        public void AddScanInQueue(string ipAddress) { 
        
            m_toScan.Enqueue(ipAddress);

          

        }
    }
}

