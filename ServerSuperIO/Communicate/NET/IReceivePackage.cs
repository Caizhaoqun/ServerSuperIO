using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    public interface IReceivePackage
    {
        string RemoteIP { get; set; }
        int RemotePort { get; set; }
        IList<byte[]> ListBytes { get; set; }
        string DeviceCode { get; set; }
    }
}
