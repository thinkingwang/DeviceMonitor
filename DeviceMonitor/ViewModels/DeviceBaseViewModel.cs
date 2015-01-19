using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using DeviceMonitor.Models.DeviceModels;
using Newtonsoft.Json;

namespace DeviceMonitor.ViewModels
{
    [DataContract]
    public class DeviceBaseViewModel
    {
        private readonly DeviceBase _deviceBase;

        public DeviceBaseViewModel(DeviceBase @base)
        {
            _deviceBase = @base;
        }
        /// <summary>
        /// 报警状态
        /// </summary>
        [DataMember]
        public virtual bool Alarmed { get; set; }


        /// <summary>
        /// 报警状态
        /// </summary>
        [DataMember]
        public virtual string url { get; set; }

        /// <summary>
        /// 设备组ID号
        /// </summary>
        [DataMember]
        public Guid id { get { return _deviceBase.id; } set { _deviceBase.id = value; }}

        /// <summary>
        /// 序列号
        /// </summary>
        [DataMember]
        public int index { get { return _deviceBase.index; } set { _deviceBase.index = value; } }

        /// <summary>
        /// 设备组名称
        /// </summary>
        [DataMember]
        public string name { get { return _deviceBase.name; }set { _deviceBase.name = value; } }

        [DataMember]
        public virtual string state
        {
            get { return _deviceBase.state; }
            set { _deviceBase.state = value; }
        }


        [DataMember]
        public NodeType nodeType
        {
            get { return _deviceBase.nodeType; }
            set { _deviceBase.nodeType = value; }
        }

    }
}