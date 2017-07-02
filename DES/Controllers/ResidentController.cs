using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DES.Models;
using DESCore.Models;
using DESCore.Database;

namespace DES.Controllers
{
    public class ResidentController : Controller
    {
        public ActionResult Index()
        {
            if (Session["SiteModel"] == null) return RedirectToAction("Index", "Dashboard");
            ResidentsViewModel model = new ResidentsViewModel();

            if (Session["ResidentsViewModel"] == null)
            {
                model.SiteId = ((Json)Session["SiteModel"]).Site.Id;
                model.Residents = new ResidentDb().LoadResidents(model.SiteId);
                Session["ResidentsViewModel"] = model;
            }
            else model = (ResidentsViewModel)Session["ResidentsViewModel"];

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ResidentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (Session["SiteModel"] == null) return RedirectToAction("Index", "Dashboard");

            var resident = new Resident
            {
                ResidentName = model.ResidentName,
                SiteId = ((Json)Session["SiteModel"]).Site.Id,
                FlatNumber = model.FlatNumber,
                HomeTel = model.HomeTel,
                MobileTel = model.MobileTel,
                Email = model.Email,
                TagIndex = model.TagIndex,
            };

            ResultModel result = new ResidentDb().CreateResident(resident);
            if(result.Status)
            {
                model = new ResidentViewModel
                {
                    Info = "Created resident successfully.",
                };
                Session["ResidentsViewModel"] = null;
            }
            else
            {
                model.Info = "";
                model.Error = (string)result.Result;
            }
            return View(model);
        }

        public ActionResult Update(int ResidentId)
        {
            if (Session["ResidentsViewModel"] == null) return RedirectToAction("Index");
            ResidentsViewModel residentsViewModel = (ResidentsViewModel)Session["ResidentsViewModel"];

            if (residentsViewModel.Residents == null || residentsViewModel.Residents.Count() == 0)
            {
                return RedirectToAction("Index");
            }

            ResidentModel residentModel = residentsViewModel.Residents.Where(r => r.Resident.Id == ResidentId).FirstOrDefault();
            if (residentModel == null) return RedirectToAction("Index");
            residentsViewModel.ResidentId = ResidentId;

            ResidentViewModel model = new ResidentViewModel
            {
                ResidentName = residentModel.Resident.ResidentName,
                FlatNumber = residentModel.Resident.FlatNumber,
                HomeTel = residentModel.Resident.HomeTel,
                MobileTel = residentModel.Resident.MobileTel,
                Email = residentModel.Resident.Email,
                TagIndex = residentModel.Resident.TagIndex,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(ResidentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (Session["ResidentsViewModel"] == null) return RedirectToAction("Index");
            ResidentsViewModel residentsViewModel = (ResidentsViewModel)Session["ResidentsViewModel"];
            if (residentsViewModel.Residents == null || residentsViewModel.Residents.Count() == 0)
            {
                return RedirectToAction("Index");
            }

            Resident resident = new Resident
            {
                Id = residentsViewModel.ResidentId,
                SiteId = residentsViewModel.SiteId,
                ResidentName = model.ResidentName,
                FlatNumber = model.FlatNumber,
                HomeTel = model.HomeTel,
                MobileTel = model.MobileTel,
                Email = model.Email,
                TagIndex = model.TagIndex,
            };

            ResultModel result = new ResidentDb().UpdateResident(resident);
            if (result.Status)
            {
                Session["ResidentsViewModel"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                model.Info = "";
                model.Error = (string)result.Result;
            }
            return View(model);
        }

        public ActionResult Delete(int ResidentId)
        {
            if (Session["ResidentsViewModel"] == null) return RedirectToAction("Index");
            ResidentsViewModel residentsViewModel = (ResidentsViewModel)Session["ResidentsViewModel"];

            if (residentsViewModel.Residents == null || residentsViewModel.Residents.Count() == 0)
            {
                return RedirectToAction("Index");
            }

            ResidentModel residentModel = residentsViewModel.Residents.Where(r => r.Resident.Id == ResidentId).FirstOrDefault();
            if (residentModel == null) return RedirectToAction("Index");
            residentsViewModel.ResidentId = ResidentId;

            ResidentViewModel model = new ResidentViewModel
            {
                ResidentName = residentModel.Resident.ResidentName,
                FlatNumber = residentModel.Resident.FlatNumber,
                HomeTel = residentModel.Resident.HomeTel,
                MobileTel = residentModel.Resident.MobileTel,
                Email = residentModel.Resident.Email,
                TagIndex = residentModel.Resident.TagIndex,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(ResidentViewModel model)
        {
            if (Session["ResidentsViewModel"] == null) return RedirectToAction("Index");
            ResidentsViewModel residentsViewModel = (ResidentsViewModel)Session["ResidentsViewModel"];
            if (residentsViewModel.Residents == null || residentsViewModel.Residents.Count() == 0)
            {
                return RedirectToAction("Index");
            }

            ResultModel result = new ResidentDb().DeleteResident(residentsViewModel.ResidentId);
            if (result.Status) 
            {
                Session["ResidentsViewModel"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                ResidentModel residentModel = residentsViewModel.Residents.Where(r => r.Resident.Id == residentsViewModel.ResidentId).FirstOrDefault();
                model = new ResidentViewModel
                {
                    ResidentName = residentModel.Resident.ResidentName,
                    FlatNumber = residentModel.Resident.FlatNumber,
                    HomeTel = residentModel.Resident.HomeTel,
                    MobileTel = residentModel.Resident.MobileTel,
                    Email = residentModel.Resident.Email,
                    TagIndex = residentModel.Resident.TagIndex,
                };
                model.Error = (string)result.Result;
            }
            return View(model);
        }
    }
}
