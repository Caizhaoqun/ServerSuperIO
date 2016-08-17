using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Protocol;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate
{
    public abstract class BaseChannel : ServerProvider,IChannel
    {
        private object _SyncLock = new object();
        public abstract CommunicateType CommunicationType { get; }
        public abstract bool IsDisposed { get; }
        public abstract string Key { get; }
        public abstract string SessionID { get; protected set; }
        public object SyncLock {
            get { return _SyncLock;}
        }

        protected abstract IList<byte[]> ReceiveDataFilter(IReceiveFilter receiveFilter);

        public abstract IChannel Channel { get; }
        public abstract void Close();
        public abstract void Dispose();
        public abstract void Initialize();

        public byte[] Read()
        {
            IList<byte[]> listBytes = ReceiveDataFilter(null);
            if (listBytes != null)
            {
                return listBytes[0];
            }
            else
            {
                return null;
            }
        }

        public IList<byte[]> Read(IReceiveFilter receiveFilter)
        {
            if (receiveFilter == null)
            {
                throw new NullReferenceException("receiveFilter为空");
            }

            return ReceiveDataFilter(receiveFilter);
        }

        public abstract int Write(byte[] data);
    }
}
