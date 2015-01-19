using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using DeviceMonitor.Dao;
using DeviceMonitor.Models;
using DeviceMonitor.Models.DeviceModels;
using DeviceMonitor.ViewModels;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;
using System.Web.Http.Cors;

namespace DeviceMonitor.Controllers
{
    public class DeviceFactoryController : ApiController
    {
        // GET api/<controller>
        public DeviceFactoryViewModel Get()
        {
            //Create(DeviceContext.Instance);
            return DeviceFactoryViewModel.GetDeviceFactory().ElementAt(0);
        }


       
        // GET api/<controller>/5
        public DeviceBaseViewModel Get(Guid id)
        {
            return new DeviceBaseViewModel(DeviceContext.Instance.DeviceGroups.Find(id));
        }

        // POST api/<controller>
        public void Post([FromBody] DeviceGroup value)
        {
            if (value == null)
            {
                return;
            }
            DeviceContext.Instance.DeviceGroups.AddOrUpdate(value);
            DeviceContext.Instance.SaveChanges();
        }

        // PUT api/<controller>/5
        public void Put(Guid id, [FromBody] DeviceGroup value)
        {
            if (value == null)
            {
                return;
            }
            var element = DeviceContext.Instance.DeviceGroups.Find(id);
            value.DeviceInfos = element.DeviceInfos;
            DeviceContext.Instance.DeviceGroups.AddOrUpdate(value);
            DeviceContext.Instance.SaveChanges();
        }

        // DELETE api/<controller>/5
        // DELETE api/<controller>/5
        public void Delete(Guid id)
        {
            DeviceContext.Instance.DeviceGroups.Remove(DeviceContext.Instance.DeviceGroups.Find(id));
            DeviceContext.Instance.SaveChanges();
        }

    }
}