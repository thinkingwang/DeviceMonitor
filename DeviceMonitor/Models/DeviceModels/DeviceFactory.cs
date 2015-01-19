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
    [Table("DeviceFactory")]
    [Serializable]
    public class DeviceFactory:DeviceBase
    {
        /// <summary>
        /// 节点状态，是树节点还是叶子节点
        /// </summary>
        public override string state
        {
            get { return "closed"; }
            set { base.state = value; }
        }

        /// <summary>
        /// 厂区列表
        /// </summary>
        public virtual ICollection<DeviceGroup> DeviceGroups { get; set; }

        [NotMapped]
        public override NodeType nodeType
        {
            get { return NodeType.Factory; }
        }

        public override DeviceBase Clone()
        {
            var deviceBase = new DeviceFactory
            {
                id = Guid.NewGuid(),
                index = index,
                name = name,
                state = state,
                DeviceGroups = new Collection<DeviceGroup>()
            };
            DeviceGroups.ForEach(m =>
            {
                var element = m.Clone() as DeviceGroup;
                if (element != null)
                {
                    element.parentId = deviceBase.id;
                    element.DeviceFactory = deviceBase;
                }
                deviceBase.DeviceGroups.Add(element);
            });
            return deviceBase;
        }
    }
}