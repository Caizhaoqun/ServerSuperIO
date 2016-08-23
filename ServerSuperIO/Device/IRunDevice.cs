using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ServerSuperIO.DataCache;
using ServerSuperIO.Communicate;
using ServerSuperIO.Log;
using ServerSuperIO.Protocol;
using ServerSuperIO.Server;

namespace ServerSuperIO.Device
{
    public interface IRunDevice:IVirtualDevice
    {
        #region �����ӿ�

        /// <summary>
        /// ��ʼ���豸�������豸������ͷһ���¾��ǳ�ʼ���豸
        /// </summary>
        /// <param name="devid"></param>
        void Initialize(int devid);

        /// <summary>
        /// ����ԭʼ��byte����
        /// </summary>
        /// <param name="data"></param>
        void SaveBytes(byte[] data, string desc);
        
        /// <summary>
        /// ��÷������ݵ��������������û���������û��ʵʱ���ݺ���
        /// </summary>
        /// <returns></returns>
        byte[] GetSendBytes();

        /// <summary>
        /// �����ǰ�����û���������øú���,һ�㷵�ػ���豸��ʵʱ�������
        /// </summary>
        /// <returns></returns>
        byte[] GetConstantCommand();

        /// <summary>
        /// ����IO���ݽӿ�
        /// </summary>
        /// <param name="io"></param>
        /// <param name="senddata"></param>
        int Send(IChannel io, byte[] senddata);

        /// <summary>
        /// ��ȡIO���ݽӿ�
        /// </summary>
        /// <param name="io"></param>
        /// <returns></returns>
        byte[] Receive(IChannel io);

        /// <summary>
        /// ����������Ϣ����������
        /// </summary>
        /// <param name="io"></param>
        /// <param name="receiveFilter"></param>
        /// <returns></returns>
        IList<byte[]> Receive(IChannel io, IReceiveFilter receiveFilter);

        /// <summary>
        /// ͬ�������豸��IO��
        /// </summary>
        /// <param name="io">ioʵ������</param>
        void Run(IChannel io);

        /// <summary>
        /// ͬ�������豸��byte[]��
        /// </summary>
        /// <param name="key"></param>
        /// <param name="channel"></param>
        /// <param name="revData">���յ�������</param>
        void Run(string key, IChannel channel, byte[] revData);

        /// <summary>
        /// ���ͨѶ�����������������������
        /// </summary>
        /// <param name="info"></param>
        void Communicate(IRequestInfo info);

        /// <summary>
        /// ͨѶ�жϣ�δ���յ�����
        /// </summary>
        void CommunicateInterrupt(IRequestInfo info);

        /// <summary>
        /// ͨѶ�����ݴ�����ܵ�����
        /// </summary>
        void CommunicateError(IRequestInfo info);

        /// <summary>
        /// ͨѶδ֪��Ĭ��״̬��һ�㲻�ã�
        /// </summary>
        void CommunicateNone();

        /// <summary>
        /// ���ͨѶ״̬
        /// </summary>
        /// <param name="revdata"></param>
        /// <returns></returns>
        CommunicateState CheckCommunicateState(byte[] revdata);

        /// <summary>
        /// �����ӿں���
        /// </summary>
        void Alert();

        /// <summary>
        /// ��������������
        /// </summary>
        void Save();

        /// <summary>
        /// չʾ
        /// </summary>
        void Show();

        /// <summary>
        /// ��ͨѶʵ��ΪNULL��ʱ�򣬵��øú���
        /// </summary>
        void UnknownIO();

        /// <summary>
        /// ͨѶ״̬�ı�
        /// </summary>
        /// <param name="comState">�ı���״̬</param>
        void CommunicateStateChanged(CommunicateState comState);

        /// <summary>
        /// ͨ��״̬�ı�
        /// </summary>
        /// <param name="channelState"></param>
        void ChannelStateChanged(ChannelState channelState);

        ///// <summary>
        ///// �������ӣ������˺�����ֻ����������ͨѶ
        ///// </summary>
        //void SocketConnect(string ip, int port);

        ///// <summary>
        ///// �Ͽ����ӣ������˺�����ֻ����������ͨѶ
        ///// </summary>
        ///// <param name="ip"></param>
        ///// <param name="port"></param>
        //void SocketDisconnect(string ip, int port);

        /// <summary>
        /// ������رյ�ʱ�䣬��Ӧ�豸�˳�����
        /// </summary>
        void Exit();

        /// <summary>
        /// ɾ���豸����Ӧ�ӿں���
        /// </summary>
        void Delete();

        /// <summary>
        /// �����Զ��巵���ݶ��������������������
        /// </summary>
        /// <returns></returns>
        object GetObject();

        /// <summary>
        /// �豸��ʱ������Ӧ��ʱ����
        /// </summary>
        void OnRunTimer();

        /// <summary>
        /// ��ʾ�����Ĳ˵�
        /// </summary>
        void ShowContextMenu();

        /// <summary>
        /// ��ʾIO�������Ĵ���
        /// </summary>
        void ShowMonitorDialog();

        /// <summary>
        /// ��IO����������ʾbyte[]����
        /// </summary>
        /// <param name="data"></param>
        /// <param name="desc"></param>
        void ShowMonitorData(byte[] data, string desc);
        #endregion

        #region ���Խӿ�
        /// <summary>
        /// Ĭ�ϳ���ID�����ڴ洢��ʱ����
        /// </summary>
        object Tag { set; get; }

        /// <summary>
        /// ͬ����������IO����
        /// </summary>
        object SyncLock { get; }

        /// <summary>
        /// ʵʱ���ݳ־ýӿ�
        /// </summary>
        IDeviceDynamic DeviceDynamic { get; }

        /// <summary>
        /// �豸�����־ýӿ�
        /// </summary>
        IDeviceParameter DeviceParameter { get; }

        /// <summary>
        /// Э������
        /// </summary>
        IProtocolDriver Protocol { get; }

        /// <summary>
        /// �Ƿ���ʱ�ӣ���ʶ�Ƿ����OnRunTimer�ӿں�����
        /// </summary>
        bool IsRunTimer { set; get;}

        /// <summary>
        /// ʱ�Ӽ��ֵ����ʶ��ʱ����DeviceTimer�ӿں���������
        /// </summary>
        int RunTimerInterval { set; get; }

        /// <summary>
        /// �豸������
        /// </summary>
        DeviceType DeviceType { get;}

        /// <summary>
        /// �豸���
        /// </summary>
        string ModelNumber { get;}

        /// <summary>
        /// �豸����Ȩ�޼���������м���ߵĻ��������ȷ��ͺͽ������ݡ�
        /// </summary>
        DevicePriority DevicePriority { get;set;}

        /// <summary>
        /// �豸��ͨѶ����
        /// </summary>
        CommunicateType CommunicateType { get;set;}

        /// <summary>
        /// ��ʶ�Ƿ������豸�����Ϊfalse�����������豸�ӿ�ʱֱ�ӷ���
        /// </summary>
        bool IsRunDevice{ get;set;}

        /// <summary>
        /// �Ƿ��ͷ���Դ
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// ��ʾ��ͼ
        /// </summary>
        Control DeviceGraphics { get;}
        #endregion

        #region �¼��ӿ�
        ///// <summary>
        ///// ���������¼�
        ///// </summary>
        //event ReceiveDataHandler ReceiveData;

        //// <summary>
        //// �������������¼�
        //// </summary>
        //// <param name="revdata"></param>
        ///// <summary>
        ///// ���������¼�����ReceiveDataHandler�¼��ķ�װ
        ///// </summary>
        //void OnReceiveData(byte[] revdata);

        /// <summary>
        /// ���������¼�
        /// </summary>
        event SendDataHandler SendData;

        /// <summary>
        /// ���������¼�����SendDataHandler�¼��ķ�װ
        /// </summary>
        /// <param name="senddata"></param>
        void OnSendData(byte[] senddata);

        /// <summary>
        /// �豸��־����¼�
        /// </summary>
        event DeviceRuningLogHandler DeviceRuningLog;

        /// <summary>
        /// ���м�������ʾ��־�¼�����DeviceRuningLogHandler�¼��ķ�װ
        /// </summary>
        void OnDeviceRuningLog(string statetext);

        /// <summary>
        /// ���ڲ����ı��¼�
        /// </summary>
        event ComParameterExchangeHandler ComParameterExchange;
        /// <summary>
        /// ���ڲ����ı��¼�����COMParameterExchangeHandler�¼��ķ�װ
        /// </summary>
        void OnComParameterExchange(int oldcom, int oldbaud, int newcom, int newbaud);

        /// <summary>
        /// �豸���ݶ���ı��¼�
        /// </summary>
        event DeviceObjectChangedHandler DeviceObjectChanged;
        /// <summary>
        /// ���������¼�����DeviceObjectChangedHandler�¼��ķ�װ
        /// </summary>
        void OnDeviceObjectChanged(object obj);

        ///// <summary>
        ///// ɾ���豸�¼�
        ///// </summary>
        //event DeleteDeviceCompletedHandler DeleteDeviceCompleted;
        ///// <summary>
        ///// ɾ���豸�¼�����DeleteDeviceHandler�¼��ķ�װ
        ///// </summary>
        //void OnDeleteDeviceCompleted();
        #endregion
    }
}
