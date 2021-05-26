using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using StoreModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;


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
        public ActionResult Shop(int? Id)
        {
          try{
            if(Id == null) return RedirectToAction(nameof(Index));
           
            Log.Verbose("ID from action: {0}", Id);
            List<InventoryItem> items = _service.getInventory((int) Id);
            //List<ItemVM> newItems = new List<ItemVM>();
            //items.ForEach(item => newItems.Add(new ItemVM(item)));
            return View(items);
          }catch(Exception){
            return View();
          }
        }

        [Authorize] 
        public async Task<ActionResult> Admin(int Id)
        {
          var item = _service.GetAllLocations().Where(location => location.LocationID == Id).FirstOrDefault();
          if ((await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))).Succeeded)
          {
            ViewBag.LocationID = item.LocationID;
            return View(_service.getInventory(item.LocationID));
          }  

          return RedirectToAction(nameof(Index));
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
            return View(_service.GetAllProducts().Select(p => p).Where(product => !items.Contains(product.ProductID) ));
          }  

          return RedirectToAction(nameof(Index));
         
        }


    
        //Add the specified product to inventory
        [Authorize]
        [Route("Location/AddProductToInventory/{LocationID}/{ProductID}")]
         public async Task<ActionResult> AddProductToInventory(int LocationID, int ProductID)
        {
          try{
            var item = _service.GetAllLocations().Select(location => location).Where(location => location.LocationID == LocationID).FirstOrDefault();
            if ((await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))).Succeeded)
            {
              _service.AddProductToInventory(LocationID,ProductID, 0);
              return RedirectToAction("Admin", new{id=LocationID});
            }
          }catch(Exception ex){
            Log.Debug("Adding Product to invetory failed,{0}\n{1}", ex.Message, ex.StackTrace);
          }

          return RedirectToAction(nameof(Index));
        }

        // Getting Product details before adding to db
        [Authorize]
        [Authorize(Policy = "Owner")]
        public ActionResult CreateProduct(Product p){
          try
          {
            if(ModelState.IsValid)
            {           
                _service.AddProduct(p.Name, p.Price); 
                return RedirectToAction(nameof(Index));
            }
            Log.Error("Model state invalid For Location Creation ");
            return View();
          }catch{
            return View();
          }

        }

        [Authorize]
        [HttpPost]
        [Route("Location/UpdateQuantity")]
        public async void UpdateQuantity(InventoryItem i){
          
          Log.Verbose("Updating Item Location: {0}, Product:{1}, Quantity: {2}", i.LocationID, i.ProductID, i.Quantity);

          int LocationID = i.LocationID;
          try{
            var item = _service.GetAllLocations().Select(location => location).Where(location => location.LocationID == LocationID).FirstOrDefault();
            if ((await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))).Succeeded)
            {
              
              _service.updateItemInStock(i);
  
              Response.Redirect($"Admin/{LocationID}");
            }else{
              Response.Redirect("/");
            }
          }catch(Exception ex){
            Log.Warning("Fialed to update quantity {0}", ex.Message);
            Response.Redirect("/");
          }          
        }
        [Authorize]
        [HttpPost]
        [Route("Location/Shop/Order")]
        public ActionResult Order(IFormCollection form){
          string id = form["LocationID"];
          int LocationID;
          if(!int.TryParse(id, out LocationID)){
            Log.Verbose("Bad LocationID from order form");
            return RedirectToAction(nameof(Index));
          }
          List<InventoryItem> possibleitems = _service.getInventory(LocationID);
          //Create order
          // Guid OwnerId = Guid.Parse(_userManager.GetUserId(User));
          // Order = new Order(OwnerId, LocationID);

          //Check form values for the id's of products
          List<OrderItem> orderItems = new List<OrderItem>();
          double Total = 0;
          possibleitems.ForEach(i => {
            string q = form[$"{i.ProductID}"];
            if(!String.IsNullOrWhiteSpace(q)){
              int quantity = int.Parse(q);
              if(quantity !=0){
                orderItems.Add(new OrderItem(){Product= i.Product, ProductID=i.ProductID, Quantity=quantity});
                Total += i.Product.Price * quantity;
              }
            }
          });

          Guid OwnerId = Guid.Parse(_userManager.GetUserId(User));
          Order Order = new Order(){
            CustomerID = Guid.Parse(_userManager.GetUserId(User)),
            LocationID = LocationID,
            OrderItems = orderItems,
            Total = 0};
          
          _service.PlaceOrder(Order);


          // ViewBag.Location = possibleitems.FirstOrDefault().location;
          // ViewBag.Total = Total;
          return RedirectToAction(nameof(Index));
        }

        //This doesnt work. 
        [Authorize]
        [HttpPost]
        [Route("Location/Shop/PlaceOrder")]
        public ActionResult PlaceOrder(IEnumerable<OrderItem> items){
           Log.Verbose("Placing Order");
           Log.Verbose("Item1 {0}",items.FirstOrDefault().ProductID);

          //IEnumerable<OrderItem> items = null; //TempData["OrderItems"];
          int LocationID = (int) TempData["LocationID"]; 
          Guid OwnerId = Guid.Parse(_userManager.GetUserId(User));
          Order Order = new Order(){
            CustomerID = Guid.Parse(_userManager.GetUserId(User)),
            LocationID = LocationID,
            OrderItems = items.ToList(),
            Total = 0};
          
          _service.PlaceOrder(Order);

          return RedirectToAction(nameof(Index));
        }
    }
}