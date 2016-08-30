using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Device;
using ServerSuperIO.Protocol;

namespace TestDeviceDriver
{
    internal class DeviceProtocol:ProtocolDriver
    {
        public override bool CheckData(byte[] data)
        {
            byte checkSum = 0;
            for (int i = 2; i < data.Length - 2; i++)
            {
                checkSum += data[i];
            }

            if (data[data.Length - 2] == checkSum)
                return true;
            else
                return false;
        }

        public override byte[] GetCommand(byte[] data)
        {
            return new byte[] { data[3] };
        }

        public override int GetAddress(byte[] data)
        {
            return data[2];
        }

        public override byte[] GetHead(byte[] data)
        {
            return new byte[] { data[0], data[1] };
        }

        public override byte[] GetEnd(byte[] data)
        {
            return new byte[] { data[data.Length - 1] };
        }

        public override byte[] GetCheckData(byte[] data)
        {
            byte checkSum = 0;
            for (int i = 2; i < data.Length - 2; i++)
            {
                checkSum += data[i];
            }
            return new byte[] { checkSum };
        }

        public override string GetCode(byte[] data)
        {
            byte[] head = new byte[] {0x55, 0xaa};
            int codeIndex = data.Mark(0, data.Length, head);
            if (codeIndex == -1)
            {
                return String.Empty;
            }
            else
            {
                return data[codeIndex + head.Length].ToString("00#");
            }
        }

        public override int GetPackageLength(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
