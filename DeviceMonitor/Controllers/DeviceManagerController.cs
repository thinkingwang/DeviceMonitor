using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using DeviceMonitor.Dao;
using DeviceMonitor.Models.DeviceModels;
using DeviceMonitor.ViewModels;
using WebGrease.Css.Extensions;

namespace DeviceMonitor.Controllers
{
    public class DeviceManagerController : ControllerBase
    {
        //
        // GET: /DeviceManager/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DeviceManager/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DeviceManager/Create

        public ActionResult Create()
        {
            return View();
        }
        // GET api/<controller>/5/5
        public JsonResult GetInstance(Guid id,int nodeType)
        {
            switch (nodeType)
            {
                case 0:
                    return Json(DeviceGroupViewModel.NewDeviceGroup(id), JsonRequestBehavior.AllowGet);
                case 1:
                    return Json(DeviceInfoViewModel.NewDeviceGroup(id), JsonRequestBehavior.AllowGet);
                case 2:
                    return Json( DeviceDataViewModel.NewDeviceData(id), JsonRequestBehavior.AllowGet);
                case 3:
                    return Json(DeviceDataFormatViewModel.NewDeviceDataFormat(id), JsonRequestBehavior.AllowGet);
                default:
                    return null;
            }
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetData(Guid id,int nodeType,string sort)
        {
            switch (nodeType)
            {
                case 0:
                    return Json(DeviceFactoryViewModel.GetChildren(id).OrderBy(m=>m.GetType().GetProperty(sort).GetValue(m)), JsonRequestBehavior.AllowGet);
                case 1:
                    return Json(DeviceGroupViewModel.GetChildren(id).OrderBy(m => m.GetType().GetProperty(sort).GetValue(m)), JsonRequestBehavior.AllowGet);
                case 2:
                    return Json(DeviceInfoViewModel.GetChildren(id).OrderBy(m => m.GetType().GetProperty(sort).GetValue(m)), JsonRequestBehavior.AllowGet);
                case 3:
                    return Json(DeviceDataViewModel.GetChildren(id).OrderBy(m => m.GetType().GetProperty(sort).GetValue(m)), JsonRequestBehavior.AllowGet);
                default:
                    return null;
            }
           
        }

        //
        // POST: /DeviceManager/Create

        [System.Web.Mvc.HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET api/<controller>/5/5
        [System.Web.Mvc.HttpPost]
        public JsonResult CopyObject(Guid parentId, Guid id, int nodeType)
        {
            switch (nodeType)
            {
                case 1:
                    return Json(DeviceGroupViewModel.Copy(id,parentId), JsonRequestBehavior.AllowGet);
                case 2:
                    return Json(DeviceInfoViewModel.Copy(id,parentId), JsonRequestBehavior.AllowGet);
                case 3:
                    return Json(DeviceDataViewModel.Copy(id, parentId), JsonRequestBehavior.AllowGet);
                case 4:
                    return Json(DeviceDataFormatViewModel.Copy(id, parentId), JsonRequestBehavior.AllowGet);
                default:
                    return null;
            }
        }

        //
        // GET: /DeviceManager/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DeviceManager/Edit/5

        [System.Web.Mvc.HttpPost]
        public void Edit(Guid id, int nodeType,[FromBody] FormCollection collection)
        {
                if (!ModelState.IsValid)
                {
                    return;
                }
                switch (nodeType)
                {
                    case 0:
                        var factory = DeviceFactoryViewModel.GetDeviceFactory(id);
                        UpdateModel(factory);
                        factory.Save();
                        break;
                    case 1:
                        var group = DeviceGroupViewModel.GetDeviceGroup(id);
                        UpdateModel(group);
                        group.Save();
                        break;
                    case 2:
                        var info = DeviceInfoViewModel.GetDeviceInfo(id);
                        UpdateModel(info);
                        info.Save();
                        break;
                    case 3:
                        var data = DeviceDataViewModel.GetDeviceData(id);
                        UpdateModel(data, new[] { "unit","upper","lower","AlarmAble" ,"address", "lengthOrIndex", "group", "type","id","groupIndex","parentId" ,"index","name"});
                        data.Save();
                        break;
                    case 4:
                        var format = DeviceDataFormatViewModel.GetDeviceDataFormat(id);
                        UpdateModel(format);
                        format.Save();
                        break;
                }
            // TODO: Add update logic here

            
        }

        //
        // GET: /DeviceManager/Delete/5

        public ActionResult Delete(Guid id)
        {
            return View();
        }

        //
        // POST: /DeviceManager/Delete/5

        [System.Web.Mvc.HttpPost]
        public void Delete(Guid id, int nodeType, [FromBody] FormCollection collection)
        {
            // TODO: Add delete logic here
            switch (nodeType)
            {
                case 0:
                    DeviceFactoryViewModel.Delete(id);
                    break;
                case 1:
                    DeviceGroupViewModel.Delete(id);
                    break;
                case 2:
                    DeviceInfoViewModel.Delete(id);
                    break;
                case 3:
                    DeviceDataViewModel.Delete(id);
                    break;
                case 4:
                    DeviceDataFormatViewModel.Delete(id);
                    break;
            }
        }
    }
}
