using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using DeviceMonitor.Dao;
using DeviceMonitor.Models.DeviceModels;
using WebGrease.Css.Extensions;

namespace DeviceMonitor.ViewModels
{
    [DataContract]
    public class DataViewModel
    {
        private readonly DeviceData _deviceData;

        private DataViewModel(DeviceData data)
        {
            _deviceData = data;
        }
        public static ICollection<DataViewModel> GetList(Guid pId,string group)
        {
            var info = DeviceContext.Instance.DeviceInfos.Find(pId);
            if (info == null)
            {
                return null;
            }
            var viewModels = new List<DataViewModel>();
            if (group == "null")
            {
                info.DeviceDatas.OrderBy(m => m.index).ForEach(m =>
                {
                    if (m.group == null)
                    {
                        viewModels.Add(new DataViewModel(m));
                    }
                });
            }
            else
            {
                info.DeviceDatas.OrderBy(m => m.index).ForEach(m =>
                {
                    if (m.group == group)
                    {
                        viewModels.Add(new DataViewModel(m));
                    }
                });
            }
            return viewModels;
        }

        public static DataViewModel GetDeviceData(DeviceData data)
        {
            return new DataViewModel(data);
        }
        public static DataViewModel GetDeviceData(Guid id)
        {
            return new DataViewModel(DeviceContext.Instance.DeviceDatas.Find(id));
        }

        /// <summary>
        /// 组内索引
        /// </summary>
        [DataMember]
        public int groupIndex
        {
            get { return _deviceData.groupIndex; }
        }

        /// <summary>
        /// 父节点Id
        /// </summary>
        [DataMember]
        public Guid parentId
        {
            get { return _deviceData.parentId; }
        }
        /// <summary>
        /// 报警状态
        /// </summary>
        [DataMember]
        public bool Alarmed
        {
            get
            {
                if (!_deviceData.AlarmAble)
                {
                    return false;
                }
                if (value.Equals("异常"))
                {
                    return true;
                }
                if (_deviceData.value < _deviceData.lower || _deviceData.value > _deviceData.upper)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 设备点数据
        /// </summary>
        [DataMember]
        public string value
        {
            get
            {
                var value1 = _deviceData.value.ToString();
                if (_deviceData.DeviceDataFormats.Count <= 0)
                {
                    return value1;
                }
                var temp = _deviceData.value;
                var format = _deviceData.DeviceDataFormats.Count == 1
                    ? _deviceData.DeviceDataFormats.ElementAt(0)
                    : _deviceData.DeviceDataFormats.FirstOrDefault(m => m.key == temp);
                if (format != null)
                {
                    switch (format.changeFormat)
                    {
                        case ChangeFormat.ReplaceWithStr:
                            value1 = format.value;
                            break;
                        case ChangeFormat.MultiWithStr:
                            float value2, value3;
                            var ret1 = float.TryParse(value1, out value2);
                            var ret2 = float.TryParse(format.value, out value3);
                            if (ret1 && ret2)
                            {
                                value1 = (value2 * value3).ToString(CultureInfo.InvariantCulture);
                            }
                            break;
                        case ChangeFormat.PlusWith:
                            value1 += format.value;
                            break;
                    }
                }

                return value1;
            }
        }

        [DataMember]
        public string unit
        {
            get { return _deviceData.unit; }
        }

        [DataMember]
        public string name
        {
            get { return _deviceData.name; }
        }
    }
}