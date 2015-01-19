using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using DeviceMonitor.Dao;
using DeviceMonitor.Models.DeviceModels;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace DeviceMonitor.ViewModels
{
    [DataContract]
    public class DeviceFactoryViewModel:DeviceBaseViewModel
    {
        private readonly DeviceFactory _deviceBase;

        public DeviceFactoryViewModel(DeviceFactory factory):base(factory)
        {
            _deviceBase = factory;
        }

        /// <summary>
        /// 报警状态
        /// </summary>
        [DataMember]
        public override bool Alarmed
        {
            get { return _deviceBase.DeviceGroups.Any(m => DeviceGroupViewModel.GetDeviceGroup(m.id).Alarmed); }
        }
        public static DeviceFactoryViewModel GetDeviceFactory(Guid id)
        {
            var factory = DeviceContext.Instance.DeviceFactories.Find(id)??new DeviceFactory();
            return new DeviceFactoryViewModel(factory);
        }

        public static List<DeviceGroupViewModel> GetChildren(Guid id)
        {
            var info = DeviceContext.Instance.DeviceFactories.Find(id);
            var viewModels = new List<DeviceGroupViewModel>();
            info.DeviceGroups.ForEach(m => viewModels.Add(DeviceGroupViewModel.GetDeviceGroup(m.id)));
            return viewModels;
        }

        public static ICollection<DeviceFactoryViewModel> GetDeviceFactory()
        {
            var infos = DeviceContext.Instance.DeviceFactories;
            var viewModels = new List<DeviceFactoryViewModel>();
            infos.ForEach(m => viewModels.Add(GetDeviceFactory(m.id)));
            return viewModels;
        }

        public static void Delete(Guid id)
        {
            var info = GetDeviceFactory(id);
            if (info != null)
            {
                info.Delete();
            }
        }

        public void Save()
        {
            DeviceContext.Instance.DeviceFactories.AddOrUpdate(_deviceBase);
            DeviceContext.Instance.SaveChanges();
        }
        public void Delete()
        {
            DeviceContext.Instance.DeviceFactories.Remove(_deviceBase);
            DeviceContext.Instance.SaveChanges();
        }
    }
}