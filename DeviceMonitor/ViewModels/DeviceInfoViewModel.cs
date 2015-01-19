using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Serialization;
using DeviceMonitor.Dao;
using DeviceMonitor.Models.DeviceModels;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace DeviceMonitor.ViewModels
{
    [DataContract]
    public class DeviceInfoViewModel:DeviceBaseViewModel
    {
        private readonly DeviceInfo _deviceInfo;

        public DeviceInfoViewModel(DeviceInfo info) : base(info)
        {
            _deviceInfo = info;
        }

        /// <summary>
        /// ±¨¾¯×´Ì¬
        /// </summary>
        [DataMember]
        public override bool Alarmed
        {
            get { return _deviceInfo.DeviceDatas.Any(m => DataViewModel.GetDeviceData(m).Alarmed); }
        }

        public static DeviceInfoViewModel GetDeviceInfo(Guid id)
        {
            var info = DeviceContext.Instance.DeviceInfos.Find(id) ?? new DeviceInfo();
            return new DeviceInfoViewModel(info);
        }

        public static ICollection<DeviceInfoViewModel> GetDeviceInfo()
        {
            var infos = DeviceContext.Instance.DeviceInfos;
            var viewModels = new List<DeviceInfoViewModel>();
            infos.ForEach(m => viewModels.Add(GetDeviceInfo(m.id)));
            return viewModels;
        }

        public static List<DeviceDataViewModel> GetChildren(Guid id)
        {
            var info = DeviceContext.Instance.DeviceInfos.Find(id);
            var viewModels = new List<DeviceDataViewModel>();
            info.DeviceDatas.ForEach(m=>viewModels.Add(DeviceDataViewModel.GetDeviceData(m.id)));
            return viewModels;
        }


        public static DeviceInfoViewModel Copy(Guid dataId, Guid pId)
        {
            var item = GetDeviceInfo(dataId);
            if (item == null)
            {
                return null;
            }
            return item.Copy(pId);
        }

        private DeviceInfoViewModel Copy(Guid pId)
        {
            var itemCopy = _deviceInfo.Clone() as DeviceInfo;
            if (itemCopy != null)
            {
                itemCopy.parentId = pId;
                itemCopy.DeviceGroup = DeviceContext.Instance.DeviceGroups.Find(pId);
            }
            DeviceContext.Instance.DeviceInfos.AddOrUpdate(itemCopy);
            DeviceContext.Instance.SaveChanges();
            return new DeviceInfoViewModel(itemCopy);
        }

        public static DeviceInfoViewModel NewDeviceGroup(Guid id)
        {
            var group = DeviceContext.Instance.DeviceGroups.Find(id);
            var info = new DeviceInfo()
            {
                id = Guid.NewGuid(),
                nodeType = NodeType.Format,
                parentId = id,
                state = "open",
                index = group.DeviceInfos.Count,
                DeviceGroup = group,
                DeviceDatas = new Collection<DeviceData>()
            };
            return new DeviceInfoViewModel(info);
        }

        public static void Delete(Guid id)
        {
            var info = GetDeviceInfo(id);
            if (info != null)
            {
                info.Delete();
            }
        }

        public void Save()
        {
            DeviceContext.Instance.DeviceInfos.AddOrUpdate(_deviceInfo);
            DeviceContext.Instance.SaveChanges();
        }

        public void Delete()
        {
            DeviceContext.Instance.DeviceInfos.Remove(_deviceInfo);
            DeviceContext.Instance.SaveChanges();
        }

        [DataMember]
        public string ip
        {
            get { return _deviceInfo.ip; }
            set { _deviceInfo.ip = value; }
        }

        [DataMember]
        public int port
        {
            get { return _deviceInfo.port; }
            set { _deviceInfo.port = value; }
        }

        [DataMember]
        public byte commandByte
        {
            get { return _deviceInfo.commandByte; }
            set { _deviceInfo.commandByte = value; }
        }

        [DataMember]
        public Guid parentId
        {
            get { return _deviceInfo.parentId; }
            set { _deviceInfo.parentId = value; }
        }

        [DataMember]
        public int headerLength
        {
            get { return _deviceInfo.headerLength; }
            set { _deviceInfo.headerLength = value; }
        }


        [DataMember]
        public int dataLength
        {
            get { return _deviceInfo.dataLength; }
            set { _deviceInfo.dataLength = value; }
        }


        /// <summary>
        /// url
        /// </summary>
        [DataMember]
        public override string url
        {
            get { return _deviceInfo.DeviceGroup.ip; }
        }
    }
}