using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DeviceMonitor.Dao;
using Newtonsoft.Json;

namespace DeviceMonitor.Models.DeviceModels
{
    public enum NodeType
    {
        Factory=0,
        Group=1,
        Info=2,
        Data=3,
        Format=4
    }
    [Table("DeviceGroup")]
    [Serializable]
    public  class DeviceBase
    {
        /// <summary>
        /// 设备组ID号
        /// </summary>
        [Key]
        public Guid id { get; set; }

        public int index { get; set; }

        /// <summary>
        /// 设备组名称
        /// </summary>
        public string name { get; set; }

        public virtual string state { get; set; }

       
        /// <summary>
        /// 节点类型
        /// </summary>
        [NotMapped]
        public virtual NodeType nodeType { get; set; }

        public virtual DeviceBase Clone()
        {
            throw new NotImplementedException();
        }
    }
}