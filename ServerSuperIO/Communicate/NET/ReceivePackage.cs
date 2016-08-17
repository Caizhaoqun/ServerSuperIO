using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    public class ReceivePackage : IReceivePackage
    {
        public ReceivePackage()
        {
            RemoteIP = String.Empty;
            RemotePort = -1;
            ListBytes = null;
            DeviceCode = String.Empty;
        }

        public ReceivePackage(string remoteIP, int remotePort, IList<byte[]> listBytes,string deviceCode)
        {
            RemoteIP = remoteIP;
            RemotePort = remotePort;
            ListBytes = listBytes;
            DeviceCode = deviceCode;
        }

        public IList<byte[]> ListBytes { get;  set; }

        public string RemoteIP { get;  set; }

        public int RemotePort { get;  set; }

        public string DeviceCode { get; set; }
    }
}
