using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Runtime.Serialization;
using System.Web.Mvc;
using DeviceMonitor.Dao;
using DeviceMonitor.Models.DeviceModels;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace DeviceMonitor.ViewModels
{
    [DataContract]
    public class DeviceDataFormatViewModel:DeviceBaseViewModel
    {
        private readonly DeviceDataFormat _deviceFormat;

        private DeviceDataFormatViewModel(DeviceDataFormat format)
            : base(format)
        {
            _deviceFormat = format;
        }

        public static DeviceDataFormatViewModel GetDeviceDataFormat(Guid id)
        {
            var format = DeviceContext.Instance.DeviceDataFormats.Find(id)??new DeviceDataFormat();
            return new DeviceDataFormatViewModel(format);
        }
        public static ICollection<DeviceDataFormatViewModel> GetDeviceDataFormat()
        {
            var infos = DeviceContext.Instance.DeviceDataFormats;
            var viewModels = new List<DeviceDataFormatViewModel>();
            infos.ForEach(m => viewModels.Add(GetDeviceDataFormat(m.id)));
            return viewModels;
        }
        public static DeviceDataFormatViewModel Copy(Guid dataId, Guid pId)
        {
            var item = GetDeviceDataFormat(dataId);
            return item.Copy(pId);
        }

        private DeviceDataFormatViewModel Copy(Guid pId)
        {
            var itemCopy = _deviceFormat.Clone() as DeviceDataFormat;
            if (itemCopy != null)
            {
                itemCopy.parentId = pId;
                itemCopy.DeviceData = DeviceContext.Instance.DeviceDatas.Find(pId);
            }
            DeviceContext.Instance.DeviceDataFormats.AddOrUpdate(itemCopy);
            DeviceContext.Instance.SaveChanges();
            return new DeviceDataFormatViewModel(itemCopy);
        }

        public static DeviceDataFormatViewModel NewDeviceDataFormat(Guid id)
        {
            var data = DeviceContext.Instance.DeviceDatas.Find(id);
            var format = new DeviceDataFormat()
            {
                id = Guid.NewGuid(),
                nodeType = NodeType.Format,
                parentId = id,
                state = "open",
                DeviceData = data,
                index = data.DeviceDataFormats.Count,
                key = data.DeviceDataFormats.Count,
                value = "0"
            };
            return new DeviceDataFormatViewModel(format);
        }
        public static void Delete(Guid id)
        {
            var info = GetDeviceDataFormat(id);
            if (info != null)
            {
                info.Delete();
            }
        }
        public void Save()
        {
            DeviceContext.Instance.DeviceDataFormats.AddOrUpdate(_deviceFormat);
            DeviceContext.Instance.SaveChanges();
        }

        public void Delete()
        {
            DeviceContext.Instance.DeviceDataFormats.AddOrUpdate(_deviceFormat);
            DeviceContext.Instance.SaveChanges();
        }
       
        [DataMember]
        public int key
        {
            get { return _deviceFormat.key; }
            set { _deviceFormat.key = value; }
        }
        [DataMember]
        public string value
        {
            get { return _deviceFormat.value; }
            set { _deviceFormat.value = value; }
        }
        [DataMember]
        public Guid parentId
        {
            get { return _deviceFormat.parentId; }
            set { _deviceFormat.parentId = value; }
        }
        [DataMember]
        public string changeFormat
        {
            get {
                switch (_deviceFormat.changeFormat)
                {
                    case Models.DeviceModels.ChangeFormat.ReplaceWithStr:
                        return "Ìæ»»";
                    case Models.DeviceModels.ChangeFormat.PlusWith:
                        return "×·¼Ó";
                    case Models.DeviceModels.ChangeFormat.MultiWithStr:
                        return "Ïà³Ë";
                    case Models.DeviceModels.ChangeFormat.DividWith:
                        return "Ïà³ý";
                    default:
                        return "Ìæ»»";
                }
            }
            set
            {
                switch (value)
                {
                    case "Ìæ»»":
                        _deviceFormat.changeFormat = Models.DeviceModels.ChangeFormat.ReplaceWithStr;
                        break;
                    case "×·¼Ó":
                        _deviceFormat.changeFormat = Models.DeviceModels.ChangeFormat.PlusWith;
                        break;
                    case "Ïà³Ë":
                        _deviceFormat.changeFormat = Models.DeviceModels.ChangeFormat.MultiWithStr;
                        break;
                    case "Ïà³ý":
                        _deviceFormat.changeFormat = Models.DeviceModels.ChangeFormat.DividWith;
                        break;
                    default:
                        _deviceFormat.changeFormat = Models.DeviceModels.ChangeFormat.ReplaceWithStr;
                        break;
                }
            }
        }
        /// <summary>
        /// url
        /// </summary>
        [DataMember]
        public override string url
        {
            get { return _deviceFormat.DeviceData.DeviceInfo.DeviceGroup.ip; }
        }
    }
}