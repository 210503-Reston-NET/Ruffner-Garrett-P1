using System.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service;
using WebUI.Models;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using StoreModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace WebUI
{
    public class LocationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IAuthorizationService _AuthorizationService;
   
        private IServices _service;
        public LocationController(IServices service, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuthorizationService AuthorizationService)
        {
          this._service = service;
          this._userManager = userManager;
          this._signInManager = signInManager;
          this._AuthorizationService = AuthorizationService;
        }
        
        [AllowAnonymous]
        public ActionResult Index()
        {
          return View(_service.GetAllLocations().AsEnumerable());
        }

        [Authorize]
        public ActionResult Create()
        {
          return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location c)
        {
          try
          {
            if(ModelState.IsValid)
            {
              Guid OwnerId = Guid.Parse(_userManager.GetUserId(User));
              c = _service.AddLocation(c.LocationName, c.Address, OwnerId);
              ApplicationUser u = await _userManager.GetUserAsync(User);
              

              await _userManager.AddClaimAsync(u,new Claim("Owner", c.LocationID.ToString()));
              //Need to relog user to refresh admin option
              await _signInManager.RefreshSignInAsync(u);

              return RedirectToAction(nameof(Index));
            }
            Log.Error("Model state invalid For LocationCreation ");
            return View();
          }catch{
            return View();
          }
          
        }
        [AllowAnonymous]
        public ActionResult Shop(int Id)
        {
          Log.Verbose("ID from action: {0}", Id);
          List<InventoryItem> items = _service.getInventory(Id);
          List<ItemVM> newItems = new List<ItemVM>();
          items.ForEach(item => newItems.Add(new ItemVM(item, Id)));
          return View(newItems);
        }

        [Authorize] 
        public async Task<ActionResult> Admin(int Id)
        {
          var item = _service.GetAllLocations().Select(location => location).Where(location => location.LocationID == Id).FirstOrDefault();
          if ((await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))).Succeeded)
          {
            List<InventoryItem> items = item.InventoryItems;
            List<ItemVM> newItems = new List<ItemVM>();
            items.ForEach(item => newItems.Add(new ItemVM(item)));
            ViewBag.LocationID = item.LocationID;
            return View(newItems);
          }  

          return RedirectToAction(nameof(Index));
          //if(await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))){
        }


        [Authorize]
        [Route("Location/AddProduct/{LocationID}")]
        public async Task<ActionResult> AddProduct(int LocationID)
        {

          Log.Verbose("LocationID: {0}", LocationID);
          var item = _service.GetAllLocations().Select(location => location).Where(location => location.LocationID == LocationID).FirstOrDefault();
          if ((await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))).Succeeded)
          {
            Location location = item;
            List<int> items= new List<int>();
            location.InventoryItems.ForEach(i => items.Add(i.ProductID));
            ViewBag.LocationID=LocationID;
            //get all products not in inventory
            return View(_service.GetAllProducts().Select(product => new ProductVM(product)).Where(product => !items.Contains(product.ProductID) ));
          }  

          return RedirectToAction(nameof(Index));
          //if(await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))){
        }


        
        // [Authorize]
        // 
        //  public async Task<ActionResult> AddProductToInventory(int LocationID)
        // {

        //   Log.Verbose("LocationID: {0}", LocationID);
        //   var item = _service.GetAllLocations().Select(location => location).Where(location => location.LocationID == LocationID).FirstOrDefault();
        //   if ((await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))).Succeeded)
        //   {
        //     return View(LocationID);
        //   }

        //   return RedirectToAction(nameof(Index));
        //   //if(await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))){
        // }

        [Authorize]
        [Route("Location/AddProductToInventory/{LocationID}/{ProductID}")]
         public async Task<ActionResult> AddProductToInventory(int LocationID, int ProductID)
        {
          var item = _service.GetAllLocations().Select(location => location).Where(location => location.LocationID == LocationID).FirstOrDefault();
          if ((await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))).Succeeded)
          {
            _service.AddProductToInventory(LocationID,ProductID, 0);
            return RedirectToAction("Admin", new{id=LocationID});
          }

          return RedirectToAction(nameof(Index));
          //if(await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))){
        }
        [Authorize] 
        public ActionResult CreateProduct(ProductVM p){
          try
          {
            if(ModelState.IsValid)
            {
              _service.AddProduct(p.Name, p.Price);
              

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