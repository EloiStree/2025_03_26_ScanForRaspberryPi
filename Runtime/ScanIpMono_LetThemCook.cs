using Eloi.ScanIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.ScanIP
{
    public class ScanIpMono_LetThemCook : MonoBehaviour
    {
        public bool m_startScanAtEnable = true;
        public float m_delayBeforePushResult = 4;
        [Header("Debug")]
        public string m_guarantyTargetIp;
        public string m_lastAddressIpFound;
        public List<string> m_ipsFound;

        [Header("Event")]
        public UnityEvent m_onStartScanning;
        public UnityEvent<string> m_onIpFoundAfterScan;
        public UnityEvent m_onNothingFound;

        public A_LaunchableIpScanCoroutinesMono [] m_scanToLaunch;
        private void OnEnable()
        {
            if (m_startScanAtEnable)
                StartScanning();
        }

        [ContextMenu("Start Scanning")]
        public void StartScanning()
        {
            StartCoroutine(PushAfterDelay(m_delayBeforePushResult));
        }

        public IEnumerator PushAfterDelay(float delay)
        {
            foreach (var scan in m_scanToLaunch)
            {
                if (scan != null)
                    scan.LaunchIpScanCoroutinesIfActive();
            }
            m_onStartScanning.Invoke();
            yield return new WaitForSeconds(delay);
            PushResult();
        }
        public void PushResult()
        {

            if (string.IsNullOrWhiteSpace(m_guarantyTargetIp) &&
                string.IsNullOrWhiteSpace(m_lastAddressIpFound))
            {
                m_onNothingFound.Invoke();
            }
            else if (!string.IsNullOrWhiteSpace(m_guarantyTargetIp))
            {
                m_onIpFoundAfterScan.Invoke(m_guarantyTargetIp);
            }
            else if (!string.IsNullOrWhiteSpace(m_lastAddressIpFound))
            {
                m_onIpFoundAfterScan.Invoke(m_lastAddressIpFound);
            }
        }


        public void Clear()
        {
            m_guarantyTargetIp = "";
            m_lastAddressIpFound = "";
            m_ipsFound.Clear();
        }
        public void AddAddressFound(string address)
        {

            m_ipsFound.Add(address);
            m_lastAddressIpFound = address;
            RemoveDouble();
        }

        private void RemoveDouble()
        {
            m_ipsFound = m_ipsFound.Distinct().ToList();
        }

        public void SetAsGuarantyTargetIp(string value)
        {
            {

                m_guarantyTargetIp = value;
                AddAddressFound(value);
            }


        }
    }
}