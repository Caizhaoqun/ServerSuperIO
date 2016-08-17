using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Persistence;

namespace ServerSuperIO.Config
{
    [Serializable]
    public class GlobalConfig : XmlPersistence
    {

        public override string SavePath {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "ServerSuperIO/Config/";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return String.Format("{0}/GlobalConfig.xml", path);
            }
        }

        public override object Repair()
        {
            return new GlobalConfig();
        }
    }
}
