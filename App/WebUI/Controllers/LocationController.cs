using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service;
using WebUI.Models;
using Serilog;

namespace WebUI
{
    public class LocationController : Controller
    {
        private IServices _service;
        public LocationController(IServices service)
        {
          this._service = service;
        }

        public ActionResult Index()
        {
          return View(_service.GetAllLocations().Select(
            Location => new LocationVM(Location))
            .ToList()
            );
        }

        public ActionResult Create()
        {
          return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LocationVM c)
        {
          try
          {
            if(ModelState.IsValid)
            {
              _service.AddLocation(c.LocationName, c.Address);
              return RedirectToAction(nameof(Index));
            }
            Log.Error("Model state invalid For LocationCreation ");
            return View();
          }catch{
            return View();
          }
          
        }
    }
}