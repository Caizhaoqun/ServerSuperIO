using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ServerSuperIO.Base;
using ServerSuperIO.DataCache;
using ServerSuperIO.Protocol;

namespace ServerSuperIO.Device
{
    public abstract class ProtocolDriver:IProtocolDriver
    {
        private Manager<string,IProtocolCommand> _Commands = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        protected ProtocolDriver()
        {
            _Commands = new Manager<string, IProtocolCommand>();
            SendCache=new SendCache();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ProtocolDriver()
        {
            if (_Commands != null)
            {
                _Commands.Clear();
                _Commands = null;
            }

            if (SendCache != null && SendCache.Count > 0)
            {
                SendCache.Clear();
            }
        }

        /// <summary>
        /// 初始化驱动
        /// </summary>
        public virtual void InitDriver(IRunDevice runDevice,IReceiveFilter receiveFilter)
        {
            ReceiveFilter = receiveFilter;

            this._Commands.Clear();
            System.Reflection.Assembly asm = runDevice.GetType().Assembly;
            Type[] types = asm.GetTypes();
            foreach (Type t in types)
            {
                if (typeof(IProtocolCommand).IsAssignableFrom(t))
                {
                    if (t.Name != "IProtocolCommand" && t.Name != "ProtocolCommand")
                    {
                        IProtocolCommand cmd = (IProtocolCommand)t.Assembly.CreateInstance(t.FullName);
                        if (cmd != null)
                        {
                            cmd.Setup(this);
                            _Commands.TryAdd(cmd.Name, cmd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public IProtocolCommand GetProcotolCommand(string cmdName)
        {
            IProtocolCommand cmd;
            if (this._Commands!=null && this._Commands.TryGetValue(cmdName, out cmd))
            {
                return cmd;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object DriverAnalysis(string cmdName, byte[] data, object obj)
        {
            IProtocolCommand cmd = GetProcotolCommand(cmdName);
            if (cmd != null)
            {
                return cmd.Analysis(data, obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 打包数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="cmdName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] DriverPackage(int addr, string cmdName, object obj)
        {
            IProtocolCommand cmd = GetProcotolCommand(cmdName);
            if (cmd != null)
            {
                return cmd.Package(addr,obj);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 校验数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract bool CheckData(byte[] data);

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract byte[] GetCommand(byte[] data);

        /// <summary>
        /// 获得地址
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract int GetAddress(byte[] data);

        /// <summary>
        /// 获得校验数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract byte[] GetCheckData(byte[] data);

        /// <summary>
        /// 获得ID信息，是该传感器的唯一标识。2016-07-29新增加（wxzz)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract string GetCode(byte[] data);

        /// <summary>
        /// 获得应该接收的数据长度，如果当前接收的数据小于这个返回值，那么继续接收数据，直到大于等于这个返回长度。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract int GetPackageLength(byte[] data);

        /// <summary>
        /// 协议头
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract byte[] GetHead(byte[] data);

        /// <summary>
        /// 协议尾
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract byte[] GetEnd(byte[] data);

        /// <summary>
        /// 发送数据缓存
        /// </summary>
        public ISendCache SendCache { private set; get; }

        /// <summary>
        /// 协议过滤器
        /// </summary>
        public IReceiveFilter ReceiveFilter { set; get; }
    }
}
