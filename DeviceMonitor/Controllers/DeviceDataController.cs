using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Web.Http;
using DeviceMonitor.Dao;
using DeviceMonitor.HUB;
using DeviceMonitor.Models.DeviceModels;
using DeviceMonitor.ViewModels;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;
using System.Web.Http.Cors;

namespace DeviceMonitor.Controllers
{
    public class DeviceDataController : ApiController
    {
        // GET api/<controller>
        public DeviceDataViewModel Get()
        {
            return DeviceDataViewModel.GetDeviceData().ElementAt(0);
        }
       
        // GET api/<controller>/5
        public DeviceDataViewModel Get(Guid id)
        {
            return DeviceDataViewModel.GetDeviceData(id);
        }
        // GET api/<controller>/5
        public IEnumerable<DataViewModel> Get(Guid id, string group)
        {
            return DataViewModel.GetList(id,group);
        }

        // POST api/<controller>
        public void Post([FromBody] DataDto value)
        {
            if (value == null)
            {
                return;
            }
        }

        public string Options()
        {

            return null; // HTTP 200 response with empty body

        } 
        // PUT api/<controller>/5
        public HttpResponseMessage Put(Guid id, [FromBody]DataDto dto)
        {
            GlobalHost.ConnectionManager.GetHubContext<ChatHub>().Clients.All.broadcastMessage(id, dto.name,
                            dto.index, dto.value, dto.Alarmed);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        // DELETE api/<controller>/5
        public void Delete(Guid id)
        {
           DeviceDataViewModel.Delete(id);
        }

    }
}