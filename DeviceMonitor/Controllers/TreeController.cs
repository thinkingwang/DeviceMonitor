using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DeviceMonitor.Dao;
using DeviceMonitor.ViewModels;

namespace DeviceMonitor.Controllers
{
    public class TreeController : ApiController
    {
        private readonly DeviceContext _context = new DeviceContext();
        //定义一个私有成员变量，用于Lock  
        private static object _lockobj = new object();
        // GET api/<controller>
        [Queryable]
        public IEnumerable<DeviceInfoViewModel> Get()
        {
            try
            {
                var groups = DeviceInfoViewModel.GetDeviceInfo();
                return groups;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        [Queryable]
        public IEnumerable<DeviceFactoryViewModel> Get(int nodeType)
        {
            try
            {
                var groups = DeviceFactoryViewModel.GetDeviceFactory();
                return groups;
            }
            catch (Exception)
            {

                throw;
            }
        }

        // GET api/<controller>/5
        [Queryable]
        public IEnumerable<DeviceBaseViewModel> Get(Guid id,int nodeType)
        {
            switch (nodeType)
            {
                case 0:
                    return DeviceFactoryViewModel.GetChildren(id).OrderBy(m => m.index).AsQueryable();
                case 1:
                    return DeviceGroupViewModel.GetChildren(id).AsQueryable();
                case 2:
                    return DeviceInfoViewModel.GetChildren(id).OrderBy(m => m.index).AsQueryable();
                case 3:
                    return DeviceDataViewModel.GetChildren(id).OrderBy(m => m.index).AsQueryable();
                default :
                    return null;
            }
        }

        // POST api/<controller>
        public void Post([FromBody] DeviceBaseViewModel value)
        {
            
        }

        // PUT api/<controller>/5
        public void Put(Guid id, [FromBody] DeviceBaseViewModel value)
        {
         
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}