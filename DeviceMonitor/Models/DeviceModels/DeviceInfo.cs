using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DeviceMonitor.Dao;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace DeviceMonitor.Models.DeviceModels
{
    [Table("DeviceInfo")]
    [Serializable]
    public class DeviceInfo : DeviceBase
    {
        
        /// <summary>
        /// 所属设备组
        /// </summary>
        [ForeignKey("parentId")]
        public virtual DeviceGroup DeviceGroup { get; set; }

        public Guid parentId { get; set; }

        /// <summary>
        /// 设备IP地址
        /// </summary>
        public string ip { get; set; }

        /// <summary>
        /// 设备IP地址
        /// </summary>
        public int port { get; set; }

        /// <summary>
        /// 数据头长度
        /// </summary>
        public int headerLength { get; set; }

        /// <summary>
        /// 数据包长度
        /// </summary>
        public int dataLength { get; set; }

        /// <summary>
        /// 命令字
        /// </summary>
        public byte commandByte { get; set; }

        public override string state
        {
            get
            {
                return "closed";
            }
            set { base.state = value; }
        }

        public virtual ICollection<DeviceData> DeviceDatas { get; set; }

        [NotMapped]
        public override NodeType nodeType
        {
            get { return NodeType.Info; }
        }

        public override DeviceBase Clone()
        {
            var deviceBase = new DeviceInfo
            {
                id = Guid.NewGuid(),
                commandByte = commandByte,
                headerLength = headerLength,
                index = index,
                dataLength = dataLength,
                ip = ip,
                name = name,
                state = state,
                port = port,
                DeviceDatas = new Collection<DeviceData>()
            };
            DeviceDatas.ForEach(m =>
            {
                var element = m.Clone() as DeviceData;
                if (element != null)
                {
                    element.parentId = deviceBase.id;
                    element.DeviceInfo = deviceBase;
                }
                deviceBase.DeviceDatas.Add(element);
            });
            return deviceBase;
        }
    }
}