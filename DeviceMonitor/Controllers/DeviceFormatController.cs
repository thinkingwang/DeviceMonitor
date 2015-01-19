using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using DeviceMonitor.Dao;
using DeviceMonitor.Models.DeviceModels;
using DeviceMonitor.ViewModels;
using WebGrease.Css.Extensions;
using System.Web.Http.Cors;

namespace DeviceMonitor.Controllers
{
    public class DeviceFormatController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<DeviceDataFormatViewModel> Get()
        {
            return DeviceDataFormatViewModel.GetDeviceDataFormat();
        }

        // GET api/<controller>/5
        public DeviceDataFormatViewModel Get(Guid id)
        {
            return DeviceDataFormatViewModel.GetDeviceDataFormat(id);
        }
    

        // POST api/<controller>
        public void Post(DeviceDataFormatViewModel value)
        {
            if (value == null)
            {
                return;
            }
            value.Save();
        }

        // PUT api/<controller>/5
        public void Put(Guid id, [FromBody] DeviceDataFormat value)
        {
            if (value == null)
            {
                return;
            }
            var element = DeviceContext.Instance.DeviceDataFormats.Find(id);
            value.DeviceData = element.DeviceData;
            DeviceContext.Instance.DeviceDataFormats.AddOrUpdate(value);
            DeviceContext.Instance.SaveChanges();
        }

        // DELETE api/<controller>/5
        public void Delete(Guid id)
        {
            var item = DeviceContext.Instance.DeviceDataFormats.Find(id);
            DeviceContext.Instance.DeviceDataFormats.Remove(item);
            DeviceContext.Instance.SaveChanges();
        }
    }
}