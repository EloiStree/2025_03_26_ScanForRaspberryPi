namespace Eloi.ScanIP
{
    [System.Serializable]
    public class RaspberryPiKownPort
    {
        public string m_ipAddress;
        public CheckPortCallBackResult  m_hasSsh_22;
        public CheckPortCallBackResult  m_hasHttp_80;
        public CheckPortCallBackResult  m_hasHttp_8080;
        public CheckPortCallBackResult  m_hasHttps_443;
        public CheckPortCallBackResult  m_hasDns_53;
        public CheckPortCallBackResult  m_hasmDNS_5353;
        public CheckPortCallBackResult  m_hasVnc_5900;
        public CheckPortCallBackResult  m_hasMqtt_1883;
        public CheckPortCallBackResult  m_hasAsymIID_4615;
        public CheckPortCallBackResult  m_hasTrustedIID_4625;
    }
}

