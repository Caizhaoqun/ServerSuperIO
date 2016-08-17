using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.DataCache;
using ServerSuperIO.Device;

namespace ServerSuperIO.Protocol
{
    public interface IProtocolDriver
    {
        /// <summary>
        /// 初始化协议驱动
        /// </summary>
        void InitDriver(IRunDevice runDevice,IReceiveFilter receiveFilter);

        /// <summary>
        /// 获得协议命令
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        IProtocolCommand GetProcotolCommand(string cmdName);

        /// <summary>
        /// 驱动解析
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        object DriverAnalysis(string cmdName, byte[] data, object obj);

        /// <summary>
        /// 驱动打包
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="cmdName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] DriverPackage(int addr, string cmdName, object obj);

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>true:校验成功 false:校验失败</returns>
        bool CheckData(byte[] data);

        /// <summary>
        /// 获得校验数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetCheckData(byte[] data);

        /// <summary>
        /// 获得命令集全，如果命令和命令参数
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>返回命令集合</returns>
        byte[] GetCommand(byte[] data);

        /// <summary>
        /// 获得地址
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>返回地址</returns>
        int GetAddress(byte[] data);

        /// <summary>
        /// 获得ID信息，是该传感器的唯一标识。2016-07-29新增加（wxzz)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GetCode(byte[] data);

        /// <summary>
        /// 获得应该接收的数据长度，如果当前接收的数据小于这个返回值，那么继续接收数据，直到大于等于这个返回长度。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int GetPackageLength(byte[] data);

        /// <summary>
        /// 协议头
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetHead(byte[] data);

        /// <summary>
        /// 协议尾
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetEnd(byte[] data);

        /// <summary>
        /// 命令缓存器，把要发送的命令数据放到这里，框架会自动提取数据进行发送
        /// </summary>
        ISendCache SendCache { get; }

        /// <summary>
        /// 协议过滤器
        /// </summary>
        IReceiveFilter ReceiveFilter { set;get; }
    }
}
