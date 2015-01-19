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
    public class DeviceGroupViewModel:DeviceBaseViewModel
    {
        private readonly DeviceGroup _deviceGroup;
        private DeviceGroupViewModel(DeviceGroup group) : base(group)
        {
            _deviceGroup = group;
        }
        /// <summary>
        /// 报警状态
        /// </summary>
        [DataMember]
        public override bool Alarmed
        {
            get { return _deviceGroup.DeviceInfos.Any(m => DeviceInfoViewModel.GetDeviceInfo(m.id).Alarmed); }
        }
        public static DeviceGroupViewModel GetDeviceGroup(Guid id)
        {
            var data = DeviceContext.Instance.DeviceGroups.Find(id)??new DeviceGroup();
            return new DeviceGroupViewModel(data);
        }
        public static List<DeviceInfoViewModel> GetChildren(Guid id)
        {
            var info = DeviceContext.Instance.DeviceGroups.Find(id);
            if (info == null)
            {
                return DeviceInfoViewModel.GetDeviceInfo().ToList();
            }
            var viewModels = new List<DeviceInfoViewModel>();
            info.DeviceInfos.ForEach(m => viewModels.Add(DeviceInfoViewModel.GetDeviceInfo(m.id)));
            return viewModels;
        }
        public static ICollection<DeviceGroupViewModel> GetDeviceGroup()
        {
            var infos = DeviceContext.Instance.DeviceGroups;
            var viewModels = new List<DeviceGroupViewModel>();
            infos.ForEach(m => viewModels.Add(GetDeviceGroup(m.id)));
            return viewModels;
        }

        public static  DeviceGroupViewModel Copy(Guid dataId,Guid pId)
        {
            var item = GetDeviceGroup(dataId);
            if (item == null)
            {
                return null;
            }
            return item.Copy(pId);
        }

        private DeviceGroupViewModel Copy(Guid pId)
        {
            var itemCopy = _deviceGroup.Clone() as DeviceGroup;
            if (itemCopy != null)
            {
                itemCopy.parentId = pId;
                itemCopy.DeviceFactory = DeviceContext.Instance.DeviceFactories.Find(pId);
            }
            DeviceContext.Instance.DeviceGroups.AddOrUpdate(itemCopy);
            DeviceContext.Instance.SaveChanges();
            return new DeviceGroupViewModel(itemCopy);
        }
        public static DeviceGroupViewModel NewDeviceGroup(Guid id)
        {
            var factory = DeviceContext.Instance.DeviceFactories.Find(id);
            var group = new DeviceGroup()
            {
                id = Guid.NewGuid(),
                nodeType = NodeType.Format,
                parentId = id,
                state = "open",
                index = factory.DeviceGroups.Count,
                DeviceFactory = factory,
                DeviceInfos = new Collection<DeviceInfo>()
            };
            return new DeviceGroupViewModel(group);
        }
        public static void Delete(Guid id)
        {
            var info = GetDeviceGroup(id);
            if (info != null)
            {
                info.Delete();
            }
        }

        public void Save()
        {
            DeviceContext.Instance.DeviceGroups.AddOrUpdate(_deviceGroup);
            DeviceContext.Instance.SaveChanges();
        }
        public void Delete()
        {
            DeviceContext.Instance.DeviceGroups.Remove(_deviceGroup);
            DeviceContext.Instance.SaveChanges();
        }

        [DataMember]
        public int refreshTime
        {
            get { return _deviceGroup.refreshTime; }
            set { _deviceGroup.refreshTime = value; }
        }


        [DataMember]
        public Guid parentId
        {
            get { return _deviceGroup.parentId; }
            set { _deviceGroup.parentId = value; }
        }

        [DataMember]
        public int timeOut
        {
            get { return _deviceGroup.timeOut; }
            set { _deviceGroup.timeOut = value; }
        }

        [DataMember]
        public string ip
        {
            get { return _deviceGroup.ip; }
            set { _deviceGroup.ip = value; }
        }

        [DataMember]
        public override string url { get { return ip; } }
    }
}