using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Web.ModelBinding;
using DeviceMonitor.Dao;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace DeviceMonitor.Models.DeviceModels
{
    [Table("DeviceGroup")]
    [Serializable]
    public class DeviceGroup:DeviceBase
    {
        /// <summary>
        /// 节点状态，是树节点还是叶子节点
        /// </summary>
        public override string state
        {
            get
            {
                return  "closed";
            }
            set { base.state = value; }
        }
        /// <summary>
        /// 所属设备组
        /// </summary>
        [ForeignKey("parentId")]
        public virtual DeviceFactory DeviceFactory { get; set; }

        public Guid parentId { get; set; }

        /// <summary>
        /// 数据刷新周期
        /// </summary>
        public int refreshTime { get; set; }

        /// <summary>
        /// 接收UDP数据的IP地址
        /// </summary>
        public string ip { get; set; }

        /// <summary>
        /// 设备列表
        /// </summary>
        public virtual ICollection<DeviceInfo> DeviceInfos { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int timeOut{ get; set; }

        [NotMapped]
        public override NodeType nodeType
        {
            get { return NodeType.Group; }
        }

        public override DeviceBase Clone()
        {
            var deviceBase = new DeviceGroup
            {
                id = Guid.NewGuid(),
                index = index,
                ip = ip,
                name = name,
                state = state,
                timeOut = timeOut,
                refreshTime = refreshTime,
                DeviceInfos = new Collection<DeviceInfo>()
            };
            DeviceInfos.ForEach(m =>
            {
                var element = m.Clone() as DeviceInfo;
                if (element != null)
                {
                    element.parentId = deviceBase.id;
                    element.DeviceGroup = deviceBase;
                }
                deviceBase.DeviceInfos.Add(element);
            });
            return deviceBase;
        }
    }
}