using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using DeviceMonitor.Dao;
using DeviceMonitor.Models;
using DeviceMonitor.Models.DeviceModels;
using DeviceMonitor.ViewModels;
using WebGrease.Css.Extensions;
using System.Web.Http.Cors;

namespace DeviceMonitor.Controllers
{
    public class DeviceInfoController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<DeviceInfoViewModel> Get()
        {
            return DeviceInfoViewModel.GetDeviceInfo();
        }

        // GET api/<controller>/5
        public IEnumerable<string> Get(Guid id,string group)
        {
            var items = (from r in DeviceInfoViewModel.GetChildren(id).OrderBy(m=>m.index) select r.@group).Distinct();
            return items;
        }
        
        public DeviceInfoViewModel Get(Guid id)
        {
            return new DeviceInfoViewModel(DeviceContext.Instance.DeviceInfos.Find(id));
        }

        // POST api/<controller>
        public void Post(DeviceInfoViewModel value)
        {
            if (value == null)
            {
                return;
            }
            value.Save();
        }

        // PUT api/<controller>/5
        public void Put(Guid id,  DeviceInfo value)
        {
            if (value == null)
            {
                return;
            }
            var element = DeviceContext.Instance.DeviceInfos.Find(id);
            value.DeviceDatas = element.DeviceDatas;
            value.DeviceGroup = element.DeviceGroup;
            DeviceContext.Instance.DeviceInfos.AddOrUpdate(value);
            DeviceContext.Instance.SaveChanges();
        }

        // DELETE api/<controller>/5
        // DELETE api/<controller>/5
        public void Delete(Guid id)
        {
            DeviceContext.Instance.DeviceInfos.Remove(DeviceContext.Instance.DeviceInfos.Find(id));
            DeviceContext.Instance.SaveChanges();
        }

    }
}