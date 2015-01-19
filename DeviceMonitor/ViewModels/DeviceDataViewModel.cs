using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Runtime.Serialization;
using DeviceMonitor.Dao;
using DeviceMonitor.Models.DeviceModels;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace DeviceMonitor.ViewModels
{
    [DataContract]
    public class DeviceDataViewModel:DeviceBaseViewModel
    {
        private readonly DeviceData _deviceData;

        private DeviceDataViewModel(DeviceData data):base(data)
        {
            _deviceData = data;
        }
        public static ICollection<DeviceDataViewModel> GetDeviceData()
        {
            var infos = DeviceContext.Instance.DeviceDatas;
            var viewModels = new List<DeviceDataViewModel>();
            infos.ForEach(m => viewModels.Add(GetDeviceData(m.id)));
            return viewModels;
        }


        public static List<DeviceDataFormatViewModel> GetChildren(Guid id)
        {
            var info = DeviceContext.Instance.DeviceDatas.Find(id);
            var viewModels = new List<DeviceDataFormatViewModel>();
            info.DeviceDataFormats.ForEach(m => viewModels.Add(DeviceDataFormatViewModel.GetDeviceDataFormat(m.id)));
            return viewModels;
        }

        public static DeviceDataViewModel GetDeviceData(Guid id)
        {
            var data = DeviceContext.Instance.DeviceDatas.Find(id)??new DeviceData();
            return new DeviceDataViewModel(data);
        }
        public static void Delete(Guid id)
        {
            var info = GetDeviceData(id);
            if (info != null)
            {
                info.Delete();
            }
        }
        public void Save()
        {
            DeviceContext.Instance.DeviceDatas.AddOrUpdate(_deviceData);
            DeviceContext.Instance.SaveChanges();
        }

        public void Delete()
        {
            DeviceContext.Instance.DeviceDatas.AddOrUpdate(_deviceData);
            DeviceContext.Instance.SaveChanges();
        }

        public static DeviceDataViewModel Copy(Guid dataId, Guid pId)
        {
            var item = GetDeviceData(dataId);
            if (item == null)
            {
                return null;
            }
            return item.Copy(pId);
        }

        private DeviceDataViewModel Copy(Guid pId)
        {
            var itemCopy = _deviceData.Clone() as DeviceData;
            if (itemCopy != null)
            {
                itemCopy.parentId = pId;
                itemCopy.DeviceInfo = DeviceContext.Instance.DeviceInfos.Find(pId);
            }
            DeviceContext.Instance.DeviceDatas.AddOrUpdate(itemCopy);
            DeviceContext.Instance.SaveChanges();
            return new DeviceDataViewModel(itemCopy);
        }

        public static DeviceDataViewModel NewDeviceData(Guid id)
        {
            var data = DeviceContext.Instance.DeviceInfos.Find(id);
            var format = new DeviceData()
            {
                id = Guid.NewGuid(),
                nodeType = NodeType.Format,
                parentId = id,
                state = "closed",
                DeviceInfo = data,
                DeviceDataFormats = new Collection<DeviceDataFormat>(),
                index = data.DeviceDatas.Count,
                lengthOrIndex = 1,
                type = DataType.DeviceReal
            };
            return new DeviceDataViewModel(format);
        }
        /// <summary>
        /// 设备点名
        /// </summary>
        [DataMember]
        public string group
        {
            get { return _deviceData.group; }
        }
      

        /// <summary>
        /// 启用报警
        /// </summary>
        [DataMember]
        public bool AlarmAble
        {
            get { return _deviceData.AlarmAble; }
            set { _deviceData.AlarmAble = value; }
        }

        /// <summary>
        /// 报警上限
        /// </summary>
        [DataMember]
        public int? upper
        {
            get { return _deviceData.upper; }
            set { _deviceData.upper = value; }
        }

        /// <summary>
        /// 报警下限
        /// </summary>
        [DataMember]
        public int? lower
        {
            get { return _deviceData.lower; }
            set { _deviceData.lower = value; }
        }

        /// <summary>
        /// 组内索引
        /// </summary>
        public int groupIndex
        {
            get { return _deviceData.groupIndex; }
            set { _deviceData.groupIndex = value; }
        }

        public int lengthOrIndex
        {
            get { return _deviceData.lengthOrIndex; }
            set { _deviceData.lengthOrIndex = value; }
        }

        [DataMember]
        public Guid parentId
        {
            get { return _deviceData.parentId; }
            set { _deviceData.parentId = value; }
        }

        public override string state
        {
            get { return "open"; }
        }
        
        [DataMember]
        public int address
        {
            get { return _deviceData.address; }
            set { _deviceData.address = value; }
        }
        /// <summary>
        /// 设备点名
        /// </summary>
        [DataMember]
        public DataType type
        {
            get { return _deviceData.type; }
            set { _deviceData.type = value; }
        }


        [DataMember]
        public string unit
        {
            get { return _deviceData.unit; }
            set { _deviceData.unit = value; }
        }

        /// <summary>
        /// url
        /// </summary>
        [DataMember]
        public override string url
        {
            get { return _deviceData.DeviceInfo.DeviceGroup.ip; }
        }
    }
}