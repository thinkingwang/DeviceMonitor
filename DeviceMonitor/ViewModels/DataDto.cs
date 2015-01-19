using System;
using System.Runtime.Serialization;
using DeviceMonitor.Models.DeviceModels;

namespace DeviceMonitor.ViewModels
{
    [DataContract]
    public class DataDto
    {
        /// <summary>
        /// 报警状态
        /// </summary>
        [DataMember]
        public virtual bool Alarmed { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        [DataMember]
        public int index { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [DataMember]
        public string value { get; set; }
        /// <summary>
        /// 设备组名称
        /// </summary>
        [DataMember]
        public string name { get; set; }

    }
}