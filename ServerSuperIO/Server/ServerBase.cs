using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Base;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Log;
using ServerSuperIO.Service;
using ServerSuperIO.Show;

namespace ServerSuperIO.Server
{
    public abstract class ServerBase:IServer
    {
        private Manager<string, IGraphicsShow> _Shows;
        private Manager<string, IService> _Services;

        private MonitorException _monitorException;

        internal ServerBase(IServerConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("配制信息","参数为空");
            }
            ServerConfig = config;

            _Shows = new Manager<string, IGraphicsShow>();
            _Services = new Manager<string, IService>();

            _monitorException =new MonitorException();

            Logger = (new LogFactory()).GetLog(ServerName);
            DeviceManager=new DeviceManager();
            ChannelManager=new ChannelManager();
            ControllerManager=new ControllerManager();

            IsDisposed = false;
        }

        ~ServerBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName
        {
            get { return ServerConfig.ServerName; }
        }

        /// <summary>
        /// 开始服务
        /// </summary>
        public virtual void Start()
        {
            _monitorException.Setup(this);
            _monitorException.Monitor();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public virtual void Stop()
        {
            Dispose(true);
        }

        /// <summary>
        /// 增加设备
        /// </summary>
        /// <param name="dev"></param>
        /// <returns>设备ID</returns>
        public abstract void AddDevice(Device.IRunDevice dev);

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public abstract void RemoveDevice(int devid);

        public bool AddGraphicsShow(Show.IGraphicsShow graphicsShow)
        {
            if (!_Shows.ContainsKey(graphicsShow.ThisKey))
            {
                graphicsShow.MouseRightContextMenu += GraphicsShow_MouseRightContextMenu;
                graphicsShow.GraphicsShowClosed += GraphicsShow_GraphicsShowClosed;
                if (_Shows.TryAdd(graphicsShow.ThisKey, graphicsShow))
                {
                    Logger.Info(true, String.Format("<{0}>显示视图显示成功", graphicsShow.ThisName));
                    try
                    {
                        graphicsShow.Show();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(true, "", ex);
                    }
                    return true;
                }
                else
                {
                    Logger.Info(true, String.Format("<{0}>显示视图显示失败", graphicsShow.ThisName));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool RemoveGraphicsShow(string showKey)
        {
            IGraphicsShow gShow;
            if (_Shows.TryRemove(showKey, out gShow))
            {
                gShow.MouseRightContextMenu -= GraphicsShow_MouseRightContextMenu;
                gShow.GraphicsShowClosed -= GraphicsShow_GraphicsShowClosed;
                gShow.Close();
                gShow.Dispose();

                Logger.Info(true, String.Format("<{0}>显示视图关闭成功", gShow.ThisName));
                return true;
            }
            else
            {
                Logger.Info(true, String.Format("<{0}>显示视图关闭失败", gShow.ThisName));
                return false;
            }
        }

        protected void RemoveDeviceFromShows(IRunDevice dev)
        {
            //--------------删除动态显示的实时数据----------------------//
            foreach (IGraphicsShow show in this._Shows.Values)
            {
                show.RemoveDevice(dev.DeviceParameter.DeviceID);
            }
        }

        private void GraphicsShow_GraphicsShowClosed(object key)
        {
            RemoveGraphicsShow(key.ToString());
        }

        private void GraphicsShow_MouseRightContextMenu(int devid)
        {
            IRunDevice dev = DeviceManager.GetDevice(devid);
            if (dev != null)
            {
                try
                {
                    dev.ShowContextMenu();
                }
                catch (Exception ex)
                {
                    Logger.Error(true, ex.Message);
                }
            }
            else
            {
                Logger.Info(false, "未找到能够显示菜单的设备");
            }
        }

        public bool AddService(IService service)
        {
            if (!_Services.ContainsKey(service.ThisKey))
            {
                if (_Services.TryAdd(service.ThisKey, service))
                {
                    try
                    {
                        service.StartService();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(true, "", ex);
                    }

                    Logger.Info(true, String.Format("<{0}>增加服务成功", service.ThisName));
                    return true;
                }
                else
                {
                    Logger.Info(true, String.Format("<{0}>增加服务失败", service.ThisName));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool RemoveService(string serviceKey)
        {
            IService service;
            if (_Services.TryRemove(serviceKey, out service))
            {
                service.StopService();
                service.Dispose();
                Logger.Info(true, String.Format("<{0}>关闭服务成功", service.ThisName));
                return true;
            }
            else
            {
                Logger.Info(true, String.Format("<{0}>关闭服务失败", service.ThisName));
                return false;
            }
        }

        protected void RemoveDeviceFromServices(IRunDevice dev)
        {
            //---------------------删除服务数据-----------------------//
            foreach (IService service in this._Services.Values)
            {
                service.RemoveDevice(dev.DeviceParameter.DeviceID);
            }
        }

        /// <summary>
        /// 改变设备的串口信息
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="oldCom"></param>
        /// <param name="oldBaud"></param>
        /// <param name="newCom"></param>
        /// <param name="newBaud"></param>
        public abstract void ChangeDeviceComInfo(int devid, int oldCom, int oldBaud, int newCom, int newBaud);

        /// <summary>
        /// 设备管理器
        /// </summary>
        public Device.IDeviceManager<int, Device.IRunDevice> DeviceManager { get; private set; }

        /// <summary>
        /// IO通道管理器
        /// </summary>
        public Communicate.IChannelManager<string, Communicate.IChannel> ChannelManager { get; private set; }

        /// <summary>
        /// 控制管理器
        /// </summary>
        public Communicate.IControllerManager<string, Communicate.IController> ControllerManager { get; private set; }

        /// <summary>
        /// 增加设备完成事件
        /// </summary>
        public event AddDeviceCompletedHandler AddDeviceCompleted;

        /// <summary>
        /// 增加设备完成事件
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="devName"></param>
        /// <param name="isSuccess"></param>
        protected virtual void OnAddDeviceCompleted(int devid, string devName,bool isSuccess)
        {
            if (AddDeviceCompleted != null)
            {
                AddDeviceCompleted(devid, devName, isSuccess);
            }
        }

        /// <summary>
        /// 删除设备完成事件
        /// </summary>
        public event DeleteDeviceCompletedHandler DeleteDeviceCompleted;

        /// <summary>
        /// 删除设备完成事件
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="devName"></param>
        /// <param name="isSuccess"></param>
        protected virtual void OnDeleteDeviceCompleted(int devid, string devName,bool isSuccess)
        {
            if (DeleteDeviceCompleted != null)
            {
                DeleteDeviceCompleted(devid, devName, isSuccess);
            }
        }

        /// <summary>
        /// 串口打开事件
        /// </summary>
        public event ComOpenedHandler ComOpened;

        /// <summary>
        /// 串口打开事件
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <param name="openSuccess"></param>
        protected virtual void OnComOpened(int port, int baud,bool openSuccess)
        {
            if (ComOpened != null)
            {
                ComOpened(port, baud, openSuccess);
            }
        }

        /// <summary>
        /// 串口关闭事件
        /// </summary>
        public event ComClosedHandler ComClosed;

        /// <summary>
        /// 串口关闭事件
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <param name="closeSuccess"></param>
        protected virtual void OnComClosed(int port, int baud,bool closeSuccess)
        {
            if (ComClosed != null)
            {
                ComClosed(port, baud, closeSuccess);
            }
        }

        /// <summary>
        /// 网络连接事件
        /// </summary>
        public event SocketConnectedHandler SocketConnected;

        /// <summary>
        /// 网络连接事件
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        protected virtual void OnSocketConnected(string ip, int port)
        {
            if (SocketConnected != null)
            {
                SocketConnected(ip, port);
            }
        }

        /// <summary>
        /// 网络断开事件
        /// </summary>
        public event SocketClosedHandler SocketClosed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        protected virtual void OnSocketClosed(string ip, int port)
        {
            if (SocketClosed != null)
            {
                SocketClosed(ip, port);
            }
        }

        /// <summary>
        /// 日志实例
        /// </summary>
        public Log.ILog Logger { get; private set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public Config.IServerConfig ServerConfig { get; private set; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    ControllerManager.RemoveAllController();
                    ChannelManager.RemoveAllChannel();

                    ICollection<IRunDevice> devs = DeviceManager.GetValues();
                    foreach (IRunDevice dev in devs)
                    {
                        dev.Exit();
                    }

                    DeviceManager.RemoveAllDevice();

                    if (_Shows != null)
                    {
                        foreach (KeyValuePair<string, IGraphicsShow> show in _Shows)
                        {
                            try
                            {
                                show.Value.Close();
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        _Shows.Clear();
                    }

                    if (_Services != null)
                    {
                        foreach (KeyValuePair<string, IService> service in _Services)
                        {
                            try
                            {
                                service.Value.StopService();
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        _Services.Clear();
                    }
                }

                _monitorException.UnMonitor();

                IsDisposed = true;

                Logger.InfoFormat(false, "{0}-{1}", ServerName, "已经停止");
            }
        }

        private bool IsDisposed { get; set; }

        protected void OnChannelChanged(string comPara1, CommunicateType comType, ChannelState channelState)
        {
            if (DeviceManager.Count > 0)
            {
                if (ServerConfig.ControlMode == ControlMode.Loop
                    || ServerConfig.ControlMode == ControlMode.Self
                    || ServerConfig.ControlMode == ControlMode.Parallel)
                {

                    IRunDevice[] list = this.DeviceManager.GetDevices(comPara1, comType);
                    if (list != null && list.Length > 0)
                    {
                        foreach (IRunDevice dev in list)
                        {
                            try
                            {
                                dev.ChannelStateChanged(channelState);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(true, ex.Message);
                            }
                        }
                    }
                }
            }
        }

        protected void SendData(object source, SendDataArgs e)
        {
            if (e == null)
                return;

            if (e.Data == null || e.Data.Length <= 0)
            {
                Logger.Info(false, e.DeviceName + ",要发送的数据为空");
                return;
            }

            IRunDevice dev = DeviceManager.GetDevice(e.DeviceID);

            if (dev != null)
            {
                if (dev.CommunicateType == CommunicateType.COM)
                {
                    Logger.Info(false, e.DeviceName + ",串口通讯设备无法实现自主发送数据");
                }
                else
                {
                    if (ServerConfig.ControlMode == ControlMode.Self)
                    {
                        ISocketController netController = (ISocketController)ControllerManager.GetController(SocketController.ConstantKey);
                        if (netController != null)
                        {
                            netController.Send(dev, e.Data);
                        }
                        else
                        {
                            Logger.Info(false, e.DeviceName + ",无法找到对应的网络控制器");
                        }
                    }
                    else
                    {
                        Logger.Info(false, e.DeviceName + ",只有控制方式为自主模式的时候，设备才能发送数据");
                    }
                }
            }
            else
            {
                Logger.Info(false, e.DeviceName + "无法获得可发送数据的设备");
            }
        }

        protected void DeviceObjectChanged(object source, DeviceObjectChangedArgs e)
        {
            if (e == null) return;

            try
            {
                //---------------------实时数据显示--------------------//
                ICollection<IGraphicsShow> showList = _Shows.Values;
                foreach (IGraphicsShow show in showList)
                {
                    if (!show.IsDisposed)
                    {
                        show.UpdateDevice(e.DeviceID, e.Object);
                    }
                }
                //---------------------服务输出-----------------------//
                ICollection<IService> serviceList = _Services.Values;
                foreach (IService app in serviceList)
                {
                    if (!app.IsDisposed)
                    {
                        app.UpdateDevice(e.DeviceID, e.Object);
                    }
                }

                //---------------------检测虚拟设备-------------------//
                if (e.DeviceType != DeviceType.Virtual)
                {
                    IRunDevice[] vdevList = DeviceManager.GetDevices(DeviceType.Virtual);
                    foreach (IRunDevice dev in vdevList)
                    {
                        dev.RunVirtualDevice(e.DeviceID, e.Object);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(true, ex.Message);
            }
        }

        protected void DeviceRuningLog(object source, DeviceRuningLogArgs e)
        {
            Logger.Info(false, String.Format("{0}>>{1}", e.DeviceName, e.StateDesc));
        }
    }
}
