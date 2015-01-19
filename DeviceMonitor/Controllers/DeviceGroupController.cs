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
    public class DeviceGroupController : ApiController
    {
        // GET api/<controller>
        public DeviceGroupViewModel Get()
        {
            //Create(DeviceContext.Instance);
            return DeviceGroupViewModel.GetDeviceGroup().ElementAt(0);
        }


       
        // GET api/<controller>/5
        public DeviceGroupViewModel Get(Guid id)
        {
            return DeviceGroupViewModel.GetDeviceGroup(id);
        }

        // POST api/<controller>
        public void Post([FromBody] DeviceGroupViewModel value)
        {
            if (value == null)
            {
                return;
            }
           value.Save();
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